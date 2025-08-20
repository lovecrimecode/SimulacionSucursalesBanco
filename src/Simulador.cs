using System;
using System.Collections.Generic;
using System.Threading;

namespace SimulacionSucursalesBanco
{
    public sealed class Simulador
    {
        private readonly int _numSucursales;
        private readonly int _ventanillasPorSucursal;
        private readonly int _cajerosPorSucursal;
        private readonly EstrategiaAtencion _estrategia;
        private readonly int _clientesTotales;
        private readonly TimeSpan _duracion;
        private readonly int _maxProcesadores;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly List<Sucursal> _sucursales = new List<Sucursal>();
        private readonly List<Ventanilla> _ventanillas = new List<Ventanilla>();
        private readonly List<Cajero> _cajeros = new List<Cajero>();
        private Thread? _generador;

        private int _idClienteSeq = 0;
        private readonly Random _rnd = new Random();

        public Simulador(int numSucursales, int ventanillasPorSucursal, int cajerosPorSucursal, EstrategiaAtencion estrategia, int clientesTotales, TimeSpan duracion, int maxProcesadores)
        {
            _numSucursales = numSucursales;
            _ventanillasPorSucursal = ventanillasPorSucursal;
            _cajerosPorSucursal = cajerosPorSucursal;
            _estrategia = estrategia;
            _clientesTotales = clientesTotales;
            _duracion = duracion;
            _maxProcesadores = maxProcesadores;

            // Crear sucursales
            for (int i = 0; i < _numSucursales; i++)
            {
                _sucursales.Add(new Sucursal(i, $"Sucursal-{i + 1}"));
            }
        }

        public void Iniciar()
        {
            ThreadPool.SetMaxThreads(_maxProcesadores, _maxProcesadores);
            var ct = _cts.Token;

            // Crear y lanzar ventanillas/cajeros
            foreach (var suc in _sucursales)
            {
                for (int v = 0; v < _ventanillasPorSucursal; v++)
                {
                    var vent = new Ventanilla(v, suc, ct, _estrategia);
                    _ventanillas.Add(vent);
                    vent.Start();
                }

                for (int c = 0; c < _cajerosPorSucursal; c++)
                {
                    var caj = new Cajero(c, suc, ct, _estrategia);
                    _cajeros.Add(caj);
                    caj.Start();
                }
            }

            // Generador de clientes
            _generador = new Thread(GenerarClientesLoop)
            { IsBackground = true, Name = "Generador-Clientes" };
            _generador.Start();
        }

        public void Ejecutar()
        {
            DateTime finGeneracion = DateTime.UtcNow + _duracion;

            // Esperar hasta generar todos los clientes o que pase el tiempo
            while (VolumenGenerado() < _clientesTotales && DateTime.UtcNow < finGeneracion)
            {
                Thread.Sleep(50);
            }

            DateTime finMaximo = DateTime.UtcNow + TimeSpan.FromSeconds(30); // Límite de 30 segundos

            // Esperar hasta que todas las colas estén vacías
            bool colasVacias;
            do
            {
                colasVacias = true;
                foreach (var suc in _sucursales)
                {
                    colasVacias &= suc.ColasVacias();
                }
                Thread.Sleep(50);
            } while (!colasVacias && DateTime.UtcNow < finMaximo);
            if (!colasVacias)
            {
                Console.WriteLine("Advertencia: La simulación terminó con clientes aún en cola.");
            }
        }

        public void Detener()
        {
            _cts.Cancel();
            _generador?.Join();

            foreach (var v in _ventanillas) v.Join();
            foreach (var c in _cajeros) c.Join();
            _cts.Dispose();
        }

        private int VolumenGenerado() => _idClienteSeq;

        private void MostrarClienteGenerado(Cliente c)
        {
           Console.WriteLine($"[Generado] Cliente #{c.Id} | Cuenta: {c.Cuenta.Tipo} | " +
                              $"Transacción: {c.Transaccion.Tipo} {(c.Transaccion.Monto > 0 ? $"{c.Transaccion.Monto:C}" : "")} | " +
                              $"Preferencial: {c.Preferencial} | Sucursal: {c.IdSucursalDestino} | Destino: {c.Destino}");
        }

        private void GenerarClientesLoop()
        {
            int localId = 0;
            while (!_cts.IsCancellationRequested && localId < _clientesTotales)
            {
                Thread.Sleep(_rnd.Next(5, 20));

                var suc = _sucursales[_rnd.Next(_numSucursales)];

                // Crear cuenta para el cliente
                var tipoCuenta = _rnd.NextDouble() < 0.65 ? TipoCuenta.Ahorro : TipoCuenta.Corriente;
                var cuenta = new Cuenta(Interlocked.Increment(ref _idClienteSeq), $"Cliente-{localId + 1}", tipoCuenta, 5000m);

                // Elegir operación
                double r = _rnd.NextDouble();
                TipoTransaccion tipoTrans = r < 0.4 ? TipoTransaccion.Deposito
                                       : r < 0.8 ? TipoTransaccion.Retiro
                                                  : TipoTransaccion.Consulta;

                // Monto de la transacción
                decimal monto = tipoTrans != TipoTransaccion.Consulta ? _rnd.Next(100, 10_001) : 0;

                // Crear transacción
                var transaccion = new Transaccion(tipoTrans, cuenta, monto);

                // Preferencial ~ 15%
                bool preferencial = _rnd.NextDouble() < 0.15;

                // Destino: Cajero o Ventanilla
                var destino = _rnd.NextDouble() < 0.55 ? PuntoAtencion.Cajero : PuntoAtencion.Ventanilla;

                // Crear cliente con cuenta y transacción ya listas
                var cliente = new Cliente(
                    Interlocked.Increment(ref _idClienteSeq),
                    cuenta,
                    transaccion,
                    preferencial,
                    suc.Id,
                    destino
                );

                // Encolar el cliente en la sucursal
                suc.RecibirCliente(cliente);
                MostrarClienteGenerado(cliente);

                localId++;
            }
        }

        public void ImprimirMetricasConsola()
        {
            long totalProcesados = 0, totalExitos = 0, totalFallos = 0;
            double totalEsperaMs = 0, totalServicioMs = 0;
            long totalAtVentanilla = 0, totalAtCajero = 0;
            long totalDepositos = 0, totalRetiros = 0, totalConsultas = 0;
            decimal totalFondos = 0;
            int totalClientesCola = 0;

            foreach (var suc in _sucursales)
            {
                var (p, e, f, esperaProm, servProm, atV, atC, depositos, retiros, consultas, fondos, clientesEnCola) = suc.ObtenerMetricas();

                Console.WriteLine($"\nSucursal #{suc.Id}");
                Console.WriteLine($"  Clientes procesados: {p}, Éxitos: {e}, Fallos: {f}");
                Console.WriteLine($"  Espera promedio: {esperaProm:F1} ms, Servicio promedio: {servProm:F1} ms");
                Console.WriteLine($"  Atenciones -> Ventanilla: {atV}, Cajero: {atC}");
                Console.WriteLine($"  Operaciones -> Depósitos: {depositos}, Retiros: {retiros}, Consultas: {consultas}");
                Console.WriteLine($"  Clientes aún en cola: {clientesEnCola}");
                Console.WriteLine($"  Fondos finales: {fondos:C}");

                totalProcesados += p;
                totalExitos += e;
                totalFallos += f;
                totalEsperaMs += esperaProm * Math.Max(p, 1);
                totalServicioMs += servProm * Math.Max(p, 1);
                totalAtVentanilla += atV;
                totalAtCajero += atC;
                totalDepositos += depositos;
                totalRetiros += retiros;
                totalConsultas += consultas;
                totalFondos += fondos;
                totalClientesCola += clientesEnCola;
            }

            double promEsperaGlobal = totalProcesados > 0 ? totalEsperaMs / totalProcesados : 0.0;
            double promServicioGlobal = totalProcesados > 0 ? totalServicioMs / totalProcesados : 0.0;

            Console.WriteLine("\n--- Agregado global ---");
            Console.WriteLine($"Procesados: {totalProcesados}, Éxitos: {totalExitos}, Fallos: {totalFallos}");
            Console.WriteLine($"Espera global promedio: {promEsperaGlobal:F1} ms");
            Console.WriteLine($"Servicio global promedio: {promServicioGlobal:F1} ms");
            Console.WriteLine($"Atenciones -> Ventanilla: {totalAtVentanilla}, Cajero: {totalAtCajero}");
            Console.WriteLine($"Operaciones -> Depósitos: {totalDepositos}, Retiros: {totalRetiros}, Consultas: {totalConsultas}");
            Console.WriteLine($"Clientes aún en cola: {totalClientesCola}");
            Console.WriteLine($"Fondos totales finales: {totalFondos:C}");
        }

        public IReadOnlyList<Sucursal> GetSucursales()
        {
            return _sucursales.AsReadOnly();
        }
    }
}
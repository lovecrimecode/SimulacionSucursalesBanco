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
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly List<Sucursal> _sucursales = new List<Sucursal>();
        private readonly List<Ventanilla> _ventanillas = new List<Ventanilla>();
        private readonly List<Cajero> _cajeros = new List<Cajero>();
        private Thread? _generador;

        private int _idClienteSeq = 0;
        private readonly Random _rnd = new Random();

        public Simulador(
            int numSucursales,
            int ventanillasPorSucursal,
            int cajerosPorSucursal,
            EstrategiaAtencion estrategia,
            int clientesTotales,
            TimeSpan duracion)
        {
            _numSucursales = numSucursales;
            _ventanillasPorSucursal = ventanillasPorSucursal;
            _cajerosPorSucursal = cajerosPorSucursal;
            _estrategia = estrategia;
            _clientesTotales = clientesTotales;
            _duracion = duracion;

            for (int i = 0; i < _numSucursales; i++)
            {
                // Fondos iniciales aleatorios (entre 50k y 120k)
                decimal fondosIni = 50000m + (decimal)_rnd.Next(0, 70001);
                _sucursales.Add(new Sucursal(i, fondosIni));
            }
        }

        public void Iniciar()
        {
            var ct = _cts.Token;

            // Crear y lanzar ventanillas/cajeros
            for (int s = 0; s < _numSucursales; s++)
            {
                var suc = _sucursales[s];

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
            _generador = new Thread(GenerarClientesLoop) { IsBackground = true, Name = "Generador-Clientes" };
            _generador.Start();
        }

        public void Ejecutar()
        {
            // Corre hasta que pase el tiempo _duracion o se alcance el total de clientes
            DateTime fin = DateTime.UtcNow + _duracion;
            while (DateTime.UtcNow < fin && VolumenGenerado() < _clientesTotales)
            {
                Thread.Sleep(100);
            }
        }

        public void Detener()
        {
            _cts.Cancel();
            _generador?.Join();

            foreach (var v in _ventanillas) v.Join();
            foreach (var c in _cajeros) c.Join();
        }

        private int VolumenGenerado() => _idClienteSeq;

        private void GenerarClientesLoop()
        {
            int localId = 0;
            while (!_cts.IsCancellationRequested && localId < _clientesTotales)
            {
                // Ritmo de llegada: entre 5 y 20 ms por cliente (alto tráfico)
                int esperaMs = _rnd.Next(5, 20);
                Thread.Sleep(esperaMs);

                int sucId = _rnd.Next(0, _numSucursales);
                var suc = _sucursales[sucId];

                var destino = _rnd.NextDouble() < 0.55 ? PuntoAtencion.Cajero : PuntoAtencion.Ventanilla;
                var tipoCuenta = _rnd.NextDouble() < 0.65 ? TipoCuenta.Ahorro : TipoCuenta.Corriente;

                // Preferencial ~ 15%
                bool preferencial = _rnd.NextDouble() < 0.15;

                // Operación: depósito 40%, retiro 40%, consulta 20%
                double r = _rnd.NextDouble();
                TipoOperacion op = r < 0.4 ? TipoOperacion.Deposito
                                           : (r < 0.8 ? TipoOperacion.Retiro : TipoOperacion.Consulta);

                decimal monto = 0m;
                if (op != TipoOperacion.Consulta)
                {
                    // Monto entre 100 y 10,000
                    monto = _rnd.Next(100, 10_001);
                }

                int id = Interlocked.Increment(ref _idClienteSeq);
                var cliente = new Cliente(id, tipoCuenta, op, monto, preferencial, sucId, destino);

                suc.EncolarCliente(cliente);
                localId++;
            }
        }

        public void ImprimirMetricasConsola()
        {
            long totalProcesados = 0, totalExitos = 0, totalFallos = 0;
            double totalEsperaMs = 0, totalServicioMs = 0;
            long totalAtVentanilla = 0, totalAtCajero = 0;
            decimal totalFondos = 0;

            for (int i = 0; i < _sucursales.Count; i++)
            {
                var s = _sucursales[i];
                var (p, e, f, esperaProm, servProm, atV, atC, fondos) = s.ObtenerMetricas();

                Console.WriteLine($"\nSucursal #{s.Id}");
                Console.WriteLine($"  Procesados: {p}, Éxitos: {e}, Fallos: {f}");
                Console.WriteLine($"  Espera promedio: {esperaProm:F1} ms");
                Console.WriteLine($"  Servicio promedio: {servProm:F1} ms");
                Console.WriteLine($"  Atenciones -> Ventanilla: {atV}, Cajero: {atC}");
                Console.WriteLine($"  Fondos finales: {fondos:C}");

                totalProcesados += p;
                totalExitos += e;
                totalFallos += f;
                totalEsperaMs += esperaProm * Math.Max(p, 1);
                totalServicioMs += servProm * Math.Max(p, 1);
                totalAtVentanilla += atV;
                totalAtCajero += atC;
                totalFondos += fondos;
            }

            int sucCount = Math.Max(_sucursales.Count, 1);
            double promEsperaGlobal = totalProcesados > 0 ? totalEsperaMs / totalProcesados : 0.0;
            double promServicioGlobal = totalProcesados > 0 ? totalServicioMs / totalProcesados : 0.0;

            Console.WriteLine("\n--- Agregado global ---");
            Console.WriteLine($"Procesados: {totalProcesados}, Éxitos: {totalExitos}, Fallos: {totalFallos}");
            Console.WriteLine($"Espera global promedio: {promEsperaGlobal:F1} ms");
            Console.WriteLine($"Servicio global promedio: {promServicioGlobal:F1} ms");
            Console.WriteLine($"Atenciones -> Ventanilla: {totalAtVentanilla}, Cajero: {totalAtCajero}");
            Console.WriteLine($"Suma de fondos finales (todas las sucursales): {totalFondos:C}");
        }
    }
}


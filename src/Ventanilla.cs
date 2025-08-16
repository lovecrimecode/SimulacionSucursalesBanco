using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SimulacionSucursalesBanco
{
    public sealed class Ventanilla
    {
        private readonly int _id;
        private readonly Sucursal _sucursal;
        private readonly Thread _thread;
        private readonly CancellationToken _ct;
        private readonly EstrategiaAtencion _estrategia;
        private readonly Random _rnd;

        public Ventanilla(int id, Sucursal sucursal, CancellationToken ct, EstrategiaAtencion estrategia, int? seed = null)
        {
            _id = id;
            _sucursal = sucursal;
            _ct = ct;
            _estrategia = estrategia;
            _thread = new Thread(WorkerLoop) { IsBackground = true, Name = $"Ventanilla-{sucursal.Id}-{id}" };
            _rnd = new Random(seed ?? Environment.TickCount ^ id);
        }

        public void Start() => _thread.Start();
        public void Join() => _thread.Join();

        private void WorkerLoop()
        {
            while (!_ct.IsCancellationRequested)
            {
                try
                {
                    Cliente? cliente = _sucursal.TomarClienteVentanilla(_estrategia, _ct);
                    if (cliente == null) continue;

                    cliente.InicioAtencion = DateTime.UtcNow;

                    // Tiempo de servicio simulado (50–250 ms)
                    int servicioMs = _rnd.Next(50, 250);
                    Thread.Sleep(servicioMs);

                    bool exito = Procesar(cliente);
                    cliente.FinAtencion = DateTime.UtcNow;

                    _sucursal.RegistrarResultado(cliente, exito, PuntoAtencion.Ventanilla, servicioMs);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }

        private bool Procesar(Cliente c)
        {
            switch (c.Operacion)
            {
                case TipoOperacion.Deposito:
                    _sucursal.ModificarFondos(c.Monto);
                    return true;
                case TipoOperacion.Retiro:
                    return _sucursal.IntentarRetiro(c.Monto);
                case TipoOperacion.Consulta:
                    _ = _sucursal.Fondos; // leer saldo
                    return true;
                default:
                    return false;
            }
        }
    }
}

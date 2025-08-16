using System;
using System.Threading;

namespace SimulacionSucursalesBanco
{
    public sealed class Cajero
    {
        private readonly int _id;
        private readonly Sucursal _sucursal;
        private readonly Thread _thread;
        private readonly CancellationToken _ct;
        private readonly EstrategiaAtencion _estrategia;
        private readonly Random _rnd;

        public Cajero(int id, Sucursal sucursal, CancellationToken ct, EstrategiaAtencion estrategia, int? seed = null)
        {
            _id = id;
            _sucursal = sucursal;
            _ct = ct;
            _estrategia = estrategia;
            _thread = new Thread(WorkerLoop) { IsBackground = true, Name = $"Cajero-{sucursal.Id}-{id}" };
            _rnd = new Random(seed ?? Environment.TickCount ^ (id * 7));
        }

        public void Start() => _thread.Start();
        public void Join() => _thread.Join();

        private void WorkerLoop()
        {
            while (!_ct.IsCancellationRequested)
            {
                try
                {
                    Cliente? cliente = _sucursal.TomarClienteCajero(_estrategia, _ct);
                    if (cliente == null) continue;

                    cliente.InicioAtencion = DateTime.UtcNow;

                    // Cajero: operaciones un poco más cortas (40–180 ms)
                    int servicioMs = _rnd.Next(40, 180);
                    Thread.Sleep(servicioMs);

                    bool exito = Procesar(cliente);
                    cliente.FinAtencion = DateTime.UtcNow;

                    _sucursal.RegistrarResultado(cliente, exito, PuntoAtencion.Cajero, servicioMs);
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
                    _ = _sucursal.Fondos;
                    return true;
                default:
                    return false;
            }
        }
    }
}

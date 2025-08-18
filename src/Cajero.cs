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
            _sucursal = sucursal ?? throw new ArgumentNullException(nameof(sucursal));
            _ct = ct;
            _estrategia = estrategia;
            _thread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = $"Cajero-{sucursal.Id}-{id}"
            };
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
                    // Tomar cliente desde la sucursal
                    Cliente? cliente = _sucursal.TomarClienteCajero(_estrategia, _ct);
                    if (cliente == null) continue;

                    cliente.InicioAtencion = DateTime.UtcNow;

                    // Simular tiempo de servicio (40–180 ms)
                    int servicioMs = _rnd.Next(40, 180);
                    Thread.Sleep(servicioMs);

                    // Aquí procesamos la transacción asociada
                    bool exito = Procesar(cliente);

                    cliente.FinAtencion = DateTime.UtcNow;

                    // Registrar resultado en la sucursal
                    _sucursal.RegistrarResultado(cliente, exito, PuntoAtencion.Cajero, servicioMs);
                    Console.WriteLine($"[Cajero {_id} Sucursal {_sucursal.Id}] Cliente #{cliente.Id} | {cliente.Transaccion.Tipo} | {(exito ? "Éxito" : "Falló")}");
                }
                catch (OperationCanceledException) { break; }
                catch (ObjectDisposedException) { break; }
            }
        }

        private bool Procesar(Cliente cliente)
        {
            try
            {
                // El cliente ya trae su transacción lista para ejecutar
                cliente.Transaccion.Ejecutar();
                return cliente.Transaccion.Estado == EstadoTransaccion.Completada;
            }
            catch
            {
                return false;
            }
        }

    }
}

using System;
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
            _rnd = new Random(seed ?? Environment.TickCount ^ (id * 13));
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

                    // Simulación de servicio más lento que cajero (50–250 ms)
                    int servicioMs = _rnd.Next(50, 250);
                    Thread.Sleep(servicioMs);

                    bool exito = Procesar(cliente);

                    cliente.FinAtencion = DateTime.UtcNow;

                    _sucursal.RegistrarResultado(cliente, exito, PuntoAtencion.Ventanilla, servicioMs);
                    Console.WriteLine($"[Ventanilla {_id} Sucursal {_sucursal.Id}] Cliente #{cliente.Id} | {cliente.Transaccion.Tipo} | {(exito ? "Éxito" : "Falló")}");
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

        private bool Procesar(Cliente cliente)
        {
            try
            {
                // Ejecutar la transacción que ya trae el cliente
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

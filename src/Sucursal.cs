using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SimulacionSucursalesBanco
{
    public enum EstrategiaAtencion
    {
        FIFO = 1,
        PRIORIDAD = 2
    }

    public sealed class Sucursal
    {
        private readonly object _lockFondos = new object();

        // Colas para ventanilla
        private readonly BlockingCollection<Cliente> _colaVentanilla;
        private readonly BlockingCollection<Cliente> _colaVentanillaPrio;

        // Colas para cajero
        private readonly BlockingCollection<Cliente> _colaCajero;
        private readonly BlockingCollection<Cliente> _colaCajeroPrio;

        // Métricas
        private long _procesados;
        private long _exitos;
        private long _fallos;
        private long _tiempoEsperaAcumMs;
        private long _tiempoServicioAcumMs;
        private long _atencionesVentanilla;
        private long _atencionesCajero;

        public int Id { get; }
        public decimal Fondos { get; private set; }

        public Sucursal(int id, decimal fondosIniciales, int capacidadCola = 10_000)
        {
            Id = id;
            Fondos = fondosIniciales;

            _colaVentanilla = new BlockingCollection<Cliente>(capacidadCola);
            _colaVentanillaPrio = new BlockingCollection<Cliente>(capacidadCola);

            _colaCajero = new BlockingCollection<Cliente>(capacidadCola);
            _colaCajeroPrio = new BlockingCollection<Cliente>(capacidadCola);
        }

        public void EncolarCliente(Cliente c)
        {
            if (c.Destino == PuntoAtencion.Ventanilla)
            {
                if (c.Preferencial) _colaVentanillaPrio.Add(c);
                else _colaVentanilla.Add(c);
            }
            else
            {
                if (c.Preferencial) _colaCajeroPrio.Add(c);
                else _colaCajero.Add(c);
            }
        }

        public Cliente? TomarClienteVentanilla(EstrategiaAtencion estrategia, CancellationToken ct)
        {
            if (estrategia == EstrategiaAtencion.PRIORIDAD)
            {
                if (_colaVentanillaPrio.TryTake(out var prio, 5, ct)) return prio;
                return _colaVentanilla.Take(ct);
            }
            return _colaVentanilla.Take(ct); // FIFO
        }

        public Cliente? TomarClienteCajero(EstrategiaAtencion estrategia, CancellationToken ct)
        {
            if (estrategia == EstrategiaAtencion.PRIORIDAD)
            {
                if (_colaCajeroPrio.TryTake(out var prio, 5, ct)) return prio;
                return _colaCajero.Take(ct);
            }
            return _colaCajero.Take(ct); // FIFO
        }

        public void ModificarFondos(decimal delta)
        {
            lock (_lockFondos)
            {
                Fondos += delta;
            }
        }

        public bool IntentarRetiro(decimal monto)
        {
            lock (_lockFondos)
            {
                if (Fondos >= monto)
                {
                    Fondos -= monto;
                    return true;
                }
                return false;
            }
        }

        public void RegistrarResultado(Cliente c, bool exito, PuntoAtencion donde, int servicioMs)
        {
            Interlocked.Increment(ref _procesados);
            if (exito) Interlocked.Increment(ref _exitos);
            else Interlocked.Increment(ref _fallos);

            if (donde == PuntoAtencion.Ventanilla) Interlocked.Increment(ref _atencionesVentanilla);
            else Interlocked.Increment(ref _atencionesCajero);

            if (c.InicioAtencion.HasValue)
            {
                long esperaMs = (long)(c.InicioAtencion.Value - c.Llegada).TotalMilliseconds;
                Interlocked.Add(ref _tiempoEsperaAcumMs, esperaMs);
            }
            Interlocked.Add(ref _tiempoServicioAcumMs, servicioMs);
        }

        public (long procesados, long exitos, long fallos,
                double esperaPromMs, double servicioPromMs,
                long ventanillaAtenciones, long cajeroAtenciones,
                decimal fondosFinales) ObtenerMetricas()
        {
            double esperaProm = _procesados > 0 ? _tiempoEsperaAcumMs / (double)_procesados : 0.0;
            double servicioProm = _procesados > 0 ? _tiempoServicioAcumMs / (double)_procesados : 0.0;

            return (_procesados, _exitos, _fallos, esperaProm, servicioProm,
                    _atencionesVentanilla, _atencionesCajero, Fondos);
        }
    }
}


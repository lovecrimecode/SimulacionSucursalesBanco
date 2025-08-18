using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SimulacionSucursalesBanco
{
    public enum EstrategiaAtencion
    {
        FIFO = 1,
        PRIORIDAD = 2,
        MIXTA = 3

    }

    public sealed class Sucursal
    {
        // Colas para ventanilla
        private readonly BlockingCollection<Cliente> _colaVentanilla = new BlockingCollection<Cliente>(10_000);
        private readonly BlockingCollection<Cliente> _colaVentanillaPrio = new BlockingCollection<Cliente>(10_000);

        // Colas para cajero
        private readonly BlockingCollection<Cliente> _colaCajero = new BlockingCollection<Cliente>(10_000);
        private readonly BlockingCollection<Cliente> _colaCajeroPrio = new BlockingCollection<Cliente>(10_000);

        // Métricas
        private long _procesados;
        private long _exitos;
        private long _fallos;
        private long _tiempoEsperaAcumMs;
        private long _tiempoServicioAcumMs;
        private long _atencionesVentanilla;
        private long _atencionesCajero;

        private long _depositos;
        private long _retiros;
        private long _consultas;

        public int Id { get; }
        public string Nombre { get; }

        // Fondos totales acumulados de todos los clientes (puede ajustarse según modelo)
        private decimal _fondos;
        public decimal Fondos => _fondos;

        private readonly object _lockFondos = new object();


        public Sucursal(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        #region Gestión de colas

        public void RecibirCliente(Cliente c)
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

        public bool ColasVacias()
        {
            return _colaVentanilla.Count == 0 && _colaVentanillaPrio.Count == 0 &&
                   _colaCajero.Count == 0 && _colaCajeroPrio.Count == 0;
        }

        #endregion

        #region Registro de métricas

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


            // Contadores por tipo de transacción
            switch (c.Transaccion.Tipo)
            {
                case TipoTransaccion.Deposito:
                    Interlocked.Increment(ref _depositos);
                    lock (_lockFondos) { _fondos += c.Transaccion.Monto; }
                    break;
                case TipoTransaccion.Retiro:
                    Interlocked.Increment(ref _retiros);
                    lock (_lockFondos) { _fondos -= c.Transaccion.Monto; }
                    break;
                case TipoTransaccion.Consulta:
                    Interlocked.Increment(ref _consultas);
                    break;
            }
        }

        public (long procesados, long exitos, long fallos,
                double esperaPromMs, double servicioPromMs,
                long ventanillaAtenciones, long cajeroAtenciones,
                long depositos, long retiros, long consultas,
                decimal fondosFinales, int clientesEnCola) ObtenerMetricas()
        {
            double esperaProm = _procesados > 0 ? _tiempoEsperaAcumMs / (double)_procesados : 0.0;
            double servicioProm = _procesados > 0 ? _tiempoServicioAcumMs / (double)_procesados : 0.0;

            int clientesEnCola = _colaVentanilla.Count + _colaVentanillaPrio.Count +
                                  _colaCajero.Count + _colaCajeroPrio.Count;

            return (_procesados, _exitos, _fallos, esperaProm, servicioProm,
                    _atencionesVentanilla, _atencionesCajero,
                    _depositos, _retiros, _consultas,
                    _fondos, clientesEnCola);
        }

        #endregion
    }

}

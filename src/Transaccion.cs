using SimulacionSucursalesBanco.src;
using System;

namespace SimulacionSucursalesBanco
{
    public enum TipoTransaccion
    {
        Deposito = 1,
        Retiro = 2,
        Consulta = 3
    }

    public enum EstadoTransaccion
    {
        Pendiente = 0,
        Completada = 1,
        Fallida = 2
    }

    public sealed class Transaccion
    {
        public TipoTransaccion Tipo { get; }
        public Cuenta Cuenta { get; }
        public decimal Monto { get; }
        public EstadoTransaccion Estado { get; private set; }

        // Registro de auditoría
        public DateTime Creacion { get; }
        public DateTime? Ejecucion { get; private set; }
        public PuntoAtencion? PuntoAtencion { get; private set; }
        public int? IdSucursal { get; private set; }
        public string? Mensaje { get; private set; }

        public Transaccion(TipoTransaccion tipo, Cuenta cuenta, decimal monto = 0)
        {
            if (tipo != TipoTransaccion.Consulta && monto <= 0)
                throw new ArgumentException("Monto debe ser positivo para depósito/retiro.", nameof(monto));

            Tipo = tipo;
            Cuenta = cuenta ?? throw new ArgumentNullException(nameof(cuenta));
            Monto = monto;
            Estado = EstadoTransaccion.Pendiente;
            Creacion = DateTime.UtcNow;
        }

        public void Ejecutar(PuntoAtencion? punto = null, int? idSucursal = null)
        {
            if (Estado != EstadoTransaccion.Pendiente)
                return; // ya procesada

            Ejecucion = DateTime.UtcNow;
            PuntoAtencion = punto;
            IdSucursal = idSucursal;

            try
            {
                bool ok = false;
                switch (Tipo)
                {
                    case TipoTransaccion.Deposito:
                        ok = Cuenta.Depositar(Monto);
                        break;

                    case TipoTransaccion.Retiro:
                        ok = Cuenta.Retirar(Monto);
                        break;

                    case TipoTransaccion.Consulta:
                        _ = Cuenta.ConsultarSaldo();
                        ok = true;
                        break;
                }

                Estado = ok ? EstadoTransaccion.Completada : EstadoTransaccion.Fallida;
                Mensaje = ok ? "OK" : "Fondos insuficientes o error de operación.";
            }
            catch (Exception ex)
            {
                Estado = EstadoTransaccion.Fallida;
                Mensaje = $"Excepción: {ex.Message}";
            }
        }

        public override string ToString()
        {
            string infoSucursal = IdSucursal.HasValue ? $" Suc:{IdSucursal}" : "";
            string infoPunto = PuntoAtencion.HasValue ? $" [{PuntoAtencion}]" : "";

            return $"[{Tipo}] {Monto:C} - Estado: {Estado}{infoSucursal}{infoPunto} - {Mensaje}";
        }
    }
}

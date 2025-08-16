using System;

namespace SimulacionSucursalesBanco
{
    public enum TipoCuenta
    {
        Ahorro = 1,
        Corriente = 2
    }

    public enum TipoOperacion
    {
        Deposito = 1,
        Retiro = 2,
        Consulta = 3
    }

    public sealed class Cliente
    {
        public int Id { get; }
        public TipoCuenta TipoCuenta { get; }
        public TipoOperacion Operacion { get; }
        public decimal Monto { get; }
        public bool Preferencial { get; } // prioridad
        public DateTime Llegada { get; }
        public DateTime? InicioAtencion { get; set; }
        public DateTime? FinAtencion { get; set; }
        public int IdSucursalDestino { get; }
        public PuntoAtencion Destino { get; } // Ventanilla o Cajero

        public TimeSpan TiempoEspera =>
            (InicioAtencion.HasValue ? InicioAtencion.Value : DateTime.UtcNow) - Llegada;

        public Cliente(
            int id,
            TipoCuenta tipoCuenta,
            TipoOperacion operacion,
            decimal monto,
            bool preferencial,
            int idSucursalDestino,
            PuntoAtencion destino)
        {
            Id = id;
            TipoCuenta = tipoCuenta;
            Operacion = operacion;
            Monto = monto;
            Preferencial = preferencial;
            Llegada = DateTime.UtcNow;
            IdSucursalDestino = idSucursalDestino;
            Destino = destino;
        }

        public override string ToString()
            => $"Cliente#{Id} [{(Preferencial ? "PRIO" : "NOR")}] {Operacion} {Monto:C} ({TipoCuenta}) -> {Destino} Suc:{IdSucursalDestino}";
    }

    public enum PuntoAtencion
    {
        Ventanilla = 1,
        Cajero = 2
    }
}

using SimulacionSucursalesBanco.src;


namespace SimulacionSucursalesBanco
{
    public sealed class Cliente
    {
        public int Id { get; }
        public bool Preferencial { get; } // prioridad en la cola
        public DateTime Llegada { get; }
        public DateTime? InicioAtencion { get; set; }
        public DateTime? FinAtencion { get; set; }

        // El cliente está asociado a una cuenta
        public Cuenta Cuenta { get; }

        // La transacción que solicita
        public Transaccion Transaccion { get; }

        // A dónde quiere ir (cajero o ventanilla)
        public int IdSucursalDestino { get; }
        public PuntoAtencion Destino { get; }

        public TimeSpan TiempoEspera =>
            (InicioAtencion.HasValue ? InicioAtencion.Value : DateTime.UtcNow) - Llegada;

        public Cliente(
            int id,
            Cuenta cuenta,
            Transaccion transaccion,
            bool preferencial,
            int idSucursalDestino,
            PuntoAtencion destino)
        {
            Id = id;
            Cuenta = cuenta ?? throw new ArgumentNullException(nameof(cuenta));
            Transaccion = transaccion ?? throw new ArgumentNullException(nameof(transaccion));
            Preferencial = preferencial;
            Llegada = DateTime.UtcNow;
            IdSucursalDestino = idSucursalDestino;
            Destino = destino;
        }

        public override string ToString()
            => $"Cliente#{Id} [{(Preferencial ? "PRIO" : "NOR")}] " +
               $"{Transaccion.Tipo} {Transaccion.Monto:C} ({Cuenta.ToString}) -> " +
               $"{Destino} Suc:{IdSucursalDestino}";
    }

    public enum PuntoAtencion
    {
        Ventanilla = 1,
        Cajero = 2
    }
}

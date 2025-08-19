using SimulacionSucursalesBanco;

namespace SimulacionSucursalesBanco
{
    public interface IEstrategiaAtencion
    {
        void AgregarCliente(Cliente cliente);
        Cliente AtenderCliente();
        bool TieneClientes();
    }
}

using System.Collections.Generic;

namespace SimulacionSucursalesBanco
{
    public class EstrategiaFIFO : IEstrategiaAtencion
    {
        private Queue<Cliente> cola = new Queue<Cliente>();

        public void AgregarCliente(Cliente cliente)
        {
            if (cliente != null)
                cola.Enqueue(cliente);
        }
        public Cliente? AtenderCliente()
        {
            return cola.Count > 0 ? cola.Dequeue() : null;
        }
        public bool TieneClientes()
        {
            return cola.Count > 0;
        }
    }
}

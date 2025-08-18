using System.Collections.Generic;

namespace SimulacionSucursalesBanco
{
    public class EstrategiaFIFO : IEstrategiaAtencion
    {
        private Queue<Cliente> cola = new Queue<Cliente>();

        public void AgregarCliente(Cliente cliente)
        {
            cola.Enqueue(cliente);
        }

        public Cliente AtenderCliente()
        {
            if (cola.Count == 0) return null;
            return cola.Dequeue();
        }

        public bool TieneClientes()
        {
            return cola.Count > 0;
        }
    }
}

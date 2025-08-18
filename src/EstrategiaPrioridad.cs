using SimulacionSucursalesBanco.src.clases;
using System.Collections.Generic;
using System.Linq;

namespace SimulacionSucursalesBanco
{
    public class EstrategiaPrioridad
    {
        private List<Cliente> cola = new List<Cliente>();

        public void AgregarCliente(Cliente cliente)
        {
            if (cliente != null)
                cola.Add(cliente);
        }

        public Cliente AtenderCliente()
        {
            if (cola.Count == 0) return null;

            // escogera de  primero los clientes preferenciales
            var clientePrioritario = cola.FirstOrDefault(c => c.Preferencial);

            if (clientePrioritario != null)
            {
                cola.Remove(clientePrioritario);
                return clientePrioritario;
            }

            // Si no hay clientes preferenciales, atiende al primero de la lista
            var clienteNormal = cola[0];
            cola.RemoveAt(0);
            return clienteNormal;
        }

        public int CantidadEnCola() => cola.Count;
    }
}

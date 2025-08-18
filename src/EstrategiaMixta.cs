using System.Collections.Generic;

namespace SimulacionSucursalesBanco
{
    public class EstrategiaMixta
    {
        private Queue<Cliente> colaFIFO = new Queue<Cliente>();
        private EstrategiaPrioridad estrategiaPrioridad = new EstrategiaPrioridad();

        // Agrega un cliente a la estrategia correspondiente
        public void AgregarCliente(Cliente cliente)
        {
            if (cliente == null) return;

            if (cliente.Preferencial)
            {
                estrategiaPrioridad.AgregarCliente(cliente);
            }
            else
            {
                colaFIFO.Enqueue(cliente);
            }
        }

        // Atender al siguiente cliente según la estrategia mixta
        public Cliente? AtenderCliente()
        {
            // Prioridad a clientes preferenciales
            if (estrategiaPrioridad.CantidadEnCola() > 0)
            {
                return estrategiaPrioridad.AtenderCliente();
            }

            // Si no hay preferenciales, atiende al primero en FIFO
            if (colaFIFO.Count > 0)
            {
                return colaFIFO.Dequeue();
            }

            // No hay clientes
            return null;
        }

        // Saber cuántos clientes quedan en la cola
        public int CantidadTotalClientes()
        {
            return colaFIFO.Count + estrategiaPrioridad.CantidadEnCola();
        }
    }
}

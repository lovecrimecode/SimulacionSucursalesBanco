using System.Collections.Generic;

namespace SimulacionSucursalesBanco
{
    public class EstrategiaMixta : IEstrategiaAtencion
    {
        private Queue<Cliente> colaFIFO = new Queue<Cliente>();
        private EstrategiaPrioridad estrategiaPrioridad = new EstrategiaPrioridad();
        private readonly Random random = new Random();

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
            if (estrategiaPrioridad.CantidadEnCola() > 0 && (colaFIFO.Count == 0 || random.NextDouble() < 0.75))
            {
                return estrategiaPrioridad.AtenderCliente();
            }
            return colaFIFO.Count > 0 ? colaFIFO.Dequeue() : null;
        }

        public bool TieneClientes()
        {
            return colaFIFO.Count > 0 || estrategiaPrioridad.TieneClientes();
        }

        public int CantidadEnCola()
        {
            return colaFIFO.Count + estrategiaPrioridad.CantidadEnCola();
        }
    }
    }

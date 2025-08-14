using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimulacionSucursalesBanco
{
    public class MainApp
    {
        public void IniciarSimulacion()
        {
            // Simulación mínima mientras se implementa la lógica real
            Task ventanilla = Task.Run(() => ProcesarVentanilla(1));
            Task cajero = Task.Run(() => ProcesarCajero(1));

            Task.WaitAll(ventanilla, cajero);
        }

        private void ProcesarVentanilla(int id)
        {
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"[Ventanilla {id}] Atendiendo cliente {i}...");
                Thread.Sleep(500); // Simula tiempo de atención
            }
            Console.WriteLine($"[Ventanilla {id}] Atención finalizada.");
        }

        private void ProcesarCajero(int id)
        {
            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"[Cajero {id}] Procesando transacción {i}...");
                Thread.Sleep(700); // Simula tiempo de operación
            }
            Console.WriteLine($"[Cajero {id}] Operaciones completadas.");
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimulacionSucursalesBanco
{
    public class MainApp
    {
        public void IniciarSimulacion()
        {
            // Simulaci�n m�nima mientras se implementa la l�gica real
            Task ventanilla = Task.Run(() => ProcesarVentanilla(1));
            Task cajero = Task.Run(() => ProcesarCajero(1));

            Task.WaitAll(ventanilla, cajero);
        }

        private void ProcesarVentanilla(int id)
        {
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"[Ventanilla {id}] Atendiendo cliente {i}...");
                Thread.Sleep(500); // Simula tiempo de atenci�n
            }
            Console.WriteLine($"[Ventanilla {id}] Atenci�n finalizada.");
        }

        private void ProcesarCajero(int id)
        {
            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"[Cajero {id}] Procesando transacci�n {i}...");
                Thread.Sleep(700); // Simula tiempo de operaci�n
            }
            Console.WriteLine($"[Cajero {id}] Operaciones completadas.");
        }
    }
}

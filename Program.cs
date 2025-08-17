
using System;

namespace SimulacionSucursalesBanco
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Simulación de Sucursales Bancarias";
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("=========================================");
            Console.WriteLine("  SISTEMA DE SIMULACIÓN - BANCO ITLA");
            Console.WriteLine("=========================================\n");
            Console.ResetColor();

            Console.WriteLine("Iniciando simulación...\n");

            // Ahora solo llamas a tu MainApp con el nuevo método
            MainApp app = new MainApp();
            app.IniciarSimulacion();

            Console.WriteLine("\nSimulación finalizada.");
            Console.WriteLine("Presione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}

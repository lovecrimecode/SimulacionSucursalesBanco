using System;

namespace SimulacionSucursalesBanco
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var app = new MainApp();
                app.IniciarSimulacion();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPresione una tecla para salir...");
            Console.ReadKey();
        }
    }
}
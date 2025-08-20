using System;

namespace SimulacionSucursalesBanco
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Seleccione modo:");
            Console.WriteLine("1. Pruebas unitarias");
            Console.WriteLine("2. Simulación / Métricas");
            var opcion = Console.ReadLine();
            if (opcion == "1")
            {
                Console.WriteLine("Ejecutando pruebas unitarias...");
                PruebasUnitarias.RunAll();
            }
            else if (opcion == "2")
            {
                Console.WriteLine("Ejecutando simulación...");
                var app = new MainApp();
                app.IniciarSimulacion();
            }
            else
            {
                Console.WriteLine("Opción inválida");
            }
            Console.WriteLine("\nPresione una tecla para salir...");
            Console.ReadKey();
        }
    }
}
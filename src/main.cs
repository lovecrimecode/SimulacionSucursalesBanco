<<<<<<< Updated upstream
=======
using SimulacionSucursalesBanco;
>>>>>>> Stashed changes
using System;
using System.IO;

namespace SimulacionSucursalesBanco
{
    public class MainApp
    {
        public void IniciarSimulacion()
        {
            Console.WriteLine("=== Simulación Bancaria Multi-Sucursal ===");

            int sucursales = LeerInt("Cantidad de sucursales (ej: 1, 3, 5): ");
            int ventanillasPorSucursal = LeerInt("Ventanillas por sucursal (ej: 2, 4): ");
            int cajerosPorSucursal = LeerInt("Cajeros por sucursal (ej: 1, 2): ");
            int clientesTotales = LeerInt("Clientes simulados (ej: 20, 50, 100): ");
            int duracionSegundos = LeerInt("Duración de la simulación en segundos (ej: 10, 20, 60): ");

            Console.WriteLine("Estrategia de atención (1=FIFO, 2=Prioridad, 3=Mixta): ");
            int estrategiaNum = LeerInt("Seleccione opción: ");
            EstrategiaAtencion estrategia = estrategiaNum switch
            {
                1 => EstrategiaAtencion.FIFO,
                //  2 => EstrategiaAtencion.Prioridad,
                //  3 => EstrategiaAtencion.Mixta,
                _ => EstrategiaAtencion.FIFO
            };

            Console.WriteLine($"\nSucursales: {sucursales}, Ventanillas/Sucursal: {ventanillasPorSucursal}, Cajeros/Sucursal: {cajerosPorSucursal}");
            Console.WriteLine($"Estrategia: {estrategia}, Clientes a generar: {clientesTotales}, Duración: {duracionSegundos}s\n");

            var simulador = new Simulador(
                sucursales,
                ventanillasPorSucursal,
                cajerosPorSucursal,
                estrategia,
                clientesTotales,
                TimeSpan.FromSeconds(duracionSegundos)); 

            simulador.Iniciar();
            simulador.Ejecutar();
            simulador.Detener();

            Console.WriteLine("\n=== Métricas ===");
            simulador.ImprimirMetricasConsola();
        }

        private int LeerInt(string mensaje)
        {
            int valor;
            do
            {
                Console.Write(mensaje);
            } while (!int.TryParse(Console.ReadLine(), out valor) || valor <= 0);
            return valor;
        }
    }
/*
    public static class TestUtils
    {
        public static void GuardarResultado(string nombreTest, string resultado)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_results.txt");
            string linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {nombreTest} | {resultado}";
            File.AppendAllLines(ruta, new[] { linea });
        }
    }*/
}
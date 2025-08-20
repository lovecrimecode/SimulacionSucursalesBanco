using SimulacionSucursalesBanco;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimulacionSucursalesBanco
{
    public class MainApp
    {
        public void IniciarSimulacion()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=========================================");
            Console.WriteLine(" SIMULACION DE SUCURSALES DE BANCO ");
            Console.WriteLine("=========================================\n");
            Console.ResetColor();
            int sucursales = LeerInt("Cantidad de sucursales (ej: 1, 3, 5): ");
            int ventanillasPorSucursal = LeerInt("Ventanillas por sucursal (ej: 2, 4): ");
            int cajerosPorSucursal = LeerInt("Cajeros por sucursal (ej: 1, 2): ");
            int clientesTotales = LeerInt("Clientes simulados (ej: 20, 50, 100): ");
            int duracionSegundos = LeerInt("Duración de la simulación en segundos (ej: 10, 20, 60): ");
            int maxProcesadores = LeerInt("Máximo de procesadores (ej: 4, 8): ");
            Console.WriteLine("Estrategia de atención (1=FIFO, 2=Prioridad, 3=Mixta): ");
            int estrategiaNum = LeerInt("Seleccione opción: ");
            EstrategiaAtencion estrategia = estrategiaNum switch
            {
                1 => EstrategiaAtencion.FIFO,
                2 => EstrategiaAtencion.Prioridad,
                3 => EstrategiaAtencion.Mixta,
                _ => EstrategiaAtencion.FIFO
            };
            var simulador = new Simulador(sucursales, ventanillasPorSucursal, cajerosPorSucursal, estrategia, clientesTotales, TimeSpan.FromSeconds(duracionSegundos), maxProcesadores);
            simulador.Iniciar();
            simulador.Ejecutar();
            simulador.Detener();
            Console.WriteLine("\n=== Métricas ===");
            simulador.ImprimirMetricasConsola();
            GuardarMetricas(simulador, estrategia);
        }

        private void GuardarMetricas(Simulador simulador, EstrategiaAtencion estrategia)
        {
            string ruta = Path.Combine("metrics", $"estrategia_{estrategia}.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            var clientesAtendidos = new List<Cliente>();
            foreach (var suc in simulador.GetSucursales())
                clientesAtendidos.AddRange(suc.ObtenerClientesAtendidos());
            var calculadora = new CalculadoraMetricas(clientesAtendidos);
            calculadora.GuardarMetricasEnArchivo(ruta);
            using var writer = new StreamWriter(ruta, true);
            writer.WriteLine($"Simulación - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            foreach (var suc in simulador.GetSucursales())
            {
                (long p, long e, long f, double esperaProm, double servProm, long atV, long atC, long depositos, long retiros, long consultas, decimal fondos, int clientesEnCola) = suc.ObtenerMetricas();
                writer.WriteLine($"Sucursal #{suc.Id} | Clientes procesados: {p}, Éxitos: {e}, Fallos: {f}");
                writer.WriteLine($"Espera promedio: {esperaProm:F1} ms | Servicio promedio: {servProm:F1} ms");
                writer.WriteLine($"Atenciones -> Ventanilla: {atV}, Cajero: {atC}");
                writer.WriteLine($"Operaciones -> Depósitos: {depositos}, Retiros: {retiros}, Consultas: {consultas}");
                writer.WriteLine($"Clientes en cola: {clientesEnCola}, Fondos: {fondos:C}");
            }
            Console.WriteLine($"Métricas guardadas correctamente en {ruta}");
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
}
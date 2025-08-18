using SimulacionSucursalesBanco.src.clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SimulacionSucursalesBanco
{
    public class MainApp
    {
        public void IniciarSimulacion()
        { 
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("=========================================");
            Console.WriteLine("  SIMULACION DE SUCURSALES DE BANCO   ");
            Console.WriteLine("=========================================\n");
            Console.ResetColor();


            Console.WriteLine("=== Iniciando la Simulacion Bancaria Multi-Sucursal ===");

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
                2 => EstrategiaAtencion.Prioridad,
                3 => EstrategiaAtencion.Mixta,
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
            GuardarMetricas(simulador, estrategia);
        }

        private void GuardarMetricas(Simulador simulador, EstrategiaAtencion estrategia)
        {
            string ruta = Path.Combine("metrics", $"estrategia_{estrategia}.txt");
            string? carpeta = Path.GetDirectoryName(ruta);
            if (!string.IsNullOrEmpty(carpeta))
                Directory.CreateDirectory(carpeta);
            var clientesAtendidos = new List<Cliente>();
            var sucursales = (List<Sucursal>)simulador.GetType()
                .GetField("_sucursales", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(simulador);
            foreach (var suc in sucursales)
            {
                clientesAtendidos.AddRange(suc.ObtenerClientesAtendidos());
            }
            var calculadora = new CalculadoraMetricas(clientesAtendidos);
            calculadora.GuardarMetricasEnArchivo(ruta);
            using (var writer = new StreamWriter(ruta, true))
            {
                writer.WriteLine($"Simulación - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                foreach (var suc in sucursales)
                {
                    var (p, e, f, esperaProm, servProm, atV, atC, depositos, retiros, consultas, fondos, clientesEnCola) = suc.ObtenerMetricas();
                    writer.WriteLine($"Sucursal #{suc.Id}");
                    writer.WriteLine($"Clientes procesados: {p}, Éxitos: {e}, Fallos: {f}");
                    writer.WriteLine($"Espera promedio: {esperaProm:F1} ms, Servicio promedio: {servProm:F1} ms");
                    writer.WriteLine($"Atenciones -> Ventanilla: {atV}, Cajero: {atC}");
                    writer.WriteLine($"Operaciones -> Depósitos: {depositos}, Retiros: {retiros}, Consultas: {consultas}");
                    writer.WriteLine($"Clientes en cola: {clientesEnCola}, Fondos: {fondos:C}");
                }
            }
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
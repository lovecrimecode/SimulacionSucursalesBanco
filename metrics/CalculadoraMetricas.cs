using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimulacionSucursalesBanco
{
    public class CalculadoraMetricas
    {
        private readonly List<Cliente> clientes;

        public CalculadoraMetricas(List<Cliente> clientes)
        {
            this.clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
        }

        // Tiempo promedio de espera
        public TimeSpan TiempoPromedioEspera()
        {
            if (clientes.Count == 0) return TimeSpan.Zero;
            var atendidos = clientes.Where(c => c.InicioAtencion.HasValue).ToList();
            if (atendidos.Count == 0) return TimeSpan.Zero;
            return TimeSpan.FromMilliseconds(atendidos.Average(c => (c.InicioAtencion.Value - c.Llegada).TotalMilliseconds));
        }

        // Tiempo promedio de atención
        public TimeSpan TiempoPromedioAtencion()
        {
            var atendidos = clientes.Where(c => c.InicioAtencion.HasValue && c.FinAtencion.HasValue).ToList();
            if (atendidos.Count == 0) return TimeSpan.Zero;
            return TimeSpan.FromMilliseconds(atendidos.Average(c => (c.FinAtencion.Value - c.InicioAtencion.Value).TotalMilliseconds));
        }

        // Total de clientes atendidos
        public int TotalClientesAtendidos()
        {
            return clientes.Count(c => c.FinAtencion.HasValue);
        }

        // Obtener todas las métricas
        public (TimeSpan esperaPromedio, TimeSpan servicioPromedio, int totalAtendidos, double speedup, double eficiencia, double transaccionesPorHora) ObtenerMetricas()
        {
            if (!clientes.Any())
                return (TimeSpan.Zero, TimeSpan.Zero, 0, 0.0, 0.0, 0.0);

            var esperaPromedio = TiempoPromedioEspera();
            var servicioPromedio = TiempoPromedioAtencion();
            int totalAtendidos = TotalClientesAtendidos();

            // Speedup: Tiempo secuencial (suma de tiempos de servicio) / Tiempo paralelo (duración de simulación)
            double tiempoSecuencialMs = clientes.Where(c => c.InicioAtencion.HasValue && c.FinAtencion.HasValue)
                            .Sum(c => (c.FinAtencion.Value - c.InicioAtencion.Value).TotalMilliseconds);
            double tiempoParaleloMs = clientes.Any() ? (clientes.Max(c => c.FinAtencion ?? c.Llegada) - clientes.Min(c => c.Llegada)).TotalMilliseconds : 1.0;
            double speedup = tiempoSecuencialMs / tiempoParaleloMs;

            // Eficiencia: Speedup / Número de hilos (aproximado como sucursales * (ventanillas + cajeros))
            int numHilos = clientes.Any() ? (clientes.Max(c => c.IdSucursalDestino) + 1) * 6 : 1; // Asumiendo 4 ventanillas + 2 cajeros por sucursal
            double eficiencia = speedup / numHilos;

            // Transacciones por hora: Total atendidos / Tiempo paralelo en horas
            double transaccionesPorHora = totalAtendidos / (tiempoParaleloMs / (1000.0 * 3600.0));

            return (esperaPromedio, servicioPromedio, totalAtendidos, speedup, eficiencia, transaccionesPorHora);
        }

        // Guardar métricas en archivo txt
        public void GuardarMetricasEnArchivo(string rutaArchivo)
        {
            try
            {
                string carpeta = Path.GetDirectoryName(rutaArchivo);
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);
                var (esperaPromedio, servicioPromedio, totalAtendidos, speedup, eficiencia, transaccionesPorHora) = ObtenerMetricas();
                string contenido = $"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                  $"Tiempo promedio de espera: {esperaPromedio}\n" +
                                  $"Tiempo promedio de atención: {servicioPromedio}\n" +
                                  $"Total de clientes atendidos: {totalAtendidos}\n" +
                                  $"Speedup: {speedup:F2}\n" +
                                  $"Eficiencia: {eficiencia:F2}\n" +
                                  $"Transacciones por hora: {transaccionesPorHora:F0}\n" +
                                  $"-----------------------------\n";
                File.AppendAllText(rutaArchivo, contenido);
                Console.WriteLine("Métricas guardadas correctamente en " + rutaArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar métricas: " + ex.Message);
            }
        }
    }
}
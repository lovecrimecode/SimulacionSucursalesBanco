using SimulacionSucursalesBanco.src.clases;
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
            return TimeSpan.FromSeconds(clientes.Average(c => c.TiempoEspera.TotalSeconds));
        }

        // Tiempo promedio de atención
        public TimeSpan TiempoPromedioAtencion()
        {
            var atendidos = clientes.Where(c => c.InicioAtencion.HasValue && c.FinAtencion.HasValue).ToList();
            if (atendidos.Count == 0) return TimeSpan.Zero;
            return TimeSpan.FromSeconds(atendidos.Average(c => (c.FinAtencion.Value - c.InicioAtencion.Value).TotalSeconds));
        }

        // Total de clientes atendidos
        public int TotalClientesAtendidos()
        {
            return clientes.Count(c => c.FinAtencion.HasValue);
        }

        // Guardar métricas en archivo txt
        public void GuardarMetricasEnArchivo(string rutaArchivo)
        {
            try
            {
                string carpeta = Path.GetDirectoryName(rutaArchivo);
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string contenido = $"Fecha: {DateTime.Now}\n" +
                                   $"Tiempo promedio de espera: {TiempoPromedioEspera()}\n" +
                                   $"Tiempo promedio de atención: {TiempoPromedioAtencion()}\n" +
                                   $"Total de clientes atendidos: {TotalClientesAtendidos()}\n" +
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

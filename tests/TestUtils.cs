using System;
using System.IO;

namespace SimulacionSucursalesBanco
{
    public static class TestUtils
    {
        private static readonly object _lock = new object();

        public static void GuardarResultado(string nombrePrueba, bool exito, string descripcion, string motivoFallo = "")
        {
            lock (_lock)
            {
                string resultado = exito ? "OK" : $"FALLÓ ({motivoFallo})";
                string mensaje = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {nombrePrueba} | {resultado} | {descripcion}";
                try
                {
                    File.AppendAllText("tests/test_results.txt", mensaje + Environment.NewLine);
                    Console.WriteLine(mensaje);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al guardar resultado de {nombrePrueba}: {ex.Message}");
                }
            }
        }
    }
}
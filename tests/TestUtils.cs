using System;
using System.IO;

namespace SimulacionSucursalesBanco
{
    public static class TestUtils
    {
        public static void GuardarResultado(string nombreTest, string resultado)
        {
            string ruta = Path.Combine("tests", "test_results.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(ruta));
            string linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {nombreTest} | {resultado}";
            File.AppendAllLines(ruta, new[] { linea });
        }
    }
}
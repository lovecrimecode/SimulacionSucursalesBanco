using System;
using System.IO;

public static class TestUtils
{
    public static void GuardarResultado(string nombreTest, string resultado)
    {
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_results.txt");
        string linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {nombreTest} | {resultado}";
        File.AppendAllLines(ruta, new[] { linea });
    }
}
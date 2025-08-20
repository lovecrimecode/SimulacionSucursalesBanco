using System;
using System.Reflection;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public static class PruebasUnitarias
    {
        public static void RunAll()
        {
            Console.WriteLine("Ejecutando pruebas unitarias...");
            var testClasses = new[] { typeof(ColaTests), typeof(CuentaTests), typeof(ConcurrenciaTests) };
            foreach (var testClass in testClasses)
            {
                foreach (var method in testClass.GetMethods())
                {
                    if (Attribute.IsDefined(method, typeof(FactAttribute)))
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(testClass);
                            method.Invoke(instance, null);
                            Console.WriteLine($"Prueba {method.Name}: Completada");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error en {method.Name}: {ex.InnerException?.Message ?? ex.Message}");
                        }
                    }
                }
            }
            Console.WriteLine("Pruebas finalizadas. Resultados en tests/test_results.txt");
        }
    }
}
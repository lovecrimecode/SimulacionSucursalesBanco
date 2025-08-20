using System;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public static class PruebasUnitarias
    {
        public static void RunAll()
        {
            var colaTests = new ColaTests();
            var transaccionesTests = new CuentaTests();
            var multiprocesoTests = new ConcurrenciaTests();

            RunTest(() => colaTests.EncolarDesencolar_ClienteCorrecto(), nameof(ColaTests.EncolarDesencolar_ClienteCorrecto));
            RunTest(() => colaTests.Prioridad_ClientePreferencialPrimero(), nameof(ColaTests.Prioridad_ClientePreferencialPrimero));
            RunTest(() => colaTests.Mixta_ClientePreferencialPrimero(), nameof(ColaTests.Mixta_ClientePreferencialPrimero));
            RunTest(() => transaccionesTests.Depositar_SumaCorrectamente(), nameof(CuentaTests.Depositar_SumaCorrectamente));
            RunTest(() => transaccionesTests.Retirar_DescuentaCorrectamente(), nameof(CuentaTests.Retirar_DescuentaCorrectamente));
            RunTest(() => transaccionesTests.Retirar_SaldoInsuficiente_Falla(), nameof(CuentaTests.Retirar_SaldoInsuficiente_Falla));
            RunTest(() => multiprocesoTests.DepositosConcurrentes_SaldoCorrecto(), nameof(ConcurrenciaTests.DepositosConcurrentes_SaldoCorrecto));
        }

        private static void RunTest(Action test, string testName)
        {
            try
            {
                test();
                Console.WriteLine($"Prueba {testName}: OK");
                TestUtils.GuardarResultado(testName, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Prueba {testName}: FALLÓ ({ex.Message})");
                TestUtils.GuardarResultado(testName, "FALLÓ");
            }
        }
    }
}
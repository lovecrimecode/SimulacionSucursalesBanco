using SimulacionSucursalesBanco;
using System.Threading.Tasks;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public class ConcurrenciaTests
    {
        [Fact]
        public void DepositosConcurrentes_SaldoCorrecto()
        {
            var cuenta = new Cuenta(10, "Concurrente", TipoCuenta.Ahorro, 0m);
            int tareas = 100;
            decimal monto = 10m;

            Parallel.For(0, tareas, _ => cuenta.Depositar(monto));
            bool ok = cuenta.ConsultarSaldo() == tareas * monto;

            TestUtils.GuardarResultado(nameof(DepositosConcurrentes_SaldoCorrecto), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }
    }
}
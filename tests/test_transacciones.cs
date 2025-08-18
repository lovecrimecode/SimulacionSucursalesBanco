using SimulacionSucursalesBanco;
using SimulacionSucursalesBanco.src.clases;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public class CuentaTests
    {
        [Fact]
        public void Depositar_SumaCorrectamente()
        {
            var cuenta = new Cuenta(1, "Test", TipoCuenta.Ahorro, 1000m);
            bool ok = cuenta.Depositar(500m) && cuenta.ConsultarSaldo() == 1500m;
            TestUtils.GuardarResultado(nameof(Depositar_SumaCorrectamente), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }

        [Fact]
        public void Retirar_DescuentaCorrectamente()
        {
            var cuenta = new Cuenta(2, "Test", TipoCuenta.Ahorro, 1000m);
            bool ok = cuenta.Retirar(400m) && cuenta.ConsultarSaldo() == 600m;
            TestUtils.GuardarResultado(nameof(Retirar_DescuentaCorrectamente), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }

        [Fact]
        public void Retirar_SaldoInsuficiente_Falla()
        {
            var cuenta = new Cuenta(3, "Test", TipoCuenta.Ahorro, 100m);
            bool ok = !cuenta.Retirar(200m) && cuenta.ConsultarSaldo() == 100m;
            TestUtils.GuardarResultado(nameof(Retirar_SaldoInsuficiente_Falla), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }
    }
}
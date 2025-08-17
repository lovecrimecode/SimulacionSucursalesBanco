using SimulacionSucursalesBanco.src;
using Xunit;

public class CuentaTests
{
    [Fact]
    public void Depositar_SumaCorrectamente()
    {
        var cuenta = new Cuenta(1, "Test", TipoCuenta.Ahorro, 1000m);
        Assert.True(cuenta.Depositar(500m));
        Assert.Equal(1500m, cuenta.ConsultarSaldo());
    }

    [Fact]
    public void Retirar_DescuentaCorrectamente()
    {
        var cuenta = new Cuenta(2, "Test", TipoCuenta.Ahorro, 1000m);
        Assert.True(cuenta.Retirar(400m));
        Assert.Equal(600m, cuenta.ConsultarSaldo());
    }

    [Fact]
    public void Retirar_SaldoInsuficiente_Falla()
    {
        var cuenta = new Cuenta(3, "Test", TipoCuenta.Ahorro, 100m);
        Assert.False(cuenta.Retirar(200m));
        Assert.Equal(100m, cuenta.ConsultarSaldo());
    }
}
using SimulacionSucursalesBanco.src;
using System.Threading.Tasks;
using Xunit;

public class ConcurrenciaTests
{
    [Fact]
    public void DepositosConcurrentes_SaldoCorrecto()
    {
        var cuenta = new Cuenta(10, "Concurrente", TipoCuenta.Ahorro, 0m);
        int tareas = 100;
        decimal monto = 10m;

        Parallel.For(0, tareas, _ => cuenta.Depositar(monto));
        Assert.Equal(tareas * monto, cuenta.ConsultarSaldo());
    }
}
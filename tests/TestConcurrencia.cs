using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public class ConcurrenciaTests
    {
        [Fact]
        public void DepositosConcurrentes_SaldoCorrecto()
        {
            var cuenta = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var sucursal = new Sucursal(1, "Test");
            var clientes = new[]
            {
                new Cliente(1, cuenta, new Transaccion(TipoTransaccion.Deposito, cuenta, 100m), false, 1, PuntoAtencion.Ventanilla),
                new Cliente(2, cuenta, new Transaccion(TipoTransaccion.Deposito, cuenta, 200m), false, 1, PuntoAtencion.Ventanilla),
                new Cliente(3, cuenta, new Transaccion(TipoTransaccion.Deposito, cuenta, 300m), false, 1, PuntoAtencion.Ventanilla)
            };
            var tasks = new Task[clientes.Length];
            for (int i = 0; i < clientes.Length; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() => sucursal.ProcesarTransaccion(clientes[index]));
            }
            Task.WaitAll(tasks);
            decimal saldoEsperado = 1600m; // 1000 + 100 + 200 + 300
            bool exito = cuenta.ConsultarSaldo() == saldoEsperado;
            TestUtils.GuardarResultado(
                nameof(DepositosConcurrentes_SaldoCorrecto),
                exito,
                "Verifica que múltiples depósitos concurrentes suman correctamente al saldo",
                exito ? "" : $"Saldo esperado: {saldoEsperado}, obtenido: {cuenta.ConsultarSaldo()}"
            );
            Assert.True(exito);
        }

        [Fact]
        public void DetencionOrdenada_HilosFinalizan()
        {
            var simulador = new Simulador(1, 1, 1, EstrategiaAtencion.FIFO, 10, TimeSpan.FromSeconds(2), 4);
            simulador.Iniciar();
            Task.Delay(500).Wait();
            simulador.Detener();
            bool exito = true;
            string motivoFallo = "";
            foreach (var sucursal in simulador.GetSucursales())
            {
                if (!sucursal.ColasVacias())
                {
                    exito = false;
                    motivoFallo = "Colas no están vacías tras detención";
                    break;
                }
            }
            TestUtils.GuardarResultado(
                nameof(DetencionOrdenada_HilosFinalizan),
                exito,
                "Verifica que la detención con CancellationTokenSource finaliza hilos y vacía colas",
                motivoFallo
            );
            Assert.True(exito);
        }
    }
}
using System.Threading;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public class ColaTests
    {
        [Fact]
        public void EncolarDesencolar_ClienteCorrecto()
        {
            var sucursal = new Sucursal(1, "Sucursal-1");
            var cuenta = new Cuenta(1, "Test", TipoCuenta.Ahorro, 1000m);
            var transaccion = new Transaccion(TipoTransaccion.Deposito, cuenta, 500m);
            var cliente = new Cliente(1, cuenta, transaccion, false, 1, PuntoAtencion.Ventanilla);
            sucursal.RecibirCliente(cliente);
            bool ok = sucursal.TomarClienteVentanilla(EstrategiaAtencion.FIFO, CancellationToken.None) == cliente;
            TestUtils.GuardarResultado(nameof(EncolarDesencolar_ClienteCorrecto), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }

        [Fact]
        public void Prioridad_ClientePreferencialPrimero()
        {
            var sucursal = new Sucursal(1, "Sucursal-1");
            var cuenta1 = new Cuenta(1, "Normal", TipoCuenta.Ahorro, 1000m);
            var cuenta2 = new Cuenta(2, "Preferencial", TipoCuenta.Corriente, 2000m);
            var transaccion1 = new Transaccion(TipoTransaccion.Deposito, cuenta1, 500m);
            var transaccion2 = new Transaccion(TipoTransaccion.Retiro, cuenta2, 1000m);
            var clienteNormal = new Cliente(1, cuenta1, transaccion1, false, 1, PuntoAtencion.Ventanilla);
            var clientePreferencial = new Cliente(2, cuenta2, transaccion2, true, 1, PuntoAtencion.Ventanilla);
            sucursal.RecibirCliente(clienteNormal);
            sucursal.RecibirCliente(clientePreferencial);
            bool ok = sucursal.TomarClienteVentanilla(EstrategiaAtencion.Prioridad, CancellationToken.None) == clientePreferencial;
            TestUtils.GuardarResultado(nameof(Prioridad_ClientePreferencialPrimero), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }

        [Fact]
        public void Mixta_ClientePreferencialPrimero()
        {
            var sucursal = new Sucursal(1, "Sucursal-1");
            var cuenta1 = new Cuenta(1, "Normal", TipoCuenta.Ahorro, 1000m);
            var cuenta2 = new Cuenta(2, "Preferencial", TipoCuenta.Corriente, 2000m);
            var transaccion1 = new Transaccion(TipoTransaccion.Deposito, cuenta1, 500m);
            var transaccion2 = new Transaccion(TipoTransaccion.Retiro, cuenta2, 1000m);
            var clienteNormal = new Cliente(1, cuenta1, transaccion1, false, 1, PuntoAtencion.Ventanilla);
            var clientePreferencial = new Cliente(2, cuenta2, transaccion2, true, 1, PuntoAtencion.Ventanilla);

            bool ok = false;
            for (int i = 0; i < 5; i++)
            {
                sucursal.RecibirCliente(clienteNormal);
                sucursal.RecibirCliente(clientePreferencial);
                if (sucursal.TomarClienteVentanilla(EstrategiaAtencion.Mixta, CancellationToken.None) == clientePreferencial)
                {
                    ok = true;
                    break;
                }
                while (sucursal.TomarClienteVentanilla(EstrategiaAtencion.Mixta, CancellationToken.None) != null) { }
            }

            TestUtils.GuardarResultado(nameof(Mixta_ClientePreferencialPrimero), ok ? "OK" : "FALLÓ");
            Assert.True(ok);
        }
    }
}
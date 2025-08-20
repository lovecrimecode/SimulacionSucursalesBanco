using System.Threading;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public class ColaTests
    {
        [Fact]
        public void EncolarDesencolar_ClienteCorrecto()
        {
            var sucursal = new Sucursal(1, "Test");
            var cuenta = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var transaccion = new Transaccion(TipoTransaccion.Consulta, cuenta, 0);
            var cliente = new Cliente(1, cuenta, transaccion, false, 1, PuntoAtencion.Ventanilla);
            sucursal.RecibirCliente(cliente);
            var resultado = sucursal.TomarClienteVentanilla(EstrategiaAtencion.FIFO, CancellationToken.None);
            bool exito = resultado == cliente;
            TestUtils.GuardarResultado(
                nameof(EncolarDesencolar_ClienteCorrecto),
                exito,
                "Verifica que un cliente encolado en ventanilla (FIFO) es desencolado correctamente",
                exito ? "" : $"Se esperaba cliente ID {cliente.Id}, pero se obtuvo {(resultado?.Id ?? -1)}"
            );
            Assert.True(exito);
        }

        [Fact]
        public void Prioridad_ClientePreferencialPrimero()
        {
            var sucursal = new Sucursal(1, "Test");
            var cuenta1 = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var cuenta2 = new Cuenta(2, "Cliente2", TipoCuenta.Corriente, 2000m);
            var transaccion1 = new Transaccion(TipoTransaccion.Deposito, cuenta1, 500m);
            var transaccion2 = new Transaccion(TipoTransaccion.Retiro, cuenta2, 300m);
            var clienteNormal = new Cliente(1, cuenta1, transaccion1, false, 1, PuntoAtencion.Ventanilla);
            var clientePreferencial = new Cliente(2, cuenta2, transaccion2, true, 1, PuntoAtencion.Ventanilla);
            sucursal.RecibirCliente(clienteNormal);
            sucursal.RecibirCliente(clientePreferencial);
            var resultado = sucursal.TomarClienteVentanilla(EstrategiaAtencion.Prioridad, CancellationToken.None);
            bool exito = resultado == clientePreferencial;
            TestUtils.GuardarResultado(
                nameof(Prioridad_ClientePreferencialPrimero),
                exito,
                "Verifica que en estrategia Prioridad, un cliente preferencial es atendido antes que uno normal",
                exito ? "" : $"Se esperaba cliente preferencial ID {clientePreferencial.Id}, pero se obtuvo {(resultado?.Id ?? -1)}"
            );
            Assert.True(exito);
        }

        [Fact]
        public void Mixta_ClientePreferencialPrimero()
        {
            var sucursal = new Sucursal(1, "Test");
            var cuenta1 = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var cuenta2 = new Cuenta(2, "Cliente2", TipoCuenta.Corriente, 2000m);
            var transaccion1 = new Transaccion(TipoTransaccion.Deposito, cuenta1, 500m);
            var transaccion2 = new Transaccion(TipoTransaccion.Retiro, cuenta2, 300m);
            var clienteNormal = new Cliente(1, cuenta1, transaccion1, false, 1, PuntoAtencion.Ventanilla);
            var clientePreferencial = new Cliente(2, cuenta2, transaccion2, true, 1, PuntoAtencion.Ventanilla);
            bool exito = false;
            string motivoFallo = "";
            for (int i = 0; i < 5; i++)
            {
                sucursal.RecibirCliente(clienteNormal);
                sucursal.RecibirCliente(clientePreferencial);
                var resultado = sucursal.TomarClienteVentanilla(EstrategiaAtencion.Mixta, CancellationToken.None);
                if (resultado == clientePreferencial)
                {
                    exito = true;
                    break;
                }
                motivoFallo = $"Intento {i + 1}: Se esperaba cliente preferencial ID {clientePreferencial.Id}, pero se obtuvo {(resultado?.Id ?? -1)}";
                while (sucursal.TomarClienteVentanilla(EstrategiaAtencion.Mixta, CancellationToken.None) != null) { }
            }
            TestUtils.GuardarResultado(
                nameof(Mixta_ClientePreferencialPrimero),
                exito,
                "Verifica que en estrategia Mixta, un cliente preferencial tiene 75% de probabilidad de ser atendido primero",
                exito ? "" : motivoFallo
            );
            Assert.True(exito);
        }
    }
}
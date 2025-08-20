using System;
using Xunit;

namespace SimulacionSucursalesBanco
{
    public class CuentaTests
    {
        [Fact]
        public void Depositar_SumaCorrectamente()
        {
            var cuenta = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var transaccion = new Transaccion(TipoTransaccion.Deposito, cuenta, 500m);
            var sucursal = new Sucursal(1, "Test");
            var cliente = new Cliente(1, cuenta, transaccion, false, 1, PuntoAtencion.Ventanilla);
            bool exito = sucursal.ProcesarTransaccion(cliente);
            decimal saldoEsperado = 1500m;
            TestUtils.GuardarResultado(
                nameof(Depositar_SumaCorrectamente),
                exito && cuenta.ConsultarSaldo() == saldoEsperado,
                "Verifica que un depósito suma correctamente al saldo de la cuenta",
                exito ? (cuenta.ConsultarSaldo() == saldoEsperado ? "" : $"Saldo esperado: {saldoEsperado}, obtenido: {cuenta.ConsultarSaldo()}") : "Procesamiento falló"
            );
            Assert.True(exito && cuenta.ConsultarSaldo() == saldoEsperado);
        }

        [Fact]
        public void Retirar_DescuentaCorrectamente()
        {
            var cuenta = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var transaccion = new Transaccion(TipoTransaccion.Retiro, cuenta, 300m);
            var sucursal = new Sucursal(1, "Test");
            var cliente = new Cliente(1, cuenta, transaccion, false, 1, PuntoAtencion.Cajero);
            bool exito = sucursal.ProcesarTransaccion(cliente);
            decimal saldoEsperado = 700m;
            TestUtils.GuardarResultado(
                nameof(Retirar_DescuentaCorrectamente),
                exito && cuenta.ConsultarSaldo() == saldoEsperado,
                "Verifica que un retiro descuenta correctamente del saldo de la cuenta",
                exito ? (cuenta.ConsultarSaldo() == saldoEsperado ? "" : $"Saldo esperado: {saldoEsperado}, obtenido: {cuenta.ConsultarSaldo()}") : "Procesamiento falló"
            );
            Assert.True(exito && cuenta.ConsultarSaldo() == saldoEsperado);
        }

        [Fact]
        public void Retirar_SaldoInsuficiente_Falla()
        {
            var cuenta = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 100m);
            var transaccion = new Transaccion(TipoTransaccion.Retiro, cuenta, 500m);
            var sucursal = new Sucursal(1, "Test");
            var cliente = new Cliente(1, cuenta, transaccion, false, 1, PuntoAtencion.Cajero);
            bool exito = sucursal.ProcesarTransaccion(cliente);
            decimal saldoEsperado = 100m;
            TestUtils.GuardarResultado(
                nameof(Retirar_SaldoInsuficiente_Falla),
                !exito && cuenta.ConsultarSaldo() == saldoEsperado,
                "Verifica que un retiro con saldo insuficiente falla y no modifica el saldo",
                !exito ? (cuenta.ConsultarSaldo() == saldoEsperado ? "" : $"Saldo esperado: {saldoEsperado}, obtenido: {cuenta.ConsultarSaldo()}") : "Procesamiento no debería haber tenido éxito"
            );
            Assert.False(exito);
            Assert.Equal(saldoEsperado, cuenta.ConsultarSaldo());
        }

        [Fact]
        public void Consultar_SaldoCorrecto()
        {
            var cuenta = new Cuenta(1, "Cliente1", TipoCuenta.Ahorro, 1000m);
            var transaccion = new Transaccion(TipoTransaccion.Consulta, cuenta, 0m);
            var sucursal = new Sucursal(1, "Test");
            var cliente = new Cliente(1, cuenta, transaccion, false, 1, PuntoAtencion.Ventanilla);
            bool exito = sucursal.ProcesarTransaccion(cliente);
            decimal saldoEsperado = 1000m;
            TestUtils.GuardarResultado(
                nameof(Consultar_SaldoCorrecto),
                exito && cuenta.ConsultarSaldo() == saldoEsperado,
                "Verifica que una consulta de saldo no modifica el saldo y tiene éxito",
                exito ? (cuenta.ConsultarSaldo() == saldoEsperado ? "" : $"Saldo esperado: {saldoEsperado}, obtenido: {cuenta.ConsultarSaldo()}") : "Procesamiento falló"
            );
            Assert.True(exito && cuenta.ConsultarSaldo() == saldoEsperado);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulacionSucursalesBanco.src
{
    public enum TipoCuenta
    {
        Ahorro,
        Corriente
    }

    public class Cuenta
    {
        private readonly object _lockObj = new object(); // Para sincronización
        public int IdCuenta { get; private set; }
        public string Titular { get; private set; }
        public TipoCuenta Tipo { get; private set; } // NUEVO
        private decimal _balance;

        public Cuenta(int idCuenta, string titular, TipoCuenta tipo, decimal balanceInicial = 0)
        {
            IdCuenta = idCuenta;
            Titular = titular;
            Tipo = tipo;
            _balance = balanceInicial;
        }

        public bool Depositar(decimal monto)
        {
            if (monto <= 0) return false;
            lock (_lockObj)
            {
                _balance += monto;
                return true;
            }
        }

        public bool Retirar(decimal monto)
        {
            if (monto <= 0) return false;
            lock (_lockObj)
            {
                if (_balance >= monto)
                {
                    _balance -= monto;
                    return true;
                }
                return false; // Fondos insuficientes
            }
        }

        public decimal ConsultarSaldo()
        {
            lock (_lockObj)
            {
                return _balance;
            }
        }

        public override string ToString()
        {
            return $"Cuenta: {IdCuenta}, Titular: {Titular}, Tipo: {Tipo}, Saldo: {_balance:C}";
        }
    }
}

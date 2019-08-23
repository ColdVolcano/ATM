using System.IO;
using System.Linq;

namespace ATM
{
    public class Cuenta
    {
        private const string archivo_cuentas = @"Cuentas.txt";

        private readonly int cuenta;

        /// <summary>
        /// Saldo de la cuenta disponible para retiro (no incluye depositos).
        /// </summary>
        public float Saldo
        {
            get
            {
                if (float.IsNaN(saldo))
                    saldo = float.Parse(File.ReadAllLines(@"Cuentas.txt")[cuenta].Split(',')[2]);
                return saldo;
            }
            private set
            {
                saldo = value;
            }
        }

        private float saldo = float.NaN;

        private float saldoDepositos = 0;

        public Cuenta(int cuenta)
        {
            this.cuenta = cuenta;
        }

        /// <summary>
        /// Verifica que un numero de cuenta esté registrado
        /// </summary>
        /// <param name="cuenta">El numero de cuenta del usuario</param>
        /// <returns><see cref="true"/> si el usuario existe</returns>
        public static bool VerificarCuenta(string cuenta)
        {
            if (string.IsNullOrEmpty(cuenta) || cuenta.Length != 5)
                return false;

            return File.ReadAllLines(archivo_cuentas).Any(c => c.StartsWith(cuenta));
        }

        /// <summary>
        /// Verifica que un numero de cuenta esté registrado y accede mediante un pin
        /// </summary>
        /// <param name="cuenta">El numero de cuenta del usuario</param>
        /// <param name="pin">PIN del usuario para verificar cuenta</param>
        /// <returns>Una <see cref="Cuenta"/> para realizar operaciones bancarias</returns>
        public static Cuenta VerificarCuenta(string cuenta, string pin)
        {
            if (string.IsNullOrEmpty(cuenta) || cuenta.Length != 5 || string.IsNullOrEmpty(pin))
                return null;

            var cuentas = File.ReadAllLines(archivo_cuentas);

            for (int i = 0; i < cuentas.Length; i++)
            {
                if (cuentas[i].Substring(0, 5) == cuenta && cuentas[i].Split(',')[1] == pin)
                    return new Cuenta(i);
            }
            return null;
        }

        /// <summary>
        /// Retira <see="cantidad"> de la cuenta de usuario
        /// </summary>
        /// <param name="cantidad">Cantidad a ser retirada</param>
        /// <returns>Si el retiro fue exitoso</returns>
        public bool Retiro(float cantidad)
        {
            if (cantidad <= Saldo)
            {
                Saldo -= cantidad;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deposita <see="cantidad"> a la cuenta de usuario
        /// Notese que este saldo solo se verá reflejado despues de finalizada la sesión, conforme a la nota del punto 5 accion 3 del documento de especificaciones.
        /// </summary>
        /// <param name="cantidad">Cantidad a ser depositada</param>
        public void Deposito(float cantidad)
        {
            saldoDepositos += cantidad;
        }

        public void CerrarSesion()
        {
            var cuentas = File.ReadAllLines(archivo_cuentas);
            cuentas[cuenta] = cuentas[cuenta].Substring(0, 12) + (Saldo + saldoDepositos);
            File.WriteAllLines(archivo_cuentas, cuentas);
        }

        ~Cuenta()
        {
            CerrarSesion();
        }
    }
}

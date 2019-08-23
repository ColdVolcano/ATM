using System;
using System.Threading.Tasks;

namespace ATM
{
    public class Cajero
    {
        private static int billetes20Disponibles = 500;
        public static void IniciarServicio()
        {
            while (true)
            {
                escribirCentrado("Bienvenido, ingrese su numero de cuenta para continuar", 5);
                Console.CursorVisible = true;
                string intentoCuenta = obtenerNumero(5);
                Console.CursorVisible = false;
                if (Cuenta.VerificarCuenta(intentoCuenta))
                {
                    Cuenta usuario;
                    if ((usuario = autenticarUsuario(intentoCuenta)) != null)
                        iniciarSesion(usuario);
                    else
                        escribirCentrado("No se pudo autenticar, regresando a la pagina principal");
                }
                else
                {
                    escribirCentrado("La cuenta ingresada no existe, intente nuevamente");
                }

                Task.Delay(4000).Wait();
            }
        }

        private static void escribirCentrado(string mensaje, int espaciosEntrada = 0, int offsetVertical = 0)
        {
            Console.ResetColor();
            if (offsetVertical == 0)
                Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - mensaje.Length) / 2, Console.WindowHeight / 2 - 1 + offsetVertical);
            Console.WriteLine(mensaje);
            if (espaciosEntrada > 0)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition((Console.WindowWidth - espaciosEntrada) / 2, Console.WindowHeight / 2 + 1 + offsetVertical);
                for (int i = 0; i < espaciosEntrada; i++)
                    Console.Write(' ');
                Console.CursorLeft = (Console.WindowWidth - espaciosEntrada) / 2;
            }
        }

        private static void imprimirOpciones(string[] opciones)
        {
            for (int i = 0; i < opciones.Length; i++)
            {
                Console.SetCursorPosition(i % 2 == 0 ? 0 : Console.WindowWidth - (opciones[i] + (i + 1)).Length - 3, Console.WindowHeight / 2 + i / 2 + 1);
                if (i % 2 == 0)
                    Console.Write($"[" + (i + 1) + "]-");
                Console.Write(opciones[i]);
                if (i % 2 != 0)
                    Console.Write($"-[" + (i + 1) + "]");
            }
        }

        private static string obtenerNumero(int longitud, bool escribir = true, bool retardo = true)
        {
            string retorno = "";
            bool longitudMax = longitud < 0;
            longitud = Math.Abs(longitud);
            while (Console.KeyAvailable)
                Console.ReadKey(true);

            while (longitud > 0)
            {
                char tecla;
                if (char.IsDigit(tecla = Console.ReadKey(true).KeyChar))
                {
                    retorno += tecla;
                    if (escribir)
                        Console.Write(tecla);
                    longitud--;
                }

                if (longitudMax && tecla == '\r')
                    break;
            }

            if (retardo)
                Task.Delay(120).Wait();

            return retorno;
        }

        private static Cuenta autenticarUsuario(string usuario)
        {
            escribirCentrado("Introduzca su NIP a 5 digitos", 5);
            Console.CursorVisible = true;
            Console.CursorVisible = false;
            return Cuenta.VerificarCuenta(usuario, obtenerNumero(5));
        }

        private static string[] opcionesPrincipal =
        {
            "Consulta de saldo",
            "Retiro",
            "Deposito",
            "Salir del sistema"
        };

        private static void iniciarSesion(Cuenta cuenta)
        {
            bool write = true;
            while (true)
            {
                if (write)
                {
                    escribirCentrado("Bienvenido, elija una opción");
                    imprimirOpciones(opcionesPrincipal);
                }

                write = true;

                switch (obtenerNumero(1, false))
                {
                    case "1":
                        consultaCuenta(cuenta);
                        break;
                    case "2":
                        retiroCuenta(cuenta);
                        break;
                    case "3":
                        depositoCuenta(cuenta);
                        break;
                    case "4":
                        escribirCentrado("Gracias por su preferencia");
                        cuenta.CerrarSesion();
                        return;
                    default:
                        write = false;
                        break;
                }
            }
        }

        private static void consultaCuenta(Cuenta cuenta)
        {
            escribirCentrado("Su saldo es de " + cuenta.Saldo);
            escribirCentrado("Presione 0 para volver al menú principal", offsetVertical: 2);
            while (obtenerNumero(1, false) != "0") ;
        }

        private static string[] opcionesRetiro =
        {
            "$20",
            "$40",
            "$60",
            "$100",
            "$200",
            "Regresar",
        };

        private static void retiroCuenta(Cuenta cuenta)
        {
            bool write = true;
            while (true)
            {
                if (write)
                {
                    escribirCentrado("¿Qué cantidad desea retirar?");
                    imprimirOpciones(opcionesRetiro);
                }

                write = true;

                switch (obtenerNumero(1, false))
                {
                    case "1":
                        if (procesarRetiro(cuenta, 20))
                            return;
                        else
                            break;
                    case "2":
                        if (procesarRetiro(cuenta, 40))
                            return;
                        else
                            break;
                    case "3":
                        if (procesarRetiro(cuenta, 60))
                            return;
                        else
                            break;
                    case "4":
                        if (procesarRetiro(cuenta, 100))
                            return;
                        else
                            break;
                    case "5":
                        if (procesarRetiro(cuenta, 200))
                            return;
                        else
                            break;
                    case "6":
                        return;
                    default:
                        write = false;
                        break;
                }
            }
        }

        private static bool procesarRetiro(Cuenta cuenta, float cantidad)
        {
            if (cuenta.Saldo < cantidad)
            {
                escribirCentrado("No cuenta con el saldo suficiente para esta operación");
                escribirCentrado("Seleccione una cantidad menor", offsetVertical: 2);
                Task.Delay(4000).Wait();
                return false;
            }
            if (billetes20Disponibles * 20 < cantidad)
            {
                escribirCentrado("El cajero no cuenta con suficiente dinero para realizar la operación");
                escribirCentrado("Seleccione una cantidad menor", offsetVertical: 2);
                Task.Delay(4000).Wait();
                return false;
            }
            cuenta.Retiro(cantidad);
            billetes20Disponibles -= (int)cantidad / 20;

            escribirCentrado("Recoge tu efectivo");
            escribirCentrado("(Presiona cualquier numero)", offsetVertical: 1);

            escribirCentrado("/                             \\", offsetVertical: 3);
            escribirCentrado("/                               \\", offsetVertical: 4);
            escribirCentrado("-----------------------------------", offsetVertical: 5);

            obtenerNumero(1, false);

            return true;
        }

        private static void depositoCuenta(Cuenta cuenta)
        {
            escribirCentrado("Digite la cantidad a depositar");
            escribirCentrado("O digite 0 para regresar al menú", 10, 1);
            Console.CursorVisible = true;
            float cantidad = int.Parse(obtenerNumero(-10)) / 100f;
            Console.CursorVisible = false;

            DateTime inicioOperacion = DateTime.Now;

            escribirCentrado("Introduzca el dinero en la bandeja");
            escribirCentrado("(Presiona cualquier tecla)", offsetVertical: 1);
            escribirCentrado("-------------------------------", offsetVertical: 3);
            escribirCentrado("|                             |", offsetVertical: 4);
            escribirCentrado("-------------------------------", offsetVertical: 5);

            while ((DateTime.Now - inicioOperacion).Minutes < 2)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);

                    cuenta.Deposito(cantidad);
                    escribirCentrado("Operación exitosa");
                    escribirCentrado("El saldo depositado se verá reflejado en su siguiente sesión", offsetVertical: 1);
                    escribirCentrado("Regresando al menú principal", offsetVertical: 2);
                    Task.Delay(4000).Wait();
                    return;
                }
                Task.Delay(120).Wait();
            }

            escribirCentrado("Operación fallida, no se recibió el efectivo");
            escribirCentrado("Regresando al menú principal", offsetVertical: 2);
            Task.Delay(4000).Wait();
        }
    }
}

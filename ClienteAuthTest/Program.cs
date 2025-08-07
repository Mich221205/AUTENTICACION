using System;
using ClienteAuthTest.WSAUTENTICACION;

namespace ClienteAuthTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new AuthServiceClient();


            Console.WriteLine("=== PRUEBA LOGIN ===");
            Console.Write("Usuario: ");
            string user = Console.ReadLine();

            Console.Write("Contraseña: ");
            string pass = Console.ReadLine();


            var login = client.Login(user, pass);

            Console.WriteLine($"Resultado: {login.Resultado} | Mensaje: {login.Mensaje}");

            if (login.Resultado)
            {
                string tipo = login.TipoUsuario == 1 ? "Administrador" :
                              login.TipoUsuario == 2 ? "Cliente" : "Desconocido";

                Console.WriteLine($"Tipo detectado: {tipo} (valor = {login.TipoUsuario})");
            }


            Console.WriteLine("\n=== REGISTRAR NUEVO USUARIO ===");

            Usuario nuevo = new Usuario();

            Console.Write("identificación: ");
            nuevo.identificacion = LeerNoVacio();

            Console.Write("Nombre: ");
            nuevo.nombre = LeerNoVacio();

            Console.Write("Primer apellido: ");
            nuevo.apellido1 = LeerNoVacio();

            Console.Write("Segundo apellido: ");
            nuevo.apellido2 = LeerNoVacio();

            Console.Write("Correo: ");
            nuevo.correo = LeerNoVacio();

            Console.Write("Usuario: ");
            nuevo.usuario = LeerNoVacio();

            Console.Write("Contraseña (mínimo 14 caracteres): ");
            nuevo.contrasenna = LeerNoVacio();

            Console.Write("Tipo (1=Administrador, 2=Cliente): ");
            if (!int.TryParse(Console.ReadLine(), out int tipoNuevo) || (tipoNuevo != 1 && tipoNuevo != 2))
            {
                Console.WriteLine("Tipo inválido. Debe ser 1 o 2.");
                return;
            }
            nuevo.tipo = tipoNuevo;
            nuevo.estado = "activo";

            // Mostrar datos de confirmación
            Console.WriteLine("\n-- VERIFICACIÓN DE DATOS --");
            Console.WriteLine($"identificacion: {nuevo.identificacion}");
            Console.WriteLine($"Nombre: {nuevo.nombre}");
            Console.WriteLine($"Apellido1: {nuevo.apellido1}");
            Console.WriteLine($"Apellido2: {nuevo.apellido2}");
            Console.WriteLine($"Correo: {nuevo.correo}");
            Console.WriteLine($"Usuario: {nuevo.usuario}");
            Console.WriteLine($"Contraseña: {(string.IsNullOrWhiteSpace(nuevo.contrasenna) ? "VACÍA" : "************")}");
            Console.WriteLine($"Tipo: {nuevo.tipo}");
            Console.WriteLine($"Estado: {nuevo.estado}");

            var resultado = client.RegistrarUsuario(nuevo);
            Console.WriteLine($"\nRESULTADO → {resultado.Resultado} | MENSAJE → {resultado.Mensaje}");

            Console.WriteLine("\nPresione ENTER para salir...");
            Console.ReadLine();
        }

        static string LeerNoVacio()
        {
            string input;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("Campo obligatorio. Ingrese nuevamente: ");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input.Trim();
        }
    }
}

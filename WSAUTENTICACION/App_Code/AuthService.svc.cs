using MongoDB.Driver;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WSAutenticacion
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<Usuario> _coleccion;
        private readonly string _clave = "1234567890ABCDEF1234567890ABCDEF";

        public AuthService()
        {
            var cliente = new MongoClient("mongodb://localhost:27017");
            var db = cliente.GetDatabase("sistema_usuarios1");
            _coleccion = db.GetCollection<Usuario>("usuarios");
        }

        private string Encriptar(string texto)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_clave);
                aes.IV = new byte[16];
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] buffer = Encoding.UTF8.GetBytes(texto);
                return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }

        public AuthResponse Login(string usuarioCifrado, string contrasennaCifrada)
        {
            string usuarioEncriptado = Encriptar(usuarioCifrado);
            string contrasennaEncriptada = Encriptar(contrasennaCifrada);

            var usuario = _coleccion.Find(x =>
                x.usuario == usuarioEncriptado &&
                x.contrasenna == contrasennaEncriptada &&
                x.estado == "activo"
            ).FirstOrDefault();

            if (usuario == null)
            {
                return new AuthResponse { Resultado = false, Mensaje = "Usuario y/o contraseña incorrectos." };
            }

            return new AuthResponse
            {
                Resultado = true,
                Mensaje = "Exitoso",
                TipoUsuario = usuario.tipo  // 1=Empleado, 2=Cliente
            };
        }

        public AuthResponse RegistrarUsuario(Usuario nuevoUsuario)
        {
            // VALIDACIONES DETALLADAS
            if (string.IsNullOrWhiteSpace(nuevoUsuario.identificacion))
                return new AuthResponse { Resultado = false, Mensaje = "Identificación vacía." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.nombre))
                return new AuthResponse { Resultado = false, Mensaje = "Nombre vacío." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.apellido1))
                return new AuthResponse { Resultado = false, Mensaje = "Primer apellido vacío." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.apellido2))
                return new AuthResponse { Resultado = false, Mensaje = "Segundo apellido vacío." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.correo))
                return new AuthResponse { Resultado = false, Mensaje = "Correo vacío." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.usuario))
                return new AuthResponse { Resultado = false, Mensaje = "Usuario vacío." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.contrasenna))
                return new AuthResponse { Resultado = false, Mensaje = "Contraseña vacía." };

            if (string.IsNullOrWhiteSpace(nuevoUsuario.estado))
                return new AuthResponse { Resultado = false, Mensaje = "Estado vacío." };

            // VALIDAR caracteres válidos en nombre y apellidos (permitir tildes y espacios)
            var regexNombres = @"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$";
            if (!Regex.IsMatch(nuevoUsuario.nombre, regexNombres))
                return new AuthResponse { Resultado = false, Mensaje = "Nombre inválido." };
            if (!Regex.IsMatch(nuevoUsuario.apellido1, regexNombres))
                return new AuthResponse { Resultado = false, Mensaje = "Primer apellido inválido." };
            if (!Regex.IsMatch(nuevoUsuario.apellido2, regexNombres))
                return new AuthResponse { Resultado = false, Mensaje = "Segundo apellido inválido." };

            // VALIDAR correo
            if (!Regex.IsMatch(nuevoUsuario.correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return new AuthResponse { Resultado = false, Mensaje = "Correo inválido." };

            // VALIDAR tipo
            if (nuevoUsuario.tipo != 1 && nuevoUsuario.tipo != 2)
                return new AuthResponse { Resultado = false, Mensaje = "Tipo de usuario inválido." };

            // VALIDAR contraseña fuerte
            if (!Regex.IsMatch(nuevoUsuario.contrasenna, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{14,}$"))
                return new AuthResponse { Resultado = false, Mensaje = "Contraseña no cumple requisitos." };

            // VALIDAR estado
            if (nuevoUsuario.estado.ToLower() != "activo")
                return new AuthResponse { Resultado = false, Mensaje = "El estado inicial debe ser 'activo'." };

            // ENCRIPTAR datos
            string usuarioEncriptado = Encriptar(nuevoUsuario.usuario);
            string contrasennaEncriptada = Encriptar(nuevoUsuario.contrasenna);

            // Verificar si el usuario ya existe
            var existente = _coleccion.Find(u => u.usuario == usuarioEncriptado).FirstOrDefault();
            if (existente != null)
                return new AuthResponse { Resultado = false, Mensaje = "Usuario ya existe." };

            // Actualizar datos cifrados
            nuevoUsuario.usuario = usuarioEncriptado;
            nuevoUsuario.contrasenna = contrasennaEncriptada;

            _coleccion.InsertOne(nuevoUsuario);

            return new AuthResponse { Resultado = true, Mensaje = "Exitoso" };
        }
    }
}


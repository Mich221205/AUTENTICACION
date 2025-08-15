using System.Runtime.Serialization;

namespace WSAutenticacion
{
    [DataContract]
    public class AuthResponse
    {
        [DataMember] public bool Resultado { get; set; }
        [DataMember] public string Mensaje { get; set; }
        [DataMember] public int TipoUsuario { get; set; } // 1 = admin, 2 = cliente

        [DataMember] public string Nombre { get; set; }
        [DataMember] public string Apellido1 { get; set; }
    }
}
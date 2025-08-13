using System.Collections.Generic;
using System.ServiceModel;

namespace WSAutenticacion
{
    [ServiceContract]
    public interface IAuthService
    {
        [OperationContract]
        AuthResponse Login(string usuarioCifrado, string contrasennaCifrada);

        [OperationContract]
        AuthResponse RegistrarUsuario(Usuario nuevoUsuario);

        [OperationContract]
        List<Usuario> ObtenerAdministradores();

    }
}

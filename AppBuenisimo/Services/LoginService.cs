using AppBuenisimo.Models;
using AppBuenisimo.Repositories;
using AppBuenisimo.Utils;

namespace AppBuenisimo.Services
{
    public class LoginService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public LoginService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public tbUsuarios Autenticar(string dni, string contrasena)
        {
            var usuario = _usuarioRepository.ObtenerPorDni(dni);

            if (usuario == null)
            {
                return null;
            }

            if (usuario.contrasena.Length < 40)
            {
                if (usuario.contrasena == contrasena)
                {
                    usuario.contrasena = PasswordHasher.HashPassword(contrasena);
                    _usuarioRepository.GuardarCambios();
                    return usuario;
                }
                else
                {
                    return null;
                }
            }

            if (PasswordHasher.VerifyPassword(contrasena, usuario.contrasena))
            {
                return usuario;
            }

            return null;
        }
    }
}

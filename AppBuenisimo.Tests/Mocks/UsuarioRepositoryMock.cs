using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBuenisimo.Models;
using AppBuenisimo.Repositories;

namespace AppBuenisimo.Tests.Mocks
{
    public class UsuarioRepositoryMock : IUsuarioRepository
    {
        // Usamos una lista en memoria para simular la tabla de usuarios.
        private readonly List<tbUsuarios> _usuarios;
        public bool GuardarCambiosLlamado { get; private set; } = false;

        public UsuarioRepositoryMock(List<tbUsuarios> usuariosDePrueba)
        {
            _usuarios = usuariosDePrueba;
        }

        public tbUsuarios ObtenerPorDni(string dni)
        {
            // Busca en nuestra lista en memoria, no en la BD.
            return _usuarios.FirstOrDefault(u => u.dni == dni);
        }

        public void GuardarCambios()
        {
            // En la prueba, solo nos interesa saber si este método fue llamado.
            GuardarCambiosLlamado = true;
            // No hace nada más, porque no hay base de datos que guardar.
        }
    }
}

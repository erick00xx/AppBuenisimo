using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBuenisimo.Models;

namespace AppBuenisimo.Repositories
{
    public interface IUsuarioRepository
    {
        tbUsuarios ObtenerPorDni(string dni);
        void GuardarCambios();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBuenisimo.Models;

namespace AppBuenisimo.Repositories
{
    public interface IAsistenciaRepository
    {
        tbHorarios ObtenerHorarioVigente(int idUsuario, DateTime fecha);
        tbAsistencias ObtenerAsistenciaDelDia(int idUsuario, DateTime fecha);
        tbUsuarios ObtenerUsuarioPorId(int idUsuario);
        List<tbSucursales> ObtenerTodasLasSucursales();
        void AgregarAsistencia(tbAsistencias asistencia);
        void ActualizarAsistencia(tbAsistencias asistencia);
        void GuardarCambios();
    }
}

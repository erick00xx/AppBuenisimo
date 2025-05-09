using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Models
{
    public class ReporteAsistenciaEmpleadoViewModel
    {
        public int? IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public List<AsistenciaDetalle> Asistencias { get; set; } = new List<AsistenciaDetalle>();
        public List<SelectListItem> Usuarios { get; set; }
    }

    public class AsistenciaDetalle
    {
        public DateTime Fecha { get; set; }
        public string Sucursal { get; set; }
        public string Observacion { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Models
{
    public class AsistenciaFormViewModel
    {
        public clsAsistencia Asistencia { get; set; }
        public List<SelectListItem> Usuarios { get; set; }
        public List<SelectListItem> Sucursales { get; set; }
        public List<SelectListItem> Observaciones { get; set; }
    }
}
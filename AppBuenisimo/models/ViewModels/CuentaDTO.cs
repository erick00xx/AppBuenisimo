using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuenisimo.Models.ViewModels
{
    public class CuentaDTO
    {
        public int IdUsuario { get; set; }
        public string Rol { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contrasena { get; set; }
        public bool Activo { get; set; }
        public string Dni { get; set; }
    }
}
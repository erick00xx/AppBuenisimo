using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using AppBuenisimo.Utils;
using WebGrease.Css.Ast.Selectors;

namespace AppBuenisimo.Models
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contrasena { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public string Dni { get; set; }


        // Constructo pa registrar fecha
        public UsuarioDTO()
        {
            FechaRegistro = TimeProvider.Now;
        }


    }
}

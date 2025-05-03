using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebGrease.Css.Ast.Selectors;

namespace SistemaWebEficienciaOperativa.Models
{
    [Table("tbUsuarios")]
    public class clsUsuario
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
        public clsUsuario()
        {
            FechaRegistro = DateTime.Now;
        }


    }
}

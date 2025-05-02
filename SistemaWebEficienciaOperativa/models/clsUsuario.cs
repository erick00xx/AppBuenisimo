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

        // la tabla rol es padre de la tabla usuario
        public virtual clsRol Rol { get; set; }

        // el atributo idUsuario es foraneo en la tabla Horarios
        public virtual ICollection<clsHorario> Horarios { get; set; }

        public virtual ICollection<clsAsistencia> Asistencias { get; set; }


        // Constructo pa registrar fecha
        public clsUsuario()
        {
            FechaRegistro = DateTime.Now;
        }


    }
}

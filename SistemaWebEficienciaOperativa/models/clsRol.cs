using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models
{
    [Table("tbRoles")]
    public class clsRol
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; }

        public virtual ICollection<clsUsuario> Usuarios { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models
{
    [Table("tbSucursales")]
    public class clsSucursal
    {
        public int IdSucursal { get; set; }
        public int nombre { get; set; }


        public virtual ICollection<clsAsistencia> Asistencias { get; set; }
    }
}
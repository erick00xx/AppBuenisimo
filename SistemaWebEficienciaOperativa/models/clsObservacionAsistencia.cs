using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models
{
    [Table("tbObservacionesAsistencias")]
    public class clsObservacionAsistencia
    {
        public int IdObservacionAsistencia { get; set; }
        public string Descripcion { get; set; }

        public virtual ICollection<clsAsistencia> Asistencias { get; set; }
    }
}
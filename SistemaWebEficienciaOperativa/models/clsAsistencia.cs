using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models
{
    [Table("tbAsistencias")]
    public class clsAsistencia
    {
        public int IdAsistencia { get; set; }
        public int IdUsuario { get; set; }
        public int IdSucursal { get; set; }
        public int IdObservacionAsistencia { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }

        public virtual clsUsuario Usuario { get; set; }
        public virtual clsSucursal Sucursal { get; set; }
        public virtual clsObservacionAsistencia ObservacionAsistencia { get; set;}
    }
}
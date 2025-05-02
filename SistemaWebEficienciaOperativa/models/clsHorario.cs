using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models
{
    [Table("tbHorarios")]
    public class clsHorario
    {
        public int IdHorario { get; set; }
        public int IdUsuario { get; set; }
        public int DiaSemana { get; set; }
        public TimeSpan HoraEntradaEsperada { get; set; }
        public TimeSpan HoraSalidaEsperada { get; set; }
        
        [ForeignKey("idUsuario")]
        public virtual clsUsuario Usuario { get; set; }
    }
}
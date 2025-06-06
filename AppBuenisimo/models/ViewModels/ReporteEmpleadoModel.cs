using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuenisimo.Models.ViewModels
{
    public class ReporteEmpleadoModel
    {
        // Filtros
        public string Quincena { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }

        // Estadísticas
        public int TotalEmpleados { get; set; }
        public int TardanzasTotales { get; set; }
        public int FaltasTotales { get; set; }
        public decimal NominaTotal { get; set; }

        // Lista de empleados
        public List<EmpleadoDTO> Empleados { get; set; }
    }

    public class EmpleadoDTO
    {
        public int IdEmpleado { get; set; }
        public string NombreCompleto { get; set; }
        public string Puesto { get; set; }
        public int Tardanzas { get; set; }
        public int Faltas { get; set; }
        public decimal PagoQuincenal { get; set; }
        public string Estado { get; set; } = "Activo";
    }

    public class DetalleEmpleadoModalDTO
    {
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public decimal SalarioBaseQuincenal { get; set; }
        public List<TardanzaDTO> Tardanzas { get; set; } = new List<TardanzaDTO>();
        public List<FaltaDTO> Faltas { get; set; } = new List<FaltaDTO>();
        public List<PagoConceptoDTO> DesglosePago { get; set; } = new List<PagoConceptoDTO>();
        public decimal SueldoNeto { get; set; } // Agrega esta propiedad
    }

    public class TardanzaDTO
    {
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public string Tipo { get; set; }
        public string TipoTardanza { get; set; }

        public TimeSpan? DiferenciaTardanza { get; set; } // Ej: +30 minutos
    }

    public class FaltaDTO
    {
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; }
    }

    public class PagoConceptoDTO
    {
        public string Concepto { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } // Ingreso / Descuento
    }

}
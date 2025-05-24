using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models.ViewModels
{
    public class ReporteAbastecimientoModel
    {
        // Filtros
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string TipoMovimiento { get; set; }

        // Estadísticas
        public int ComprasEstaSemana { get; set; }
        public int VariacionCompras { get; set; }
        public int DesechosEstaSemana { get; set; }
        public int VariacionDesechos { get; set; }
        public int ComprasSemanaAnterior { get; set; }
        public int DesechosSemanaAnterior { get; set; }

        public decimal PromedioInsumosPorCompraSemana { get; set; }
        public decimal PorcentajeDesechosSobreCompras { get; set; }
        public decimal PorcentajePresupuesto { get; set; }


        // Detalles
        public List<CompraDTO> UltimasCompras { get; set; }
        public List<DesechoDTO> UltimosDesechos { get; set; }
    }

    public class CompraDTO
    {
        public int IdIngresoInsumo { get; set; }
        public DateTime FechaCompra { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Proveedor { get; set; }
        public decimal Total { get; set; }
    }

    public class DesechoDTO
    {
        public int IdDesechoInsumo { get; set; }
        public DateTime FechaDesecho { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Motivo { get; set; }
        public string Observaciones { get; set; }
    }

    public class InventarioEstadisticas
    {
        public int ComprasEstaSemana { get; set; }
        public int DesechosEstaSemana { get; set; }
        public decimal InversionTotal { get; set; }
    }

    public class DetalleProductoModel
    {
        public string Tipo { get; set; } // "compra" o "desecho"
        public object Producto { get; set; } // Puede ser CompraDTO o DesechoDTO
    }
}
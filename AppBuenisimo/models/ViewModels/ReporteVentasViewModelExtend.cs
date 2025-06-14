using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppBuenisimo.Models.ViewModels
{
    // ViewModels adicionales para las nuevas funcionalidades
    public class VentasPorMesViewModel
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
    }

    public class HorarioPicoSucursalViewModel
    {
        public int IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public int HoraPico { get; set; }
        public decimal VentasEnHoraPico { get; set; }
    }

    public class EstadisticasAvanzadasViewModel
    {
        public decimal VentaMinima { get; set; }
        public decimal VentaMaxima { get; set; }
        public decimal MedianaVenta { get; set; }
        public decimal DesviacionEstandar { get; set; }
        public int TotalTransacciones { get; set; }
    }

    // Actualizar el ViewModel principal
    public class DashboardReportesViewModelExtended : DashboardReportesViewModel
    {
        public List<VentasPorMesViewModel> VentasPorMes { get; set; }
        public List<HorarioPicoSucursalViewModel> HorariosPicoPorSucursal { get; set; }
        public EstadisticasAvanzadasViewModel EstadisticasAvanzadas { get; set; }
    }
}

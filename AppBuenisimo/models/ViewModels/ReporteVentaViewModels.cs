using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppBuenisimo.Models.ViewModels
{
    // ViewModel principal del dashboard
    public class DashboardReportesViewModel
    {
        public ResumenVentasViewModel ResumenVentas { get; set; }
        public List<VentasPorHoraViewModel> VentasPorHora { get; set; }
        public List<VentasPorDiaViewModel> VentasPorDia { get; set; }
        public List<TopProductosViewModel> TopProductos { get; set; }
        public List<TopCategoriasViewModel> TopCategorias { get; set; }
        public List<VentasPorSucursalViewModel> VentasPorSucursal { get; set; }
        public List<MetodosPagoViewModel> MetodosPago { get; set; }
        public List<AnalisisPreciosViewModel> AnalisisPrecios { get; set; }

        // Listas para filtros
        public List<SucursalViewModel> Sucursales { get; set; }
        public List<CategoriaViewModel> Categorias { get; set; }
        public List<MetodoPagoViewModel> MetodosPagoDisponibles { get; set; }

        // Filtros aplicados
        public FiltrosReporteViewModel Filtros { get; set; }
    }

    public class FiltrosReporteViewModel
    {
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime? FechaInicio { get; set; }

        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "Sucursal")]
        public int? IdSucursal { get; set; }

        [Display(Name = "Categoría")]
        public int? IdCategoria { get; set; }

        [Display(Name = "Método de Pago")]
        public int? IdMetodoPago { get; set; }
    }

    public class ResumenVentasViewModel
    {
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
        public decimal PromedioVenta { get; set; }
        public decimal CrecimientoVsAnterior { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public class VentasPorHoraViewModel
    {
        public int Hora { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
    }

    public class VentasPorDiaViewModel
    {
        public int DiaSemana { get; set; }
        public string NombreDia { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
    }

    public class TopProductosViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal PromedioVenta { get; set; }
    }

    public class TopCategoriasViewModel
    {
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
    }

    public class VentasPorSucursalViewModel
    {
        public int IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
        public decimal PromedioVenta { get; set; }
    }

    public class MetodosPagoViewModel
    {
        public int IdMetodoPago { get; set; }
        public string NombreMetodo { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class AnalisisPreciosViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public decimal PrecioPromedio { get; set; }
        public int CantidadVendida { get; set; }
    }

    // ViewModels auxiliares
    public class SucursalViewModel
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; }
    }

    public class CategoriaViewModel
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
    }

    public class MetodoPagoViewModel
    {
        public int IdMetodoPago { get; set; }
        public string Nombre { get; set; }
    }
}

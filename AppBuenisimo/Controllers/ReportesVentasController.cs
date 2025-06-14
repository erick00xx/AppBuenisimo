using AppBuenisimo.Models.ViewModels;
using System;
using System.Web.Mvc;

public class ReportesVentasController : Controller
{
    private readonly ReporteVentaService _reporteService = new ReporteVentaService();

    // GET: /ReportesVentas/ o /ReportesVentas/Index
    public ActionResult Index(FiltrosReporteViewModel filtros = null)
    {
        // Si no hay filtros, usar valores por defecto (último mes)
        if (filtros == null)
        {
            filtros = new FiltrosReporteViewModel
            {
                FechaInicio = DateTime.Now.AddDays(-30),
                FechaFin = DateTime.Now
            };
        }

        var model = new DashboardReportesViewModelExtended
        {
            Filtros = filtros,
            ResumenVentas = _reporteService.ObtenerResumenVentas(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            VentasPorHora = _reporteService.ObtenerVentasPorHora(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            VentasPorDia = _reporteService.ObtenerVentasPorDiaSemana(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            VentasPorMes = _reporteService.ObtenerVentasPorMes(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            TopProductos = _reporteService.ObtenerTopProductos(10, filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            TopCategorias = _reporteService.ObtenerTopCategorias(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            VentasPorSucursal = _reporteService.ObtenerVentasPorSucursal(filtros.FechaInicio, filtros.FechaFin),
            HorariosPicoPorSucursal = _reporteService.ObtenerHorariosPicoPorSucursal(filtros.FechaInicio, filtros.FechaFin),
            MetodosPago = _reporteService.ObtenerDistribucionMetodosPago(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            AnalisisPrecios = _reporteService.ObtenerAnalisisPrecios(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
            EstadisticasAvanzadas = _reporteService.ObtenerEstadisticasAvanzadas(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),

            // Listas para filtros
            Sucursales = _reporteService.ObtenerSucursales(),
            Categorias = _reporteService.ObtenerCategorias(),
            MetodosPagoDisponibles = _reporteService.ObtenerMetodosPago()
        };

        return View(model);
    }

    // AJAX: Actualizar dashboard con filtros - CORREGIDO
    [HttpPost]
    public JsonResult ActualizarDashboard(FiltrosReporteViewModel filtros)
    {
        try
        {
            var data = new
            {
                resumenVentas = _reporteService.ObtenerResumenVentas(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                ventasPorHora = _reporteService.ObtenerVentasPorHora(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                ventasPorDia = _reporteService.ObtenerVentasPorDiaSemana(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                ventasPorMes = _reporteService.ObtenerVentasPorMes(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                topProductos = _reporteService.ObtenerTopProductos(10, filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                topCategorias = _reporteService.ObtenerTopCategorias(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                ventasPorSucursal = _reporteService.ObtenerVentasPorSucursal(filtros.FechaInicio, filtros.FechaFin),
                horariosPicoPorSucursal = _reporteService.ObtenerHorariosPicoPorSucursal(filtros.FechaInicio, filtros.FechaFin),
                metodosPago = _reporteService.ObtenerDistribucionMetodosPago(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                analisisPrecios = _reporteService.ObtenerAnalisisPrecios(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal),
                estadisticasAvanzadas = _reporteService.ObtenerEstadisticasAvanzadas(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal)
            };

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error al actualizar el dashboard: " + ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    // AJAX: Obtener datos específicos para un gráfico - CORREGIDO
    [HttpPost]
    public JsonResult ObtenerDatosGrafico(string tipoGrafico, FiltrosReporteViewModel filtros)
    {
        try
        {
            object data = null;

            switch (tipoGrafico.ToLower())
            {
                case "ventasporhora":
                    data = _reporteService.ObtenerVentasPorHora(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal);
                    break;
                case "ventaspordia":
                    data = _reporteService.ObtenerVentasPorDiaSemana(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal);
                    break;
                case "ventaspormes":
                    data = _reporteService.ObtenerVentasPorMes(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal);
                    break;
                case "topproductos":
                    data = _reporteService.ObtenerTopProductos(10, filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal);
                    break;
                case "topcategorias":
                    data = _reporteService.ObtenerTopCategorias(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal);
                    break;
                case "metodospago":
                    data = _reporteService.ObtenerDistribucionMetodosPago(filtros.FechaInicio, filtros.FechaFin, filtros.IdSucursal);
                    break;
                default:
                    return Json(new { success = false, message = "Tipo de gráfico no válido" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error al obtener datos: " + ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}

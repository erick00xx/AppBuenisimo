using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AppBuenisimo.Controllers
{
    public class ReporteEmpleadoController : Controller
    {
        private readonly EmpleadoService _empleadoService;

        public ReporteEmpleadoController()
        {
            _empleadoService = new EmpleadoService();
        }

        public ActionResult Index()
        {
            var viewModel = new ReporteEmpleadoModel
            {
                Quincena = "Segunda quincena",
                Mes = "Mayo",
                Anio = 2025
            };

            viewModel = _empleadoService.GenerarReporte(viewModel.Quincena, viewModel.Mes, viewModel.Anio);
            return View(viewModel);
        }

        [HttpGet]
        public JsonResult FiltrarReporte(string quincena, string mes, int anio)
        {
            try
            {
                var reporte = _empleadoService.GenerarReporte(quincena, mes, anio);

                return Json(new
                {
                    TotalEmpleados = reporte.TotalEmpleados,
                    TardanzasTotales = reporte.TardanzasTotales,
                    FaltasTotales = reporte.FaltasTotales,
                    NominaTotal = reporte.NominaTotal.ToString("N2"),
                    Empleados = reporte.Empleados.Select(e => new
                    {
                        e.IdEmpleado,
                        e.NombreCompleto,
                        e.Puesto,
                        e.Tardanzas,
                        e.Faltas,
                        PagoQuincenal = e.PagoQuincenal.ToString("N2"),
                        e.Estado
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DetallePagoQuincenal(int idEmpleado, string quincena, string mes, int anio)
        {
            try
            {
                var fechaInicioQuincena = _empleadoService.GetFechaInicioQuincena(quincena, mes, anio);
                var fechaFinQuincena = _empleadoService.GetFechaFinQuincena(quincena, mes, anio);

                var detalle = _empleadoService.ObtenerDetalleEmpleado(idEmpleado, fechaInicioQuincena, fechaFinQuincena);

                if (detalle == null)
                {
                    TempData["Error"] = "Empleado no encontrado o sin registros en la quincena seleccionada.";
                    return RedirectToAction("Index");
                }

                return View(detalle);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar el detalle del pago: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}

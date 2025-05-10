using SistemaWebEficienciaOperativa.Models.ViewModels;
using SistemaWebEficienciaOperativa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Controllers
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
                Anio = 2024
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
                        e.PagoQuincenal,
                        e.Estado
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DetallePagoQuincenal(int idEmpleado, string quincena, string mes, int anio)
        {
            var fechaInicioQuincena = _empleadoService.GetFechaInicioQuincena(quincena, mes, anio);
            var fechaFinQuincena = _empleadoService.GetFechaFinQuincena(quincena, mes, anio);

            var detalle = _empleadoService.ObtenerDetalleEmpleado(idEmpleado, fechaInicioQuincena, fechaFinQuincena);

            if (detalle == null)
            {
                return View("Error");
            }

            return View(detalle);
        }


    }
}
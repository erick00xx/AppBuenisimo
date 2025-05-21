using SistemaWebEficienciaOperativa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using static SistemaWebEficienciaOperativa.Services.ReporteInventarioService;
using SistemaWebEficienciaOperativa.Utils;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class ReporteInventarioController : Controller
    {
        private readonly InventarioService _inventarioService;

        public ReporteInventarioController()
        {
            _inventarioService = new InventarioService();
        }

        public ActionResult Index()
        {
            var viewModel = new ReporteInventarioModel
            {
                FechaInicio = TimeProvider.Now.AddDays(-7),
                FechaFin = TimeProvider.Now,
                TipoMovimiento = "todos"
            };

            // Generar estadísticas
            _inventarioService.GenerarEstadisticas(
                TimeProvider.Now,
                out int comprasEstaSemana,
                out int desechosEstaSemana,
                out decimal inversionTotal);

            viewModel.ComprasEstaSemana = comprasEstaSemana;
            viewModel.DesechosEstaSemana = desechosEstaSemana;
            viewModel.InversionTotal = inversionTotal;
            viewModel.PorcentajePresupuesto = 15;

            // Obtener detalles
            viewModel.UltimasCompras = _inventarioService.ObtenerUltimasCompras(5);
            viewModel.UltimosDesechos = _inventarioService.ObtenerUltimosDesechos(5);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult DetalleProducto(int id, string tipo)
        {
            var model = new DetalleProductoModel
            {
                Tipo = tipo
            };

            if (tipo == "compra")
            {
                model.Producto = _inventarioService.ObtenerUltimaCompraPorId(id);
            }
            else if (tipo == "desecho")
            {
                model.Producto = _inventarioService.ObtenerUltimoDesechoPorId(id);
            }

            return View("~/Views/ReporteInventario/DetalleProducto.cshtml", model);
        }

        [HttpGet]
        public JsonResult FiltrarReporte(string fechaInicio, string fechaFin, string tipoMovimiento)
        {
            try
            {
                // Parsear fechas
                var format = "dd/MM/yyyy";
                var provider = new System.Globalization.CultureInfo("es-ES");

                if (!DateTime.TryParseExact(fechaInicio, format, provider, System.Globalization.DateTimeStyles.None, out DateTime inicio))
                {
                    return Json(new { error = "Fecha inicial inválida." }, JsonRequestBehavior.AllowGet);
                }

                if (!DateTime.TryParseExact(fechaFin, format, provider, System.Globalization.DateTimeStyles.None, out DateTime fin))
                {
                    return Json(new { error = "Fecha final inválida." }, JsonRequestBehavior.AllowGet);
                }

                List<CompraDTO> comprasFiltradas = new List<CompraDTO>();
                List<DesechoDTO> desechosFiltrados = new List<DesechoDTO>();

                if (tipoMovimiento == "todos" || tipoMovimiento == "compras")
                {
                    comprasFiltradas = _inventarioService.FiltrarComprasPorFecha(inicio, fin);
                }

                if (tipoMovimiento == "todos" || tipoMovimiento == "desechos")
                {
                    desechosFiltrados = _inventarioService.FiltrarDesechosPorFecha(inicio, fin);
                }

                var comprasJson = comprasFiltradas.Select(c => new
                {
                    c.IdIngresoInsumo,
                    Fecha = c.FechaCompra.ToString("yyyy-MM-dd"),
                    c.NombreInsumo,
                    Cantidad = $"{c.Cantidad} {c.Unidad}",
                    c.Proveedor,
                    Total = $"S/. {c.Total:N2}"
                });

                var desechosJson = desechosFiltrados.Select(d => new
                {
                    d.IdDesechoInsumo,
                    Fecha = d.FechaDesecho.ToString("yyyy-MM-dd"),
                    d.NombreInsumo,
                    Cantidad = $"{d.Cantidad} {d.Unidad}",
                    d.Motivo
                });

                return Json(new
                {
                    Compras = comprasJson,
                    Desechos = desechosJson,
                    ComprasEstaSemana = comprasFiltradas.Count,
                    DesechosEstaSemana = desechosFiltrados.Count,
                    InversionTotal = comprasFiltradas.Sum(x => x.Total).ToString("N2")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
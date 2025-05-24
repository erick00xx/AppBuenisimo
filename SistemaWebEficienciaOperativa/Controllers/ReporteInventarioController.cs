using SistemaWebEficienciaOperativa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using static SistemaWebEficienciaOperativa.Services.ReporteAbastecimientoService;
using SistemaWebEficienciaOperativa.Utils;
using System.Globalization;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class ReporteAbastecimientoController : Controller
    {
        private readonly InventarioService _inventarioService;

        public ReporteAbastecimientoController()
        {
            _inventarioService = new InventarioService();
        }

        public ActionResult Index()
        {
            var viewModel = new ReporteAbastecimientoModel
            {
                FechaInicio = TimeProvider.Now.AddDays(-7),
                FechaFin = TimeProvider.Now,
                TipoMovimiento = "todos"
            };

            // Solo una llamada a estadísticas (3 valores)
            _inventarioService.GenerarEstadisticas(
                TimeProvider.Now,
                out int comprasActual,
                out int desechosActual,
                out decimal porcentajeDesecho);

            viewModel.ComprasEstaSemana = comprasActual;
            viewModel.DesechosEstaSemana = desechosActual;
            viewModel.PorcentajeDesechosSobreCompras = porcentajeDesecho;

            viewModel.UltimasCompras = _inventarioService.ObtenerTodasLasCompras();
            viewModel.UltimosDesechos = _inventarioService.ObtenerTodosLosDesechos();

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult DetalleProducto(int id, string tipo)
        {
            var model = new DetalleProductoModel { Tipo = tipo };

            if (tipo == "compra")
                model.Producto = _inventarioService.ObtenerUltimaCompraPorId(id);
            else if (tipo == "desecho")
                model.Producto = _inventarioService.ObtenerUltimoDesechoPorId(id);

            return View("~/Views/ReporteAbastecimiento/DetalleProducto.cshtml", model);
        }

        [HttpGet]
        public JsonResult FiltrarReporte(string fechaInicio, string fechaFin, string tipoMovimiento)
        {
            try
            {
                var format = "yyyy-MM-dd";
                var provider = System.Globalization.CultureInfo.InvariantCulture;

                if (!DateTime.TryParseExact(fechaInicio, format, provider, DateTimeStyles.None, out DateTime inicio))
                    return Json(new { error = "Fecha inicial inválida." }, JsonRequestBehavior.AllowGet);

                if (!DateTime.TryParseExact(fechaFin, format, provider, DateTimeStyles.None, out DateTime fin))
                    return Json(new { error = "Fecha final inválida." }, JsonRequestBehavior.AllowGet);

                var finInclusive = fin.AddDays(1); // Para que incluya todo el día final

                var compras = (tipoMovimiento == "todos" || tipoMovimiento == "compras")
                    ? _inventarioService.FiltrarComprasPorFecha(inicio, finInclusive)
                    : new List<CompraDTO>();

                var desechos = (tipoMovimiento == "todos" || tipoMovimiento == "desechos")
                    ? _inventarioService.FiltrarDesechosPorFecha(inicio, finInclusive)
                    : new List<DesechoDTO>();


                // Calcular % desechos sobre compras
                decimal totalComprado = compras.Sum(c => c.Cantidad);
                decimal totalDesechado = desechos.Sum(d => d.Cantidad);
                decimal porcentaje = (totalComprado > 0) ? Math.Round((totalDesechado / totalComprado) * 100, 2) : 0;

                var comprasJson = compras.Select(c => new
                {
                    c.IdIngresoInsumo,
                    Fecha = c.FechaCompra.ToString("yyyy-MM-dd"),
                    c.NombreInsumo,
                    Cantidad = $"{c.Cantidad} {c.Unidad}",
                    c.Proveedor
                });

                var desechosJson = desechos.Select(d => new
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
                    ComprasEstaSemana = compras.Count,
                    DesechosEstaSemana = desechos.Count,
                    PorcentajeDesechosSobreCompras = porcentaje
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

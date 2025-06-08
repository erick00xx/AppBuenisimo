using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppBuenisimo.Models;
using AppBuenisimo.Services;

namespace AppBuenisimo.Controllers
{
    public class AbastecimientoController : Controller
    {
        private readonly DB_BUENISIMOEntities db = new DB_BUENISIMOEntities();
        private readonly ProductoServices _productoService = new ProductoServices();

        // GET: Abastecimiento 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteAbastecimiento()
        {
            return View();
        }

        public ActionResult RegistrarCompra()
        {
            ViewBag.Insumos = new SelectList(db.tbInsumos.OrderBy(i => i.nombre), "idInsumo", "nombre");
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "nombre");
            ViewBag.Proveedores = new SelectList(db.tbProveedores, "idProveedor", "nombreEmpresa");
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre");

            return View();
        }

        [HttpPost]
        public ActionResult RegistrarCompra(tbIngresosInsumos model)
        {
            if (ModelState.IsValid)
            {
                var existeRegistro = db.tbIngresosInsumos.Any(i =>
                    i.idInsumo == model.idInsumo &&
                    i.idUnidad == model.idUnidad &&
                    i.idProveedor == model.idProveedor &&
                    i.idSucursal == model.idSucursal &&
                    i.fechaCompra == model.fechaCompra &&
                    i.cantidad == model.cantidad);

                if (existeRegistro)
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos datos.");
                }
                else
                {
                    try
                    {
                        db.tbIngresosInsumos.Add(model);
                        db.SaveChanges();
                        return RedirectToAction("UltimosMovimientos");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                    }
                }
            }

            ViewBag.Insumos = new SelectList(db.tbInsumos.OrderBy(i => i.nombre), "idInsumo", "nombre", model.idInsumo);
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura", model.idUnidad);
            ViewBag.Proveedores = new SelectList(db.tbProveedores, "idProveedor", "nombreEmpresa", model.idProveedor);
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre", model.idSucursal);

            return View(model);
        }

        public ActionResult RegistrarDesecho()
        {
            ViewBag.Insumos = new SelectList(db.tbInsumos.OrderBy(i => i.nombre), "idInsumo", "nombre");
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura");
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre");
            ViewBag.Ingresos = new SelectList(db.tbIngresosInsumos, "idIngresoInsumo", "lote");

            return View();
        }

        [HttpPost]
        public ActionResult RegistrarDesecho(tbDesechosInsumos model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.tbDesechosInsumos.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("UltimosMovimientos");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar el desecho: " + ex.Message);
                }
            }

            ViewBag.Insumos = new SelectList(db.tbInsumos.OrderBy(i => i.nombre), "idInsumo", "nombre", model.idInsumo);
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura", model.idUnidad);
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre", model.idSucursal);
            ViewBag.Ingresos = new SelectList(db.tbIngresosInsumos, "idIngresoInsumo", "lote", model.idIngresoInsumo);

            return View(model);
        }

        public ActionResult UltimosMovimientos()
        {
            var compras = db.tbIngresosInsumos
                .OrderByDescending(i => i.fechaCompra)
                .Take(5)
                .Select(i => new CompraViewModel
                {
                    Id = i.idIngresoInsumo,
                    Insumo = i.tbInsumos.nombre,
                    Cantidad = i.cantidad,
                    Unidad = i.tbUnidades.abreviatura,
                    Proveedor = i.tbProveedores.nombreEmpresa,
                    Sucursal = i.tbSucursales.nombre,
                    Fecha = i.fechaCompra
                })
                .ToList();

            var desechos = db.tbDesechosInsumos
                .OrderByDescending(d => d.fechaDesecho)
                .Take(5)
                .Select(d => new DesechoViewModel
                {
                    Id = d.idDesechoInsumo,
                    Insumo = d.tbInsumos.nombre,
                    Cantidad = d.cantidad,
                    Unidad = d.tbUnidades.abreviatura,
                    Motivo = d.motivo,
                    Sucursal = d.tbSucursales.nombre,
                    Fecha = d.fechaDesecho
                })
                .ToList();

            var viewModel = new InventarioMovimientosViewModel
            {
                UltimasCompras = compras,
                UltimosDesechos = desechos
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult RegistrarComprasExcel()
        {
            ViewBag.MensajeFormato = "El archivo Excel debe contener las columnas: Insumo, Cantidad, Unidad, Proveedor, Sucursal, Fecha";
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarComprasExcel(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel != null && archivoExcel.ContentLength > 0)
            {
                var errores = _productoService.CargarProductosDesdeExcel(archivoExcel);

                if (errores.Count == 0)
                {
                    TempData["Exito"] = "Productos registrados exitosamente.";
                    return RedirectToAction("UltimosMovimientos");
                }
                else
                {
                    ViewBag.Errores = errores;
                }
            }
            else
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo válido.");
            }

            ViewBag.MensajeFormato = "El archivo Excel debe contener las columnas: Insumo, Cantidad, Unidad, Proveedor, Sucursal, Fecha";
            return View();
        }
    }
}

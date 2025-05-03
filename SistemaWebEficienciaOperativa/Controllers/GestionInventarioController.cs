using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionInventarioController : Controller
    {
        // GET: GestionInventario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteInventario()
        {
            return View();
        }

        public ActionResult RegistrarCompra()
        {
            ViewBag.Insumos = new SelectList(db.tbInsumos, "idInsumo", "nombre");
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura");
            ViewBag.Proveedores = new SelectList(db.tbProveedores, "idProveedor", "nombreEmpresa");
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre"); // asegúrate que tbSucursales está en EF

            return View();
        }
        [HttpPost]
        public ActionResult RegistrarCompra(tbIngresosInsumos model)
        {
            if (ModelState.IsValid)
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

            ViewBag.Insumos = new SelectList(db.tbInsumos, "idInsumo", "nombre", model.idInsumo);
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura", model.idUnidad);
            ViewBag.Proveedores = new SelectList(db.tbProveedores, "idProveedor", "nombreEmpresa", model.idProveedor);
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre", model.idSucursal);

            return View(model);
        }
        public ActionResult RegistrarDesecho()
        {
            ViewBag.Insumos = new SelectList(db.tbInsumos, "idInsumo", "nombre");
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura");
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre");
            ViewBag.Ingresos = new SelectList(db.tbIngresosInsumos, "idIngresoInsumo", "lote"); // puedes personalizar la vista

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

            ViewBag.Insumos = new SelectList(db.tbInsumos, "idInsumo", "nombre", model.idInsumo);
            ViewBag.Unidades = new SelectList(db.tbUnidades, "idUnidad", "abreviatura", model.idUnidad);
            ViewBag.Sucursales = new SelectList(db.tbSucursales, "idSucursal", "nombre", model.idSucursal);
            ViewBag.Ingresos = new SelectList(db.tbIngresosInsumos, "idIngresoInsumo", "lote", model.idIngresoInsumo);

            return View(model);
        }

        private readonly DB_BUENISIMOEntities db = new DB_BUENISIMOEntities();

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

    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Services;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionCartaController : Controller
    {
        ProductoServices productoServices = new ProductoServices();
        // GET: GestionCarta
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult CartaUsuario()
        {
            return View();
        }


        public ActionResult AgregarProducto()
        {
            var tiposProductos = productoServices.TiposProductos();
            var tiposMedidas = productoServices.TiposMedidas();
            ViewBag.TipoProducto = new SelectList(tiposProductos, "idTipoProducto", "nombre");
            ViewBag.TipoMedida = new SelectList(tiposMedidas, "idTipoMedida", "nombre");
            Debug.WriteLine("LLEGA AQUI");
            return View();
        }

        public JsonResult ObtenerCategoriasPorTipo(int idTipoProducto)
        {
            Debug.WriteLine("LLEGA ID: " + idTipoProducto);

            var categorias = productoServices.CategoriasPorTipo(idTipoProducto);
            var resultado = categorias.Select(c => new {
                idCategoria = c.idCategoria,
                nombre = c.nombre
            });

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AgregarProducto(tbProductos producto)
        {
            if (ModelState.IsValid)
            {
                Debug.Write("modelo valido");
                productoServices.GuardarProducto(producto);
                return RedirectToAction("Index");
            }
            else
            {
                Debug.Write("modelo invalido");
                // Manejar el error de validación
                return View("AgregarProducto", producto);
            }
        }
        public ActionResult OcultarProducto(int id)
        {
            if (!productoServices.OcultarProducto(id))
            {
                ViewBag.Error = "No se pudo ocultar el producto";
            }
            return RedirectToAction("ListarProductos");
        }
        public ActionResult MostrarProducto(int id)
        {
            Debug.WriteLine("Llega para mostrar producto");
            if (!productoServices.MostrarProducto(id))
            {
                Debug.WriteLine("no se pudo mostrar el producto");
            }
            return RedirectToAction("ListarProductos");
        }
        public ActionResult ListarProductos()
        {
            var productos = productoServices.ListarProductos();
            return View(productos);
        }


    }
}
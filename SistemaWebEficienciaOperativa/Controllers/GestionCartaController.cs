using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionCartaController : Controller
    {
        // GET: GestionCarta
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult CartaUsuario()
        {
            return View();
        }

        //public ActionResult AgregarProducto()
        //{
        //    using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
        //    {
        //        ViewBag.TipoProducto = new SelectList(db.tbCategorias.ToList(), "tipoProducto", "tipoProducto");
        //        ViewBag.Categorias = new SelectList(db.tbCategorias.ToList(), "idCategoria", "nombre");
        //        ViewBag.TiposMedidas = new SelectList(db.tbTiposMedidas.ToList(), "idTipoMedida", "nombre");
        //        db.Dispose();
        //        return View();
        //    }

        //}

        //Acción para cargar el formulario de agregar producto
        public ActionResult AgregarProducto()
        {
            using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
            {
                // Obtener las categorías y tipos de medida
                //ViewBag.TipoProducto = new SelectList(db.tbCategorias.Select(c => c.tipoProducto).Distinct(), "tipoProducto", "tipoProducto");
                ViewBag.TipoProducto = new SelectList(db.tbCategorias.ToList(), "tipoProducto", "tipoProducto");
                ViewBag.Categorias = new SelectList(new List<tbCategorias>(), "idCategoria", "nombre"); // Vacío inicialmente
                ViewBag.TiposMedidas = new SelectList(db.tbTiposMedidas.ToList(), "idTipoMedida", "nombre");
                db.Dispose();
                return View();
            }
        }

        // Acción para obtener categorías según el tipo de producto seleccionado
        public JsonResult ObtenerCategoriasPorTipo(string tipoProducto)
        {
            using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
            {
                var categorias = db.tbCategorias
                                   .Where(c => c.tipoProducto == tipoProducto)
                                   .ToList();

                // Devolver las categorías filtradas como JSON
                return Json(categorias, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
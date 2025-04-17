using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proyecto1_v1.Controllers
{
    public class GestionCartaController : Controller
    {
        // GET: GestionCarta
        public ActionResult Index()//interfaz 
        {
            return View();
        }
        public ActionResult CartaUsuario()
        {
            return View();
        }
        public ActionResult AgregarProducto()
        {
            return View();
        }
        public ActionResult ListarCarta()
        {
            return View();
        }
        public ActionResult InhabilitarProducto()
        {
            return View();
        }
        public ActionResult EditarProdcutos()
        {
            return View();
        }
    }
}
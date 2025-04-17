using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proyecto1_v1.Controllers
{
    public class GestionInventarioController : Controller
    {
        // GET: GestionInventario
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RegistrarInsumos()
        {
            return View();
        }
        public ActionResult RegistrarDescarte()
        {
            return View();
        }
        public ActionResult ListarInventario()
        {
            return View();
        }
        public ActionResult FiltrarInventario()
        {
            return View();
        }
    }
}
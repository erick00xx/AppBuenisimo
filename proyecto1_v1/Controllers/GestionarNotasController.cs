using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proyecto1_v1.Controllers
{
    public class GestionarNotasController : Controller
    {
        // GET: GestionarNotas
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AgregarNotas()
        {
            return View();
        }
        public ActionResult ListarNotas()
        {
            return View();
        }
    }
}
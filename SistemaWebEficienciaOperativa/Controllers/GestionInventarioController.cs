using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
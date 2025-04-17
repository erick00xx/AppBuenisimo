using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proyecto1_v1.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteInventario()
        {
            return View();
        }

        public ActionResult ReporteEmpleado()
        {
            return View();
        }

        public ActionResult ReporteVentas()
        {
            return View();
        }
        public ActionResult ImprimirReporte()
        {
            return View();
        }
    }
}
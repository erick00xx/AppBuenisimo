using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionVentasController : Controller
    {
        // GET: GestionVentas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteVentas()
        {
            return View();
        }
    }
}
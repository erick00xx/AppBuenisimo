using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionEmpleadosController : Controller
    {
        // GET: GestionEmpleados
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReporteEmpleados()
        {
            return View();
        }
    }
}
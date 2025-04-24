using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class AutenticacionController : Controller
    {
        // GET: Autenticacion
        public ActionResult Index()
        {
            return View();
        }

        // POST: Autenticacion
        [HttpPost]
        public ActionResult Login(string usuario, string contrasena)
        {
            // Lógica estática de autenticación
            if (usuario == "admin" && contrasena == "1234")
            {
                // Almacenar en sesión
                Session["Usuario"] = usuario;

                // Redirigir a Home
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Mensaje de error
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View();
            }
        }


        // Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Elimina la sesión
            return RedirectToAction("Index");
        }
    }
}
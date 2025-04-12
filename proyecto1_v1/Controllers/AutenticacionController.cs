using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proyecto1_v1.Controllers
{
    public class AutenticacionController : Controller
    {
        // GET: Autenticacion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string usuario, string contraseña)
        {
            if (usuario == "admin@a" && contraseña == "1234")
            {
                Session["Usuario"] = usuario; // Guarda el usuario en la sesión
                return RedirectToAction("Index", "Home"); // Redirige a Home si las credenciales son correctas
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View("Index");
        }

        public ActionResult Logout(string usuario, string contraseña)
        {
            Session.Clear(); // Borra la sesión
            Session.Abandon(); // Cierra completamente la sesión
            return RedirectToAction("Index", "Autenticacion"); // Redirige al login
        }


    }
}
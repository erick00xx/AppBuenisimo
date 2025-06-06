using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppBuenisimo.Models;
using AppBuenisimo.Services;

namespace AppBuenisimo.Controllers
{
    public class AutenticacionController : Controller
    {

        private readonly LoginService _loginService;

        public AutenticacionController()
        {
            _loginService = new LoginService();
        }
        // GET: Autenticacion
        public ActionResult Index()
        {
            return View();
        }

        // POST: Autenticacion
        [HttpPost]
        public ActionResult Login(tbUsuarios model)
        {
            Debug.WriteLine("Intentando autenticar usuario: " + model.dni);
            try
            {
                var usuario = _loginService.Autenticar(model.dni, model.contrasena);

                if (usuario == null)
                {
                    Debug.WriteLine("Usuario o contraseña incorrectos para DNI: " + model.dni);
                    ViewBag.Error = "Usuario o contraseña incorrectos.";
                    return View("Index");
                }
                Debug.WriteLine("Usuario autenticado: " + usuario.dni);
                Session["usuario"] = usuario.idUsuario;
                Session["idUsuario"] = usuario.idUsuario;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al autenticar: " + ex.Message);
                ModelState.AddModelError("", "Error de Sistema: " + ex.Message);
                ViewBag.Message = "Inténtalo nuevamente.";
                return View("Index");
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
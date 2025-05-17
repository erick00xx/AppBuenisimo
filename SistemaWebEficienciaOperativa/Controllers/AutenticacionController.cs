using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Services;

namespace SistemaWebEficienciaOperativa.Controllers
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
            try
            {
                var usuario = _loginService.Autenticar(model.dni, model.contrasena);

                if (usuario == null)
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos.";
                    return View("Index");
                }

                Session["usuario"] = usuario.idUsuario;
                Session["idUsuario"] = usuario.idUsuario;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
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
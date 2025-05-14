using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;

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
        public ActionResult Login(tbUsuarios model)
        {
            try
            {
                using(DB_BUENISIMOEntities context = new DB_BUENISIMOEntities())
                {
                    var usuario = context.tbUsuarios
                        .Where(u => u.dni == model.dni && u.contrasena == model.contrasena)
                        .FirstOrDefault();
                    if(usuario == null)
                    {
                        ViewBag.Error = "Usuario o contraseña incorrectos.";
                        return View("Index");
                    }
                    Session["usuario"] = usuario.idUsuario;
                    Session["idUsuario"] = usuario.idUsuario;
                    return RedirectToAction("Index", "Home");
                }

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Error de Sistema: " + ex.Message);
                string message = "Intentalo nuevamente";
                ViewBag.Message = message;
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
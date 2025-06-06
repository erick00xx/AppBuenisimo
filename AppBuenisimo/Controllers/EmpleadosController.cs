using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Services;
using AppBuenisimo.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppBuenisimo.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly HorarioService _horarioService;

        public EmpleadosController()
        {
            _horarioService = new HorarioService();
        }
        public ActionResult ReporteEmpleados()
        {
            return View();
        }

        

        // GET: Empleados
        public ActionResult Index()
        {
            var usuarios = _horarioService.ObtenerTodosLosUsuariosParaGestion();
            return View(usuarios);
        }


        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(tbUsuarios model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
                    {
                        model.contrasena = PasswordHasher.HashPassword(model.contrasena);
                        db.tbUsuarios.Add(model);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar el usuario: " + ex.Message);
                    string message = "Intentalo nuevamente";
                    ViewBag.Message = message;
                }
            }
            return View(model);
        }

        public ActionResult Editar(int idUsuario)
        {
                using(DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
                {
                    var data = db.tbUsuarios.Where(u => u.idUsuario == idUsuario).SingleOrDefault();
                    return View(data);
                }
        }

        [HttpPost]
        public ActionResult Editar(int idUsuario, tbUsuarios model)
        {
            if (!ModelState.IsValid)
            {
                // ❌ Si el modelo no es válido, devuelves el mismo modelo para que se conserven los datos ingresados
                return View(model);
            }
            try
            {
                using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
                {
                    var data = db.tbUsuarios.FirstOrDefault(x => x.idUsuario == idUsuario);

                    if (data == null)
                    {
                        // No se encontró el usuario, devolver a la vista con un mensaje de error
                        ModelState.AddModelError("", "El usuario no fue encontrado.");
                        return View(model);
                    }

                    data.idRol = model.idRol;
                    data.nombre = model.nombre;
                    data.apellido = model.apellido;
                    data.correoElectronico = model.correoElectronico;
                    data.contrasena = model.contrasena;
                    data.fechaRegistro = TimeProvider.Now;
                    data.activo = model.activo;
                    data.dni = model.dni;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }catch(Exception ex)
            {
                // Algo salió mal: te quedas en la vista y devuelves el modelo con un mensaje de error
                ModelState.AddModelError("", "Error al actualizar el usuario: " + ex.Message);
                return View(model);
            }
            
        }
        
    }
}
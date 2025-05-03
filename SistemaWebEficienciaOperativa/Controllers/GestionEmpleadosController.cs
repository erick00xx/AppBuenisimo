using SistemaWebEficienciaOperativa.Models;
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
        public ActionResult Listar()
        {
            try
            {
                using (var db = new DB_BUENISIMOEntities())
                {
                    var data = db.tbUsuarios.ToList();
                    return View(data);
                }
            }
            catch
            {
                return View("Listar");
            }
            
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
                //using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
                //{
                //    // Crear una nueva instancia de tbUsuario que es el que se mapeará a la base de datos
                //    var nuevoUsuario = new tbUsuarios
                //    {
                //        nombre = objUsuario.Nombre,
                //        apellido = objUsuario.Apellido,
                //        correoElectronico = objUsuario.CorreoElectronico,
                //        contrasena = objUsuario.Contrasena,
                //        dni = objUsuario.Dni,
                //        idRol = objUsuario.IdRol,
                //        activo = objUsuario.Activo,
                //        fechaRegistro = DateTime.Now 
                //    };

                //    db.tbUsuarios.Add(nuevoUsuario);

                //    db.SaveChanges();
                //}

                //return RedirectToAction("Listar");

                try
                {
                    using (DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
                    {
                        
                        db.tbUsuarios.Add(model);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Listar");
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
            using(DB_BUENISIMOEntities db = new DB_BUENISIMOEntities())
            {
                var data = db.tbUsuarios.FirstOrDefault(x => x.idUsuario == idUsuario);

                if (data != null)
                {
                    data.idRol = model.idRol;
                    data.nombre = model.nombre;
                    data.apellido = model.apellido;
                    data.correoElectronico = model.correoElectronico;
                    data.contrasena = model.contrasena;
                    data.fechaRegistro = model.fechaRegistro;
                    data.activo = model.activo;
                    data.dni = model.dni;

                    db.SaveChanges();
                    return View("Listar");
                }
                else return View();
            }
        }
        
    }
}
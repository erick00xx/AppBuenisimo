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
            
            using (var db = new DB_BUENISIMOEntities())
            {
                var data = db.tbUsuarios.ToList();
                return View(data);
            }
        }
        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(clsUsuario objUsuario)
        {
            if (ModelState.IsValid)
            {
                // Crear una nueva instancia del contexto de la base de datos
                using (var db = new DB_BUENISIMOEntities())
                {
                    // Crear una nueva instancia de tbUsuario que es el que se mapeará a la base de datos
                    var nuevoUsuario = new tbUsuario
                    {
                        nombre = objUsuario.Nombre,
                        apellido = objUsuario.Apellido,
                        correoElectronico = objUsuario.CorreoElectronico,
                        contrasena = objUsuario.Contrasena,
                        dni = objUsuario.Dni,
                        idRol = objUsuario.IdRol,
                        activo = objUsuario.Activo,
                        fechaRegistro = DateTime.Now  // Asignamos la fecha actual
                    };

                    // Agregar el nuevo usuario a la base de datos
                    db.tbUsuarios.Add(nuevoUsuario);

                    // Guardar los cambios en la base de datos
                    db.SaveChanges();
                }

                // Redirigir a la acción Listar después de guardar
                return RedirectToAction("Listar");
            }

            // Si el modelo no es válido, regresamos a la vista con los errores de validación
            return View(objUsuario);
        }
    }
}
using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class AsistenciaController : Controller
    {
        
        public ActionResult Registrar()
        {
            CargarUltimaAsistencia();
            return View(new clsUsuario());
        }

        [HttpPost]
        public ActionResult Registrar(clsUsuario objUsuario, string accion)
        {
            CargarUltimaAsistencia(); // Esto se llama siempre

            if (string.IsNullOrWhiteSpace(objUsuario.Dni))
            {
                ModelState.AddModelError("Dni", "El DNI es obligatorio.");
                return View(objUsuario);
            }

            using (var db = new DB_BUENISIMOEntities())
            {
                var usuario = db.tbUsuarios.FirstOrDefault(u => u.dni == objUsuario.Dni);
                if (usuario == null)
                {
                    ModelState.AddModelError("Dni", "Usuario no encontrado o inactivo.");
                    return View(objUsuario);
                }

                if (accion == "Ingresar")
                {
                    db.tbAsistencias.Add(new tbAsistencias
                    {
                        idUsuario = usuario.idUsuario,
                        fecha = DateTime.Today,
                        horaEntrada = DateTime.Now,
                        idSucursal = 1,
                        idObservacionAsistencia = 1
                    });
                    db.SaveChanges();
                    TempData["Mensaje"] = "Ingreso registrado.";
                }
                else if (accion == "Salir")
                {
                    var ultima = db.tbAsistencias
                        .Where(a => a.idUsuario == usuario.idUsuario && a.horaSalida == null)
                        .OrderByDescending(a => a.idAsistencia)
                        .FirstOrDefault();

                    if (ultima == null)
                    {
                        ModelState.AddModelError("", "No hay un ingreso previo para marcar salida.");
                        return View(objUsuario);
                    }

                    ultima.horaSalida = DateTime.Now;
                    db.SaveChanges();
                    TempData["Mensaje"] = "Salida registrada.";
                }
            }

            return RedirectToAction("Registrar");
        }

        private void CargarUltimaAsistencia()
        {
            using (var db = new DB_BUENISIMOEntities())
            {
                var ult = db.tbAsistencias
                    .OrderByDescending(a => a.idAsistencia)
                    .FirstOrDefault();

                if (ult != null)
                {
                    var user = db.tbUsuarios.Find(ult.idUsuario);
                    if (user != null)
                    {
                        ViewBag.UltimoUsuario = user.nombre + " " + user.apellido;
                        ViewBag.UltimoHora = (ult.horaSalida ?? ult.horaEntrada)?.ToString("hh:mm tt");
                        ViewBag.UltimoEstado = ult.horaSalida == null ? "INGRESANTE" : "SALIENTE";
                    }
                }
            }
        }

        public ActionResult ReporteAsistencias(int? idUsuario, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using (var db = new DB_BUENISIMOEntities())
            {
                // Lista de usuarios activos para el dropdown
                var usuarios = db.tbUsuarios
                    .Where(u => u.activo == true)
                    .Select(u => new SelectListItem
                    {
                        Value = u.idUsuario.ToString(),
                        Text = u.nombre + " " + u.apellido
                    }).ToList();

                // Armamos la consulta base
                var asistenciasQuery = db.tbAsistencias.AsQueryable();

                if (idUsuario.HasValue)
                    asistenciasQuery = asistenciasQuery.Where(a => a.idUsuario == idUsuario.Value);

                if (fechaDesde.HasValue)
                    asistenciasQuery = asistenciasQuery.Where(a => a.fecha >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    asistenciasQuery = asistenciasQuery.Where(a => a.fecha <= fechaHasta.Value);

                // Ejecutamos la consulta primero con ToList(), luego manipulamos los datos en memoria
                var asistenciasDB = asistenciasQuery
                    .OrderByDescending(a => a.fecha)
                    .ToList();

                // Proyección ya en memoria, ahora sí podemos usar TimeOfDay
                var asistencias = asistenciasDB
                    .Select(a => new AsistenciaDetalle
                    {
                        Fecha = a.fecha,
                        Sucursal = a.tbSucursales?.nombre,
                        Observacion = a.tbObservacionesAsistencias?.descripcion,
                        HoraEntrada = a.horaEntrada?.TimeOfDay,
                        HoraSalida = a.horaSalida?.TimeOfDay
                    }).ToList();

                // Creamos el ViewModel
                var viewModel = new ReporteAsistenciaEmpleadoViewModel
                {
                    IdUsuario = idUsuario,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    Asistencias = asistencias,
                    Usuarios = usuarios
                };

                return View(viewModel);
            }
        }
    }
}

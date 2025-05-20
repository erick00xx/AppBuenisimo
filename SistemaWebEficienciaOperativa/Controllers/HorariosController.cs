using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Services;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using SistemaWebEficienciaOperativa.Models;
using System.Diagnostics; // Para tbHorario si lo usas directamente

namespace SistemaWebEficienciaOperativa.Controllers
{
    // [Authorize(Roles="Admin")] // O el rol que corresponda para acceder a esta gestión
    public class HorariosController : Controller
    {
        private readonly HorarioService _horarioService;

        public HorariosController()
        {
            _horarioService = new HorarioService();
        }

        // GET: GestionEmpleados
        public ActionResult Index()
        {
            var usuarios = _horarioService.ObtenerTodosLosUsuariosParaGestion();
            return View(usuarios);
        }

        // GET: GestionEmpleados/Horarios/5
        public ActionResult Horarios(int idUsuario)
        {
            var usuario = _horarioService.ObtenerUsuarioPorId(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound("Usuario no encontrado.");
            }

            var horarios = _horarioService.ObtenerHorariosPorUsuario(idUsuario);
            var viewModel = new UsuarioHorariosViewModel
            {
                IdUsuario = usuario.idUsuario,
                NombreUsuario = $"{usuario.nombre} {usuario.apellido}",
                HorariosAsignados = horarios,
                NuevoHorario = new HorarioFormViewModel { IdUsuario = idUsuario } // Pre-popular IdUsuario
            };

            return View(viewModel);
        }

        // POST: GestionEmpleados/AgregarHorario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarHorario(UsuarioHorariosViewModel viewModelFromForm)
        {
            // Ahora los datos del formulario estarán en viewModelFromForm.NuevoHorario
            HorarioFormViewModel model = viewModelFromForm.NuevoHorario;

            // Verifica si model es null (si no se envió ningún dato para NuevoHorario)
            if (model == null)
            {
                // Manejar este caso improbable, quizás redirigir con un error
                TempData["ErrorMessage"] = "Error: No se recibieron datos del formulario.";
                return RedirectToAction("Index"); // O a la vista anterior si tienes el idUsuario
            }

            // Ahora model.IdUsuario debería tener el valor del campo oculto
            // Pon un breakpoint aquí para verificar model.IdUsuario

            if (ModelState.IsValid) // Este ModelState se aplicará a las propiedades de NuevoHorario
            {
                if (model.HoraSalida <= model.HoraEntrada)
                {
                    ModelState.AddModelError("NuevoHorario.HoraSalida", "La hora de salida debe ser posterior a la hora de entrada.");
                }
                if (model.FechaFinVigencia.HasValue && model.FechaFinVigencia < model.FechaInicioVigencia)
                {
                    ModelState.AddModelError("NuevoHorario.FechaFinVigencia", "La fecha fin de vigencia no puede ser anterior a la fecha de inicio.");
                }

                if (ModelState.IsValid)
                {
                    if (_horarioService.AgregarHorario(model)) // model es NuevoHorario
                    {
                        TempData["SuccessMessage"] = "Horario agregado exitosamente.";
                        return RedirectToAction("Horarios", new { idUsuario = model.IdUsuario });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al agregar el horario.";
                    }
                }
            }

            // Si llegamos aquí, ModelState no es válido o el guardado falló.
            // model.IdUsuario ya debería ser correcto.
            if (model.IdUsuario <= 0)
            {
                Debug.WriteLine($"Error crítico: IdUsuario en NuevoHorario es {model.IdUsuario}.");
                TempData["ErrorMessage"] = "Error crítico: Información del usuario perdida. Intente nuevamente.";
                // Es importante saber POR QUÉ idUsuario es 0 aquí.
                // Si el campo oculto estaba bien, esto no debería pasar si el binding funciona.
                return RedirectToAction("Index");
            }

            var usuario = _horarioService.ObtenerUsuarioPorId(model.IdUsuario);
            if (usuario == null)
            {
                Debug.WriteLine($"Error: Usuario no encontrado con IdUsuario {model.IdUsuario}.");
                TempData["ErrorMessage"] = "Error: Usuario no encontrado al intentar recargar el formulario.";
                return RedirectToAction("Index");
            }

            var horarios = _horarioService.ObtenerHorariosPorUsuario(model.IdUsuario);

            // Reconstruir el ViewModel para la vista, usando el 'model' (que es NuevoHorario)
            // que ya tiene los valores del formulario y los errores de validación.
            var viewModelParaVista = new UsuarioHorariosViewModel
            {
                IdUsuario = usuario.idUsuario, // El IdUsuario del usuario real
                NombreUsuario = $"{usuario.nombre} {usuario.apellido}",
                HorariosAsignados = horarios,
                NuevoHorario = model // model ya contiene los datos del formulario y los errores
            };
            TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? "Por favor corrija los errores del formulario.";
            return View("Horarios", viewModelParaVista);
        }


        // GET: GestionEmpleados/EditarHorario/5
        public ActionResult EditarHorario(int idHorario)
        {
            var horario = _horarioService.ObtenerHorarioPorId(idHorario);
            if (horario == null)
            {
                return HttpNotFound();
            }
            var usuario = _horarioService.ObtenerUsuarioPorId(horario.idUsuario);

            var viewModel = new HorarioFormViewModel
            {
                IdHorario = horario.idHorario,
                IdUsuario = horario.idUsuario,
                NombreUsuario = $"{usuario.nombre} {usuario.apellido}",
                DiaSemana = horario.diaSemana,
                HoraEntrada = horario.horaEntrada,
                HoraSalida = horario.horaSalida,
                PagoPorHora = horario.pagoPorHora,
                FechaInicioVigencia = horario.fechaInicioVigencia,
                FechaFinVigencia = horario.fechaFinVigencia,
                Activo = horario.activo
            };
            return View(viewModel);
        }

        // POST: GestionEmpleados/EditarHorario/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarHorario(HorarioFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.HoraSalida <= model.HoraEntrada)
                {
                    ModelState.AddModelError("HoraSalida", "La hora de salida debe ser posterior a la hora de entrada.");
                }
                if (model.FechaFinVigencia.HasValue && model.FechaFinVigencia < model.FechaInicioVigencia)
                {
                    ModelState.AddModelError("FechaFinVigencia", "La fecha fin de vigencia no puede ser anterior a la fecha de inicio.");
                }

                if (ModelState.IsValid)
                {
                    if (_horarioService.ActualizarHorario(model))
                    {
                        TempData["SuccessMessage"] = "Horario actualizado exitosamente.";
                        return RedirectToAction("Horarios", new { idUsuario = model.IdUsuario });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al actualizar el horario.";
                    }
                }
            }
            // Si hay error, volver a cargar el nombre de usuario para el ViewModel
            var usuario = _horarioService.ObtenerUsuarioPorId(model.IdUsuario);
            model.NombreUsuario = $"{usuario.nombre} {usuario.apellido}";
            TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? "Por favor corrija los errores del formulario.";
            return View(model);
        }


        // POST: GestionEmpleados/EliminarHorario/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarHorario(int idHorario, int idUsuario) // idUsuario para la redirección
        {
            if (_horarioService.EliminarHorario(idHorario)) // O DesactivarHorario
            {
                TempData["SuccessMessage"] = "Horario eliminado/desactivado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al eliminar/desactivar el horario.";
            }
            return RedirectToAction("Horarios", new { idUsuario = idUsuario });
        }
    }
}
using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Services;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using SistemaWebEficienciaOperativa.Models;
using System.Diagnostics;
using System.Collections.Generic; // Para tbHorario si lo usas directamente

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

        // GET: Empleados
        public ActionResult Index()
        {
            var usuarios = _horarioService.ObtenerTodosLosUsuariosParaGestion();
            return View(usuarios);
        }

        // GET: Empleados/Horarios/5
        public ActionResult Horarios(int idUsuario)
        {
            var usuario = _horarioService.ObtenerUsuarioPorId(idUsuario);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado."; // Añadir mensaje para redirección desde otros lados
                return HttpNotFound("Usuario no encontrado.");
            }

            var horarios = _horarioService.ObtenerHorariosPorUsuario(idUsuario);
            var viewModel = new UsuarioHorariosViewModel
            {
                IdUsuario = usuario.idUsuario,
                NombreUsuario = $"{usuario.nombre} {usuario.apellido}",
                HorariosAsignados = horarios,
                NuevoHorario = new HorarioFormViewModel
                {
                    IdUsuario = idUsuario
                    // DiasSeleccionados se inicializa a List<string>() en el constructor de HorarioFormViewModel
                    // DiasSemanaOptions se inicializa en el constructor de HorarioFormViewModel
                }
            };

            return View(viewModel);
        }

        // POST: Empleados/AgregarHorario
        // POST: Empleados/AgregarHorario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarHorario(UsuarioHorariosViewModel viewModelFromForm) // El modelo principal es UsuarioHorariosViewModel
        {
            // Extraemos el submodelo que nos interesa para el formulario
            HorarioFormViewModel model = viewModelFromForm.NuevoHorario;

            if (model == null) // Esto es improbable si el formulario está bien estructurado
            {
                TempData["ErrorMessage"] = "Error: No se recibieron datos del formulario.";
                // Si viewModelFromForm.IdUsuario existe, úsalo para la redirección.
                // De lo contrario, es un error más grave, ir a Index.
                if (viewModelFromForm != null && viewModelFromForm.IdUsuario > 0)
                    return RedirectToAction("Horarios", new { idUsuario = viewModelFromForm.IdUsuario });
                else if (viewModelFromForm != null && viewModelFromForm.NuevoHorario != null && viewModelFromForm.NuevoHorario.IdUsuario > 0) // Intento adicional
                    return RedirectToAction("Horarios", new { idUsuario = viewModelFromForm.NuevoHorario.IdUsuario });
                return RedirectToAction("Index");
            }

            // Validar que al menos un día fue seleccionado
            if (model.DiasSeleccionados == null || !model.DiasSeleccionados.Any())
            {
                // El name en el HTML es "NuevoHorario.DiasSeleccionados", así que el error debe asociarse a él
                ModelState.AddModelError("NuevoHorario.DiasSeleccionados", "Debe seleccionar al menos un día de la semana.");
            }

            // Validaciones personalizadas adicionales sobre los datos comunes (horas, fechas)
            if (model.HoraSalida <= model.HoraEntrada)
            {
                ModelState.AddModelError("NuevoHorario.HoraSalida", "La hora de salida debe ser posterior a la hora de entrada.");
            }
            if (model.FechaFinVigencia.HasValue && model.FechaFinVigencia < model.FechaInicioVigencia)
            {
                ModelState.AddModelError("NuevoHorario.FechaFinVigencia", "La fecha fin de vigencia no puede ser anterior a la fecha de inicio.");
            }

            if (ModelState.IsValid) // Este ModelState se aplicará a las propiedades de viewModelFromForm.NuevoHorario
            {
                int horariosAgregados = 0;
                int erroresAlAgregar = 0;
                List<string> mensajesErrorDetallados = new List<string>();

                foreach (var diaSeleccionadoStr in model.DiasSeleccionados)
                {
                    if (byte.TryParse(diaSeleccionadoStr, out byte diaSemanaByte))
                    {
                        // Creamos una nueva instancia de HorarioFormViewModel para pasar al servicio
                        // Esta instancia solo contendrá la información para UN horario (un día específico)
                        var horarioIndividualParaGuardar = new HorarioFormViewModel
                        {
                            IdUsuario = model.IdUsuario,
                            DiaSemana = diaSemanaByte, // El día específico de esta iteración
                            HoraEntrada = model.HoraEntrada,
                            HoraSalida = model.HoraSalida,
                            PagoPorHora = model.PagoPorHora,
                            FechaInicioVigencia = model.FechaInicioVigencia,
                            FechaFinVigencia = model.FechaFinVigencia,
                            Activo = model.Activo
                            // No necesitamos pasar DiasSeleccionados ni DiasSemanaOptions al servicio para este objeto individual
                        };

                        // Aquí asumo que tu _horarioService.AgregarHorario toma un HorarioFormViewModel
                        // y usa la propiedad 'DiaSemana' de ese ViewModel.
                        if (_horarioService.AgregarHorario(horarioIndividualParaGuardar))
                        {
                            horariosAgregados++;
                        }
                        else
                        {
                            erroresAlAgregar++;
                            // Podrías obtener un mensaje de error más específico del servicio si lo devuelve
                            mensajesErrorDetallados.Add($"Error al agregar horario para el día valor: {diaSemanaByte}.");
                        }
                    }
                    else
                    {
                        erroresAlAgregar++;
                        mensajesErrorDetallados.Add($"Valor de día inválido: {diaSeleccionadoStr}.");
                        // Esto no debería ocurrir si los values de los checkboxes son correctos
                    }
                }

                if (horariosAgregados > 0 && erroresAlAgregar == 0)
                {
                    TempData["SuccessMessage"] = $"{horariosAgregados} horario(s) agregado(s) exitosamente.";
                }
                else if (horariosAgregados > 0 && erroresAlAgregar > 0)
                {
                    TempData["WarningMessage"] = $"{horariosAgregados} horario(s) agregado(s). {erroresAlAgregar} error(es) al intentar agregar algunos horarios. Detalles: {string.Join(" ", mensajesErrorDetallados)}";
                }
                else if (erroresAlAgregar > 0) // Ninguno agregado, solo errores
                {
                    TempData["ErrorMessage"] = $"Error al agregar el/los horario(s). {erroresAlAgregar} error(es) ocurrieron. Detalles: {string.Join(" ", mensajesErrorDetallados)}";
                }
                else // Ningún día seleccionado y pasó la validación? Improbable, pero por si acaso.
                {
                    TempData["ErrorMessage"] = "No se seleccionaron días o ocurrió un error inesperado.";
                }
                return RedirectToAction("Horarios", new { idUsuario = model.IdUsuario });
            }

            // Si ModelState no es válido, volvemos a mostrar el formulario con los errores.
            // Necesitamos recargar la información del usuario y los horarios asignados para el ViewModel completo.

            // Es crucial que model.IdUsuario tenga un valor aquí.
            // Si se pierde, es un problema de binding del HiddenFor o de cómo se está pasando el modelo.
            if (model.IdUsuario <= 0)
            {
                Debug.WriteLine($"Error crítico en POST AgregarHorario: IdUsuario en NuevoHorario es {model.IdUsuario} después de validación fallida.");
                TempData["ErrorMessage"] = "Error crítico: Información del usuario perdida al procesar el formulario. Intente nuevamente.";
                // Intentar obtener IdUsuario de viewModelFromForm si está disponible
                if (viewModelFromForm.IdUsuario > 0)
                    return RedirectToAction("Horarios", new { idUsuario = viewModelFromForm.IdUsuario });
                return RedirectToAction("Index");
            }

            var usuario = _horarioService.ObtenerUsuarioPorId(model.IdUsuario);
            if (usuario == null)
            {
                Debug.WriteLine($"Error: Usuario no encontrado con IdUsuario {model.IdUsuario} al recargar formulario con errores.");
                TempData["ErrorMessage"] = "Error: Usuario no encontrado al intentar recargar el formulario con errores.";
                return RedirectToAction("Index");
            }

            var horariosAsignados = _horarioService.ObtenerHorariosPorUsuario(model.IdUsuario);

            // Reconstruir el ViewModel completo para la vista, pasando el 'model' (NuevoHorario)
            // que ya contiene los datos ingresados por el usuario y los errores de validación.
            var viewModelParaVista = new UsuarioHorariosViewModel
            {
                IdUsuario = usuario.idUsuario,
                NombreUsuario = $"{usuario.nombre} {usuario.apellido}",
                HorariosAsignados = horariosAsignados,
                NuevoHorario = model // 'model' (que es viewModelFromForm.NuevoHorario) ya tiene los datos y errores.
                                     // Su propiedad DiasSemanaOptions se inicializa en su constructor.
            };

            if (!TempData.ContainsKey("ErrorMessage")) // Evitar sobreescribir un error más específico
            {
                TempData["ErrorMessage"] = "Por favor corrija los errores del formulario.";
            }
            return View("Horarios", viewModelParaVista);
        }


        // GET: Empleados/EditarHorario/5
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

        // POST: Empleados/EditarHorario/5
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


        // POST: Empleados/EliminarHorario/5
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
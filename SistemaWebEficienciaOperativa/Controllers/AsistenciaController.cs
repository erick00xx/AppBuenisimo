using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using SistemaWebEficienciaOperativa.Services;

namespace TuProyecto.Controllers // Asegúrate que el namespace sea el correcto
{
    //[Authorize] // Este atributo [Authorize] necesitará ser personalizado si no usas ASP.NET Identity.
                // Si solo dependes de la sesión, podrías crear un filtro de acción personalizado
                // o simplemente verificar la sesión en cada acción o en un método base del controlador.
    public class AsistenciaController : Controller
    {
        private readonly AsistenciaService _asistenciaService;
        // private readonly HorarioService _horarioService; // Ya no es necesario para obtener datos del usuario si AsistenciaService lo maneja

        public AsistenciaController()
        {
            _asistenciaService = new AsistenciaService();
            // _horarioService = new HorarioService();
        }

        private bool ObtenerIdUsuarioActual(out int idUsuario)
        {
            idUsuario = 0;
            if (Session["idUsuario"] == null)
            {
                return false; // No hay sesión o el idUsuario no está en la sesión
            }

            if (Session["idUsuario"] is int)
            {
                idUsuario = (int)Session["idUsuario"];
                return true;
            }
            // Si no es int, intenta convertir (aunque debería ser int si lo guardaste así)
            else if (int.TryParse(Session["idUsuario"].ToString(), out int parsedId))
            {
                idUsuario = parsedId;
                return true;
            }
            return false; // No se pudo convertir a int
        }


        // GET: Asistencia/Marcar
        public ActionResult Marcar()
        {
            if (!ObtenerIdUsuarioActual(out int idUsuarioActual))
            {
                TempData["ErrorMessage"] = "Sesión expirada o no válida. Por favor, inicie sesión de nuevo.";
                return RedirectToAction("Index", "Autenticacion"); // Redirige al login
            }

            var viewModel = _asistenciaService.PrepararViewModelMarcacion(idUsuarioActual);
            if (viewModel.SucursalesDisponibles == null || !viewModel.SucursalesDisponibles.Any())
            {
                // Si AsistenciaService no pudo cargar las sucursales (quizás porque el usuario no existe, aunque no debería pasar aquí)
                // o si simplemente quieres asegurarte de que se carguen aquí.
                viewModel.SucursalesDisponibles = new SelectList(_asistenciaService.ObtenerSucursales(), "idSucursal", "nombre", viewModel.IdSucursalSeleccionada);
            }
            return View(viewModel);
        }

        // POST: Asistencia/RegistrarEntrada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEntrada(MarcarAsistenciaViewModel model)
        {
            if (!ObtenerIdUsuarioActual(out int idUsuarioActual))
            {
                TempData["ErrorMessage"] = "Sesión expirada o no válida. Por favor, inicie sesión de nuevo.";
                return RedirectToAction("Index", "Autenticacion");
            }

            if (model.IdSucursalSeleccionada <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una sucursal para marcar la entrada.";
                // Es mejor recargar el modelo completo para la vista si falla la validación del lado del cliente
                var viewModelRecargado = _asistenciaService.PrepararViewModelMarcacion(idUsuarioActual);
                viewModelRecargado.IdSucursalSeleccionada = model.IdSucursalSeleccionada; // Mantener el intento del usuario
                // Podrías añadir un error de ModelState aquí también
                // ModelState.AddModelError("IdSucursalSeleccionada", "Debe seleccionar una sucursal.");
                return View("Marcar", viewModelRecargado);
            }

            var resultado = _asistenciaService.RegistrarEntrada(idUsuarioActual, model.IdSucursalSeleccionada);
            if (resultado.Item1) // Éxito
            {
                TempData["SuccessMessage"] = resultado.Item2;
            }
            else // Error
            {
                TempData["ErrorMessage"] = resultado.Item2;
            }
            return RedirectToAction("Marcar");
        }

        // POST: Asistencia/RegistrarSalida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarSalida(MarcarAsistenciaViewModel model)
        {
            if (!ObtenerIdUsuarioActual(out int idUsuarioActual))
            {
                TempData["ErrorMessage"] = "Sesión expirada o no válida. Por favor, inicie sesión de nuevo.";
                return RedirectToAction("Index", "Autenticacion");
            }

            if (model.IdSucursalSeleccionada <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una sucursal para marcar la salida.";
                var viewModelRecargado = _asistenciaService.PrepararViewModelMarcacion(idUsuarioActual);
                viewModelRecargado.IdSucursalSeleccionada = model.IdSucursalSeleccionada;
                return View("Marcar", viewModelRecargado);
            }

            var resultado = _asistenciaService.RegistrarSalida(idUsuarioActual, model.IdSucursalSeleccionada, model.ConfirmarSalidaTemprana);

            if (resultado.Item1) // Éxito
            {
                TempData["SuccessMessage"] = resultado.Item2;
            }
            else // Error
            {
                if (resultado.Item2 == "SALIDA_TEMPRANA_REQUIERE_CONFIRMACION")
                {
                    TempData["WarningMessage"] = "Estás intentando marcar tu salida muy temprano. Por favor, confirma si deseas continuar.";
                    var viewModelRecargado = _asistenciaService.PrepararViewModelMarcacion(idUsuarioActual);
                    viewModelRecargado.IdSucursalSeleccionada = model.IdSucursalSeleccionada;
                    viewModelRecargado.ConfirmarSalidaTemprana = false; // Para que el checkbox no venga pre-marcado
                    viewModelRecargado.RequiereConfirmacionSalidaTemprana = true;
                    return View("Marcar", viewModelRecargado);
                }
                TempData["ErrorMessage"] = resultado.Item2;
            }
            return RedirectToAction("Marcar");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _asistenciaService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
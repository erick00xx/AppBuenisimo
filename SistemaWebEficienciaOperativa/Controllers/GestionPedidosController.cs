// SistemaWebEficienciaOperativa.Controllers/GestionPedidosController.cs
using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Services;
using SistemaWebEficienciaOperativa.Models; // Para tbEstadosPedidos directamente si es necesario
using SistemaWebEficienciaOperativa.Models.ViewModels;

namespace SistemaWebEficienciaOperativa.Controllers
{
    // [Authorize] // Considera añadir autorización global o por acción
    public class GestionPedidosController : Controller
    {
        private readonly PedidoService _pedidoService;
        private readonly MesaService _mesaService;

        public GestionPedidosController()
        {
            _pedidoService = new PedidoService();
            _mesaService = new MesaService();
        }

        // GET: GestionPedidos (Vista de Mesas)
        public ActionResult Index()
        {
            var mesas = _mesaService.ListarMesasConEstadoCalculado();
            var pedidosActivos = mesas
                .Where(m => m.estado == "Ocupada")
                .Select(m => new { m.idMesa, Pedido = _pedidoService.ObtenerPedidoActivoPorMesa(m.idMesa) })
                .Where(mp => mp.Pedido != null)
                .ToDictionary(mp => mp.idMesa, mp => mp.Pedido.idPedido);

            ViewBag.PedidosActivosPorMesa = pedidosActivos;
            return View(mesas);
        }

        // GET: GestionPedidos/TomarPedido/{idMesa}
        public ActionResult TomarPedido(int? idMesa) // Sigue siendo para NUEVOS pedidos
        {
            if (!idMesa.HasValue)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una mesa.";
                return RedirectToAction("Index");
            }
            var mesa = _mesaService.ObtenerMesaPorId(idMesa.Value);
            if (mesa == null)
            {
                TempData["ErrorMessage"] = "Mesa no encontrada.";
                return RedirectToAction("Index");
            }

            var pedidoActivo = _pedidoService.ObtenerPedidoActivoPorMesa(idMesa.Value);
            if (pedidoActivo != null)
            {
                TempData["InfoMessage"] = $"La mesa {mesa.numeroMesa} ya tiene el pedido N° {pedidoActivo.idPedido} activo.";
                return RedirectToAction("VerOEditarPedido", new { idPedido = pedidoActivo.idPedido });
            }

            ViewBag.IdMesa = idMesa.Value;
            ViewBag.NumeroMesa = mesa.numeroMesa;
            return View();
        }


        [HttpGet]
        public JsonResult BuscarProductos(string term) // Sigue igual
        {
            var productos = _pedidoService.BuscarProductos(term);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        // POST: GestionPedidos/GuardarNuevoPedido (Cambió de GuardarPedido)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarNuevoPedido(string detallePedido, int idMesa) // detallePedido es el JSON
        {
            if (Session["idUsuario"] == null || !int.TryParse(Session["idUsuario"].ToString(), out int idUsuarioLogueado))
            {
                TempData["ErrorMessage"] = "Sesión no válida. Por favor, inicie sesión.";
                return RedirectToAction("Login", "Account"); // Ajusta
            }

            if (string.IsNullOrEmpty(detallePedido) || detallePedido == "[]")
            {
                TempData["ErrorMessage"] = "Debe seleccionar al menos un producto.";
                return RedirectToAction("TomarPedido", new { idMesa = idMesa });
            }

            try
            {
                int nuevoPedidoId = _pedidoService.GuardarNuevoPedido(detallePedido, idUsuarioLogueado, idMesa);
                TempData["SuccessMessage"] = $"Pedido N° {nuevoPedidoId} para la mesa {idMesa} guardado exitosamente.";
                // Redirigir a la vista de edición/visualización del pedido recién creado
                return RedirectToAction("VerOEditarPedido", new { idPedido = nuevoPedidoId });
            }
            catch (ArgumentException argEx)
            {
                TempData["ErrorMessage"] = argEx.Message;
                return RedirectToAction("TomarPedido", new { idMesa = idMesa });
            }
            catch (Exception ex)
            {
                // Loguear el error (ex)
                TempData["ErrorMessage"] = "Error al guardar el pedido: " + ex.Message;
                return RedirectToAction("TomarPedido", new { idMesa = idMesa });
            }
        }

        // GET: GestionPedidos/VerOEditarPedido/{idPedido}
        public ActionResult VerOEditarPedido(int idPedido)
        {
            var pedidoVM = _pedidoService.ObtenerPedidoCompleto(idPedido);
            if (pedidoVM == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction("Index");
            }

            ViewBag.EstadosPedido = new SelectList(_pedidoService.ObtenerTodosLosEstadosPedido(), "idEstadoPedido", "estado", pedidoVM.IdEstadoPedido);
            // depende el estado es editable o no.
            ViewBag.PuedeEditarItems = pedidoVM.EstadoPedido == "En espera";

            return View(pedidoVM);
        }

        // POST: GestionPedidos/ActualizarPedidoCompleto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarPedidoCompleto(PedidoCompletoViewModel pedidoVM)
        {
            if (Session["idUsuario"] == null || !int.TryParse(Session["idUsuario"].ToString(), out int idUsuarioLogueado))
            {
                TempData["ErrorMessage"] = "Sesión no válida.";
                return RedirectToAction("Login", "Account"); // Ajusta
            }

            if (!ModelState.IsValid) // Validar data annotations del ViewModel
            {
                TempData["ErrorMessage"] = "Datos inválidos. Por favor revise los campos.";
                // Recargar vista con los datos y errores
                ViewBag.EstadosPedido = new SelectList(_pedidoService.ObtenerTodosLosEstadosPedido(), "idEstadoPedido", "estado", pedidoVM.IdEstadoPedido);
                var currentPedido = _pedidoService.ObtenerPedidoCompleto(pedidoVM.IdPedido); // Recargar desde DB
                ViewBag.PuedeEditarItems = currentPedido?.EstadoPedido == "En espera";
                return View("VerOEditarPedido", pedidoVM); // Devolver el VM con errores
            }

            try
            {
                _pedidoService.ActualizarPedidoCompleto(pedidoVM, idUsuarioLogueado);
                TempData["SuccessMessage"] = $"Pedido N° {pedidoVM.IdPedido} actualizado correctamente.";
                return RedirectToAction("VerOEditarPedido", new { idPedido = pedidoVM.IdPedido });
            }
            catch (Exception ex)
            {
                // Loguear ex
                TempData["ErrorMessage"] = "Error al actualizar el pedido: " + ex.Message;
                ViewBag.EstadosPedido = new SelectList(_pedidoService.ObtenerTodosLosEstadosPedido(), "idEstadoPedido", "estado", pedidoVM.IdEstadoPedido);
                var currentPedido = _pedidoService.ObtenerPedidoCompleto(pedidoVM.IdPedido); // Recargar desde DB
                ViewBag.PuedeEditarItems = currentPedido?.EstadoPedido == "En espera";

                return View("VerOEditarPedido", pedidoVM);
            }
        }

        // GET: GestionPedidos/PanelCocina
        public ActionResult PanelCocina()
        {
            var pedidosParaCocina = _pedidoService.ObtenerPedidosParaPanelCocina();
            // Para los dropdowns de cambio de estado en cada tarjeta de pedido
            ViewBag.ListaTodosLosEstadosPedido = _pedidoService.ObtenerTodosLosEstadosPedido();
            return View(pedidosParaCocina);
        }

        // POST: GestionPedidos/CambiarEstadoPedidoPanel (Desde el Panel de Barra)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstadoPedidoPanel(int idPedido, int idNuevoEstado)
        {
            if (Session["idUsuario"] == null || !int.TryParse(Session["idUsuario"].ToString(), out int idUsuarioLogueado))
            {
                TempData["ErrorMessage"] = "Sesión no válida.";
                return RedirectToAction("PanelCocina");
            }

            try
            {
                bool success = _pedidoService.ActualizarEstadoPedido(idPedido, idNuevoEstado, idUsuarioLogueado);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Estado del pedido N° {idPedido} actualizado.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"No se pudo actualizar el estado del pedido N° {idPedido} (quizás no existe).";
                }
            }
            catch (Exception ex)
            {
                // Loguear ex
                TempData["ErrorMessage"] = "Error al cambiar estado del pedido: " + ex.Message;
            }
            return RedirectToAction("PanelCocina");
        }
    }
}
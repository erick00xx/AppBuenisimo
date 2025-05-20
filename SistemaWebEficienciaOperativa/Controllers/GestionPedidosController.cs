// SistemaWebEficienciaOperativa.Controllers/GestionPedidosController.cs
using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Services;
using SistemaWebEficienciaOperativa.Models; // Para DB_BUENISIMOEntities
using SistemaWebEficienciaOperativa.Models.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics; // Para .Include si lo usas directamente en el controller

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionPedidosController : Controller
    {
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly DB_BUENISIMOEntities _dbContext = new DB_BUENISIMOEntities(); // Si necesitas acceso directo

        // Asumimos que obtienes esto de la sesión o autenticación
        // Por ahora, lo dejamos hardcodeado para pruebas
        private int ObtenerIdSucursalActual() { return 1; /* TODO: Implementar lógica real */ }
        private int ObtenerIdUsuarioActual()
        {
            Debug.WriteLine(Session["idUsuario"]);
            if (Session["idUsuario"] != null)
            {
                Debug.WriteLine(Session["idUsuario"]);
                return (int)Session["idUsuario"]; // No hay sesión o el idUsuario no está en la sesión
            }
            return 1;
        }

        // GET: GestionPedidos (Vista de Pedidos Activos)
        public ActionResult Index()
        {
            int idSucursal = ObtenerIdSucursalActual();
            var pedidosActivos = _pedidoService.ListarPedidosActivos(idSucursal);
            return View(pedidosActivos);
        }

        // GET: GestionPedidos/NuevoPedido
        public ActionResult NuevoPedido()
        {
            int idSucursal = ObtenerIdSucursalActual();
            var mesasDisponibles = _pedidoService.ListarMesasDisponiblesYActual(idSucursal);
            ViewBag.MesasDisponibles = new SelectList(mesasDisponibles, "codMesa", "codMesa");
            return View();
        }

        // POST: GestionPedidos/CrearPedidoPost
        [HttpPost]
        public ActionResult CrearPedidoPost(CrearPedidoInputViewModel model)
        {
            if (!ModelState.IsValid) // Verifica si el modelo cumple con las validaciones (si las tuviera)
            {
                // Colecta los errores de validación para mostrar un mensaje más detallado si es necesario.
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Datos incompletos o inválidos. " + string.Join(" ", errors) });
            }

            if (model == null || string.IsNullOrEmpty(model.CodMesa) || model.Detalles == null || !model.Detalles.Any())
            {
                return Json(new { success = false, message = "Datos incompletos. Seleccione una mesa y agregue productos." });
            }

            try
            {
                int idUsuarioActual = ObtenerIdUsuarioActual();
                int idSucursal = ObtenerIdSucursalActual(); // Aunque el pedido se asocia a la mesa, podrías necesitarlo
                _pedidoService.CrearPedido(idUsuarioActual, model.CodMesa, model.Detalles, idSucursal);
                return Json(new { success = true, message = "Pedido creado exitosamente.", redirectTo = Url.Action("Index") });
            }
            catch (Exception ex)
            {
                // Loggear el error ex
                return Json(new { success = false, message = "Error al crear el pedido: " + ex.Message });
            }
        }

        // GET: GestionPedidos/DetallesPedido/5
        public ActionResult DetallesPedido(int id)
        {
            var pedido = _pedidoService.ObtenerPedidoPorId(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            int idSucursal = ObtenerIdSucursalActual(); // o pedido.tbMesas.idSucursal.Value;

            // Lista de mesas: disponibles + la actual del pedido
            ViewBag.MesasDisponibles = new SelectList(
                _pedidoService.ListarMesasDisponiblesYActual(idSucursal, pedido.codMesa),
                "codMesa", "codMesa", pedido.codMesa);

            ViewBag.EstadosPedido = new SelectList(
                _pedidoService.ListarEstadosPedido(),
                "idEstadoPedido", "estado", pedido.idEstadoPedido);

            return View(pedido);
        }

        // POST: GestionPedidos/ActualizarPedidoPost
        [HttpPost]
        public ActionResult ActualizarPedidoPost(ActualizarPedidoInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Datos incompletos o inválidos. " + string.Join(" ", errors) });
            }

            if (model.Detalles == null || !model.Detalles.Any())
            {
                return Json(new { success = false, message = "El pedido debe tener al menos un producto." });
            }

            try
            {
                int idUsuarioActual = ObtenerIdUsuarioActual();
                bool actualizado = _pedidoService.ActualizarPedido(model, idUsuarioActual);
                if (actualizado)
                {
                    return Json(new { success = true, message = "Pedido actualizado exitosamente.", redirectTo = Url.Action("Index") });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo encontrar o actualizar el pedido." });
                }
            }
            catch (Exception ex)
            {
                // Loggear el error ex
                return Json(new { success = false, message = "Error al actualizar el pedido: " + ex.Message });
            }
        }


        // GET: GestionPedidos/Buscar (Esta acción ya la tienes y es reutilizable)
        public JsonResult Buscar(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var productos = _pedidoService.Buscar(criterio)
                .Select(p => new
                {
                    p.IdPrecio,
                    NombreProducto = p.tbProductos.nombre,
                    Medida = p.tbMedidas.nombre,
                    Categoria = p.tbProductos.tbCategorias.nombre,
                    Precio = p.Precio
                })
                .ToList();

            return Json(productos, JsonRequestBehavior.AllowGet);
        }
    }
}
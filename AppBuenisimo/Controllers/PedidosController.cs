// AppBuenisimo.Controllers/PedidosController.cs
using System;
using System.Linq;
using System.Web.Mvc;
using AppBuenisimo.Services;
using AppBuenisimo.Models; // Para DB_BUENISIMOEntities
using AppBuenisimo.Models.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics; // Para .Include si lo usas directamente en el controller
using Newtonsoft.Json; // <--- ASEGÚRATE DE TENER ESTE USING
using System.Web;      // <--- ASEGÚRATE DE TENER ESTE USING (para HtmlString)

namespace AppBuenisimo.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly DB_BUENISIMOEntities _dbContext = new DB_BUENISIMOEntities(); // Si necesitas acceso directo

        private const string SESSION_ID_SUCURSAL = "IdSucursalSeleccionada";
        private const int DEFAULT_ID_SUCURSAL = 1; // Sucursal por defecto si ninguna está seleccionada

        private int ObtenerIdSucursalActual()
        {
            if (Session[SESSION_ID_SUCURSAL] != null && int.TryParse(Session[SESSION_ID_SUCURSAL].ToString(), out int idSucursal))
            {
                return idSucursal;
            }
            // Si no hay nada en sesión, o no es un int válido, establece y devuelve la por defecto
            Session[SESSION_ID_SUCURSAL] = DEFAULT_ID_SUCURSAL;
            return DEFAULT_ID_SUCURSAL;
        }
        private void EstablecerIdSucursalActual(int idSucursal)
        {
            Session[SESSION_ID_SUCURSAL] = idSucursal;
        }

        private int ObtenerIdUsuarioActual()
        {
            // Tu lógica actual para obtener el usuario
            if (Session["idUsuario"] != null && int.TryParse(Session["idUsuario"].ToString(), out int idUsuario))
            {
                return idUsuario;
            }
            return 1; // Usuario por defecto para pruebas
        }

        // GET: Pedidos (Vista de Pedidos Activos)
        public ActionResult Index()
        {
            int idSucursalActual = ObtenerIdSucursalActual();
            var pedidosActivos = _pedidoService.ListarPedidosActivos(idSucursalActual);

            // Obtener todas las sucursales para el dropdown
            var todasLasSucursales = _pedidoService.ListarTodasLasSucursales();
            ViewBag.SucursalesDisponibles = new SelectList(todasLasSucursales, "idSucursal", "nombre", idSucursalActual);
            ViewBag.IdSucursalActual = idSucursalActual; // Para mostrar el nombre de la sucursal actual si se desea

            return View(pedidosActivos);
        }

        // GET: Pedidos/NuevoPedido
        public ActionResult NuevoPedido()
        {
            int idSucursal = ObtenerIdSucursalActual();
            var mesasDisponibles = _pedidoService.ListarMesasDisponiblesYActual(idSucursal);
            ViewBag.MesasDisponibles = new SelectList(mesasDisponibles, "codMesa", "codMesa");
            // Podríamos cargar aquí todos los agregados si quisiéramos tenerlos disponibles sin búsqueda inicial
            // ViewBag.Agregados = _pedidoService.ListarTodosAgregados();
            return View();
        }

        // POST: Pedidos/CrearPedidoPost
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

        // GET: Pedidos/DetallesPedido/5
        public ActionResult DetallesPedido(int id)
        {
            var pedido = _pedidoService.ObtenerPedidoPorId(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            int idSucursal = ObtenerIdSucursalActual();

            ViewBag.MesasDisponibles = new SelectList(
                _pedidoService.ListarMesasDisponiblesYActual(idSucursal, pedido.codMesa),
                "codMesa", "codMesa", pedido.codMesa);

            ViewBag.EstadosPedido = new SelectList(
                _pedidoService.ListarEstadosPedido(),
                "idEstadoPedido", "estado", pedido.idEstadoPedido);
            ViewBag.MetodosPago = new SelectList(_dbContext.tbMetodosPago.ToList(), "idMetodoPago", "nombre");
            // --- LÓGICA MEJORADA PARA ViewBag.TodosAgregadosJson ---
            string jsonAgregados = "[]"; // Default a un array JSON vacío
            try
            {
                var todosAgregadosLista = _pedidoService.ListarTodosAgregados();
                if (todosAgregadosLista != null && todosAgregadosLista.Any())
                {
                    var agregadosParaView = todosAgregadosLista
                        .Select(a => new { a.idAgregado, a.nombre, precio = a.precio }) // Asegurar que precio no sea null
                        .ToList();
                    jsonAgregados = JsonConvert.SerializeObject(agregadosParaView);
                    System.Diagnostics.Trace.WriteLine("JSON de Agregados generado: " + jsonAgregados); // Usar Trace en lugar de Debug
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("No se encontraron agregados en el servicio ListarTodosAgregados o la lista está vacía.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error al obtener o serializar TodosAgregados: " + ex.ToString());
                // jsonAgregados ya es "[]" por defecto, así que no es necesario reasignarlo aquí.
            }
            ViewBag.TodosAgregadosJson = new HtmlString(jsonAgregados);
            // --- FIN LÓGICA MEJORADA ---

            return View(pedido);
        }

        // POST: Pedidos/ActualizarPedidoPost
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


        // GET: Pedidos/Buscar (Esta acción ya la tienes y es reutilizable)
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

        // NUEVA ACCIÓN PARA BUSCAR AGREGADOS
        [HttpGet] // Especificar GET
        public JsonResult BuscarAgregados(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                // Devuelve una lista vacía o los primeros N agregados si no hay criterio
                var todosAgregados = _pedidoService.ListarTodosAgregados().Take(5)
                    .Select(a => new { a.idAgregado, a.nombre, a.precio });
                return Json(todosAgregados, JsonRequestBehavior.AllowGet);
            }

            var agregados = _pedidoService.BuscarAgregados(criterio)
                .Select(a => new {
                    a.idAgregado,
                    a.nombre,
                    a.precio
                    // puedes añadir descripción si la usas en el frontend
                })
                .ToList();
            return Json(agregados, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CambiarSucursal(int idSucursalSeleccionada)
        {
            EstablecerIdSucursalActual(idSucursalSeleccionada);
            return RedirectToAction("Index"); // Redirige al Index, que ahora usará la nueva sucursal de la sesión
        }
        // POST: /Ventas/Culminar
        [HttpPost]
        public ActionResult Culminar(int idPedido, int idMetodoPago)
        {
            if (idPedido <= 0 || idMetodoPago <= 0)
            {
                return Json(new { success = false, message = "Datos inválidos." });
            }

            try
            {
                bool culminado = _pedidoService.CulminarPedidoComoVenta(idPedido, idMetodoPago);
                if (culminado)
                {
                    return Json(new { success = true, message = "¡Venta generada exitosamente!", redirectTo = Url.Action("Index", "Pedidos") });
                }
                else
                {
                    // Este caso es poco probable si el servicio lanza la excepción, pero es una salvaguarda.
                    return Json(new { success = false, message = "No se pudo culminar la venta." });
                }
            }
            catch (Exception ex)
            {
                // Capturamos el error relanzado por el servicio para dar un feedback más útil.
                // Por ejemplo, el error de "UNIQUE constraint" si se intenta culminar dos veces.
                string userMessage = "Ocurrió un error al generar la venta. Es posible que este pedido ya haya sido culminado.";
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    userMessage = "Error: Este pedido ya ha sido registrado como una venta.";
                }
                return Json(new { success = false, message = userMessage });
            }
        }
    }
}
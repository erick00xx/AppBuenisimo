// SistemaWebEficienciaOperativa.Controllers/PedidosController.cs
using System;
using System.Linq;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Services;
using SistemaWebEficienciaOperativa.Models; // Para DB_BUENISIMOEntities
using SistemaWebEficienciaOperativa.Models.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics; // Para .Include si lo usas directamente en el controller
using Newtonsoft.Json; // <--- ASEGÚRATE DE TENER ESTE USING
using System.Web;      // <--- ASEGÚRATE DE TENER ESTE USING (para HtmlString)

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class PedidosController : Controller
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

        // GET: Pedidos (Vista de Pedidos Activos)
        public ActionResult Index()
        {
            int idSucursal = ObtenerIdSucursalActual();
            var pedidosActivos = _pedidoService.ListarPedidosActivos(idSucursal);
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
    }
}
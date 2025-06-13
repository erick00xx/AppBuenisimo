// VentasController.cs (Completamente reescrito)
using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Services;
using System;
using System.Web.Mvc;

public class VentasController : Controller
{
    // Idealmente, inyectarías la dependencia, pero seguimos tu patrón actual.
    private readonly VentasService _ventasService = new VentasService();

    // GET: /Ventas/ o /Ventas/Index
    public ActionResult Index()
    {
        var model = _ventasService.ListarVentas();
        return View(model);
    }

    // GET: /Ventas/Detalles/5
    public ActionResult Detalles(int id)
    {
        var model = _ventasService.ObtenerVentaParaEditar(id);
        if (model == null)
        {
            return HttpNotFound();
        }
        return View(model);
    }

    // GET: /Ventas/Editar/5
    public ActionResult Editar(int id)
    {
        // Solo roles específicos deberían poder editar, aquí puedes añadir esa lógica.
        // if (Session["idRol"] != 1) return RedirectToAction("AccesoDenegado");

        var model = _ventasService.ObtenerVentaParaEditar(id);
        if (model == null)
        {
            return HttpNotFound();
        }
        return View(model);
    }

    // POST: /Ventas/Editar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(VentaDetalleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Si el modelo no es válido, recargamos la vista con los errores.
            // Necesitamos recargar los detalles que no se postean.
            var ventaOriginal = _ventasService.ObtenerVentaParaEditar(model.IdVenta);
            model.DetallesVenta = ventaOriginal.DetallesVenta; // Recargar detalles
            return View(model);
        }

        try
        {
            // Obtener el ID del usuario de la sesión para la auditoría
            if (Session["idUsuario"] == null)
            {
                ModelState.AddModelError("", "Tu sesión ha expirado. Por favor, inicia sesión de nuevo.");
                return View(model);
            }
            int idUsuarioActual = (int)Session["idUsuario"];

            bool guardado = _ventasService.GuardarCambiosVenta(model, idUsuarioActual);

            if (guardado)
            {
                TempData["SuccessMessage"] = "¡Venta actualizada correctamente!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo guardar la venta. Inténtalo de nuevo.");
            }
        }
        catch (Exception ex)
        {
            // Loguear error (ex)
            ModelState.AddModelError("", "Ocurrió un error inesperado al guardar la venta.");
        }

        var ventaOriginalFallida = _ventasService.ObtenerVentaParaEditar(model.IdVenta);
        model.DetallesVenta = ventaOriginalFallida.DetallesVenta; // Recargar detalles en caso de error
        return View(model);
    }
}
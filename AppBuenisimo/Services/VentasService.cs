using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class VentasService
{
    private readonly DB_BUENISIMOEntities _dbContext = new DB_BUENISIMOEntities();

    // Método para obtener los detalles de una venta para ver o editar
    public VentaDetalleViewModel ObtenerVentaParaEditar(int idVenta)
    {
        var venta = _dbContext.tbVentas
            .Include(v => v.tbDetalleVenta)
            .Include(v => v.tbUsuarios)        // Incluye el usuario que MODIFICÓ
            .Include(v => v.tbUsuarios1)       // Incluye el usuario de la VENTA ORIGINAL
            .Include(v => v.tbSucursales)
            .Include(v => v.tbMetodosPago)
            .AsNoTracking()
            .FirstOrDefault(v => v.idVenta == idVenta);

        if (venta == null) return null;

        // Mapear la entidad de la BD a nuestro ViewModel con las propiedades correctas
        var viewModel = new VentaDetalleViewModel
        {
            IdVenta = venta.idVenta,
            FechaVenta = venta.fechaVenta,
            CodMesa = venta.codMesa,
            Total = venta.total,

            // CORREGIDO: Usamos tbUsuarios1 para el usuario de la VENTA ORIGINAL
            NombreUsuarioVenta = venta.tbUsuarios1?.nombre + " " + venta.tbUsuarios1?.apellido,

            NombreSucursal = venta.tbSucursales?.nombre,
            MetodoPago = venta.tbMetodosPago?.nombre ?? "No especificado",
            DetallesVenta = venta.tbDetalleVenta?.ToList() ?? new List<tbDetalleVenta>(),
            FueModificada = venta.fueModificada,
            FechaModificacion = venta.fechaModificacion,

            // CORREGIDO: Usamos tbUsuarios para el usuario que MODIFICÓ
            NombreUsuarioModificacion = venta.tbUsuarios?.nombre + " " + venta.tbUsuarios?.apellido,
            MotivoModificacion = venta.motivoModificacion
        };

        return viewModel;
    }

    // Método para listar todas las ventas en el Index
    public List<VentaIndexViewModel> ListarVentas()
    {
        var ventas = _dbContext.tbVentas
            // Solo necesitamos incluir el usuario de la venta original para el listado
            .Include(v => v.tbUsuarios1) // Incluye el usuario de la VENTA ORIGINAL
            .Include(v => v.tbSucursales)
            .OrderByDescending(v => v.fechaVenta)
            .Select(v => new VentaIndexViewModel
            {
                IdVenta = v.idVenta,
                FechaVenta = v.fechaVenta,
                Total = v.total,

                // CORREGIDO: Usamos el operador ternario con tbUsuarios1 para el usuario de la VENTA ORIGINAL
                NombreUsuario = (v.tbUsuarios1 == null) ? "Usuario Eliminado" : (v.tbUsuarios1.nombre + " " + v.tbUsuarios1.apellido),

                NombreSucursal = (v.tbSucursales == null) ? "Sucursal Eliminada" : v.tbSucursales.nombre,
                FueModificada = v.fueModificada
            })
            .ToList();

        return ventas;
    }

    // Método para guardar los cambios de una venta (CRUD - Update)
    // Este método ya estaba bien, no necesita cambios.
    public bool GuardarCambiosVenta(VentaDetalleViewModel model, int idUsuarioModificacion)
    {
        try
        {
            var ventaEnDB = _dbContext.tbVentas.Find(model.IdVenta);
            if (ventaEnDB == null) return false;

            // Actualizar los campos que se pueden editar
            ventaEnDB.total = model.Total;

            // Registrar la auditoría
            ventaEnDB.fueModificada = true;
            ventaEnDB.fechaModificacion = DateTime.Now;
            ventaEnDB.idUsuarioModificacion = idUsuarioModificacion;
            ventaEnDB.motivoModificacion = model.MotivoModificacion;

            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Aquí deberías loguear el error (ex)
            return false;
        }
    }
}
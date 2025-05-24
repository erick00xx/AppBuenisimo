using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SistemaWebEficienciaOperativa.Services
{
    public class ReporteAbastecimientoService
    {
        public class InventarioService
        {
            private readonly DB_BUENISIMOEntities _dbContext;

            public InventarioService()
            {
                _dbContext = new DB_BUENISIMOEntities();
            }

            // Obtener compras recientes
            public List<CompraDTO> ObtenerUltimasCompras(int cantidad = 5)
            {
                // Primero, ejecuta la consulta sin el cálculo del precio unitario
                var compras = (from i in _dbContext.tbIngresosInsumos
                               join ins in _dbContext.tbInsumos on i.idInsumo equals ins.idInsumo
                               join uni in _dbContext.tbUnidades on i.idUnidad equals uni.idUnidad
                               join prov in _dbContext.tbProveedores on i.idProveedor equals prov.idProveedor into g
                               from p in g.DefaultIfEmpty()
                               orderby i.fechaCompra descending
                               select new
                               {
                                   IdIngresoInsumo = i.idIngresoInsumo,
                                   FechaCompra = i.fechaCompra,
                                   NombreInsumo = ins.nombre,
                                   Cantidad = i.cantidad,
                                   Unidad = uni.abreviatura,
                                   Proveedor = p.nombreEmpresa ?? "Sin proveedor",
                                   IdInsumo = i.idInsumo
                               }).Take(cantidad).ToList();

                // Ahora, calcula el total en memoria usando Linq-to-Objects
                return compras.Select(c => new CompraDTO
                {
                    IdIngresoInsumo = c.IdIngresoInsumo,
                    FechaCompra = c.FechaCompra,
                    NombreInsumo = c.NombreInsumo,
                    Cantidad = c.Cantidad,
                    Unidad = c.Unidad,
                    Proveedor = c.Proveedor,
                    Total = c.Cantidad * CalcularPrecioUnitario(c.IdInsumo)
                }).ToList();
            }

            // Obtener desechos recientes
            public List<DesechoDTO> ObtenerUltimosDesechos(int cantidad = 5)
            {
                return (from d in _dbContext.tbDesechosInsumos
                        join ins in _dbContext.tbInsumos on d.idInsumo equals ins.idInsumo
                        join uni in _dbContext.tbUnidades on d.idUnidad equals uni.idUnidad
                        orderby d.fechaDesecho descending
                        select new DesechoDTO
                        {
                            IdDesechoInsumo = d.idDesechoInsumo,
                            FechaDesecho = d.fechaDesecho,
                            NombreInsumo = ins.nombre,
                            Cantidad = d.cantidad,
                            Unidad = uni.abreviatura,
                            Motivo = d.motivo,
                            Observaciones = d.observaciones
                        }).Take(cantidad).ToList();
            }

            // Filtrar compras por rango de fechas
            public List<CompraDTO> FiltrarComprasPorFecha(DateTime fechaInicio, DateTime fechaFin)
            {
                var compras = (from i in _dbContext.tbIngresosInsumos
                               where i.fechaCompra >= fechaInicio && i.fechaCompra <= fechaFin
                               join ins in _dbContext.tbInsumos on i.idInsumo equals ins.idInsumo
                               join uni in _dbContext.tbUnidades on i.idUnidad equals uni.idUnidad
                               join prov in _dbContext.tbProveedores on i.idProveedor equals prov.idProveedor into g
                               from p in g.DefaultIfEmpty()
                               orderby i.fechaCompra descending
                               select new
                               {
                                   IdIngresoInsumo = i.idIngresoInsumo,
                                   FechaCompra = i.fechaCompra,
                                   NombreInsumo = ins.nombre,
                                   Cantidad = i.cantidad,
                                   Unidad = uni.abreviatura,
                                   Proveedor = p.nombreEmpresa ?? "Sin proveedor",
                                   IdInsumo = i.idInsumo
                               }).ToList();

                return compras.Select(c => new CompraDTO
                {
                    IdIngresoInsumo = c.IdIngresoInsumo,
                    FechaCompra = c.FechaCompra,
                    NombreInsumo = c.NombreInsumo,
                    Cantidad = c.Cantidad,
                    Unidad = c.Unidad,
                    Proveedor = c.Proveedor,
                    Total = c.Cantidad * CalcularPrecioUnitario(c.IdInsumo)
                }).ToList();
            }

            // Filtrar desechos por rango de fechas
            public List<DesechoDTO> FiltrarDesechosPorFecha(DateTime fechaInicio, DateTime fechaFin)
            {
                var desechos = (from d in _dbContext.tbDesechosInsumos
                                where d.fechaDesecho >= fechaInicio && d.fechaDesecho <= fechaFin
                                join ins in _dbContext.tbInsumos on d.idInsumo equals ins.idInsumo
                                join uni in _dbContext.tbUnidades on d.idUnidad equals uni.idUnidad
                                orderby d.fechaDesecho descending
                                select new DesechoDTO
                                {
                                    IdDesechoInsumo = d.idDesechoInsumo,
                                    FechaDesecho = d.fechaDesecho,
                                    NombreInsumo = ins.nombre,
                                    Cantidad = d.cantidad,
                                    Unidad = uni.abreviatura,
                                    Motivo = d.motivo,
                                    Observaciones = d.observaciones
                                }).ToList();

                return desechos;
            }

            // Generar estadísticas
            public void GenerarEstadisticas(DateTime fechaReferencia, out int comprasEstaSemana, out int desechosEstaSemana, out decimal inversionTotal)
            {
                var haceUnaSemana = fechaReferencia.AddDays(-7);

                comprasEstaSemana = _dbContext.tbIngresosInsumos.Count(i => i.fechaCompra >= haceUnaSemana);
                desechosEstaSemana = _dbContext.tbDesechosInsumos.Count(d => d.fechaDesecho >= haceUnaSemana);

                var ingresosRecientes = _dbContext.tbIngresosInsumos
                    .Where(i => i.fechaCompra >= haceUnaSemana)
                    .ToList();

                inversionTotal = ingresosRecientes
    .Sum(i => (decimal?)(i.cantidad * CalcularPrecioUnitario(i.idInsumo))) ?? 0m;

            }

            // Método auxiliar para calcular precio unitario
            private decimal CalcularPrecioUnitario(int idInsumo)
            {
                switch (idInsumo)
                {
                    case 1: return 10; // Café
                    case 2: return 0.5m; // Vasos
                    default: return 5;
                }
            }

            // Obtener compra por ID
            public CompraDTO ObtenerUltimaCompraPorId(int id)
            {
                // Primero, ejecuta la consulta sin el cálculo del precio unitario
                var compra = (from i in _dbContext.tbIngresosInsumos
                              join ins in _dbContext.tbInsumos on i.idInsumo equals ins.idInsumo
                              join uni in _dbContext.tbUnidades on i.idUnidad equals uni.idUnidad
                              join prov in _dbContext.tbProveedores on i.idProveedor equals prov.idProveedor into g
                              from p in g.DefaultIfEmpty()
                              where i.idIngresoInsumo == id
                              select new
                              {
                                  IdIngresoInsumo = i.idIngresoInsumo,
                                  FechaCompra = i.fechaCompra,
                                  NombreInsumo = ins.nombre,
                                  Cantidad = i.cantidad,
                                  Unidad = uni.abreviatura,
                                  Proveedor = p.nombreEmpresa ?? "Sin proveedor",
                                  IdInsumo = i.idInsumo
                              }).FirstOrDefault();

                if (compra != null)
                {
                    // Ahora, calcula el total en memoria usando Linq-to-Objects
                    return new CompraDTO
                    {
                        IdIngresoInsumo = compra.IdIngresoInsumo,
                        FechaCompra = compra.FechaCompra,
                        NombreInsumo = compra.NombreInsumo,
                        Cantidad = compra.Cantidad,
                        Unidad = compra.Unidad,
                        Proveedor = compra.Proveedor,
                        Total = compra.Cantidad * CalcularPrecioUnitario(compra.IdInsumo)
                    };
                }

                return null;
            }

            // Obtener desecho por ID
            public DesechoDTO ObtenerUltimoDesechoPorId(int id)
            {
                var desecho = (from d in _dbContext.tbDesechosInsumos
                               join ins in _dbContext.tbInsumos on d.idInsumo equals ins.idInsumo
                               join uni in _dbContext.tbUnidades on d.idUnidad equals uni.idUnidad
                               where d.idDesechoInsumo == id
                               select new DesechoDTO
                               {
                                   IdDesechoInsumo = d.idDesechoInsumo,
                                   FechaDesecho = d.fechaDesecho,
                                   NombreInsumo = ins.nombre,
                                   Cantidad = d.cantidad,
                                   Unidad = uni.abreviatura,
                                   Motivo = d.motivo,
                                   Observaciones = d.observaciones
                               }).FirstOrDefault();

                return desecho;
            }

        }
    }
}


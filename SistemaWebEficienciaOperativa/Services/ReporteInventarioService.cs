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

            public List<CompraDTO> ObtenerUltimasCompras(int cantidad = 5)
            {
                return ObtenerTodasLasCompras().Take(cantidad).ToList();
            }

            public List<DesechoDTO> ObtenerUltimosDesechos(int cantidad = 5)
            {
                return ObtenerTodosLosDesechos().Take(cantidad).ToList();
            }

            public List<CompraDTO> ObtenerTodasLasCompras()
            {
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
                               }).ToList();

                return compras.Select(c => new CompraDTO
                {
                    IdIngresoInsumo = c.IdIngresoInsumo,
                    FechaCompra = c.FechaCompra,
                    NombreInsumo = c.NombreInsumo,
                    Cantidad = c.Cantidad,
                    Unidad = c.Unidad,
                    Proveedor = c.Proveedor,
                }).ToList();
            }

            public List<DesechoDTO> ObtenerTodosLosDesechos()
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
                        }).ToList();
            }

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

                }).ToList();
            }

            public List<DesechoDTO> FiltrarDesechosPorFecha(DateTime fechaInicio, DateTime fechaFin)
            {
                return (from d in _dbContext.tbDesechosInsumos
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
            }

            public void GenerarEstadisticas(DateTime fechaReferencia, out int comprasEstaSemana, out int desechosEstaSemana, out decimal porcentajeDesechadosSobreCompras)
            {
                DateTime inicioSemana = fechaReferencia.AddDays(-(int)fechaReferencia.DayOfWeek);
                DateTime finSemana = inicioSemana.AddDays(7);

                var comprasSemana = _dbContext.tbIngresosInsumos
                    .Where(i => i.fechaCompra >= inicioSemana && i.fechaCompra < finSemana)
                    .ToList();
                comprasEstaSemana = comprasSemana.Count;

                var desechosSemana = _dbContext.tbDesechosInsumos
                    .Where(d => d.fechaDesecho >= inicioSemana && d.fechaDesecho < finSemana)
                    .ToList();
                desechosEstaSemana = desechosSemana.Count;

                decimal totalComprado = comprasSemana.Sum(c => (decimal?)c.cantidad) ?? 0;
                decimal totalDesechado = desechosSemana.Sum(d => (decimal?)d.cantidad) ?? 0;

                porcentajeDesechadosSobreCompras = totalComprado > 0
                    ? Math.Round((totalDesechado / totalComprado) * 100, 2)
                    : 0;
            }


            public CompraDTO ObtenerUltimaCompraPorId(int id)
            {
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
                    return new CompraDTO
                    {
                        IdIngresoInsumo = compra.IdIngresoInsumo,
                        FechaCompra = compra.FechaCompra,
                        NombreInsumo = compra.NombreInsumo,
                        Cantidad = compra.Cantidad,
                        Unidad = compra.Unidad,
                        Proveedor = compra.Proveedor,
                    };
                }

                return null;
            }

            public DesechoDTO ObtenerUltimoDesechoPorId(int id)
            {
                return (from d in _dbContext.tbDesechosInsumos
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
            }
        }
    }
}

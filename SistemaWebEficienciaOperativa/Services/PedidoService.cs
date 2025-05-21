// SistemaWebEficienciaOperativa.Services/PedidoService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Models.ViewModels;
using System.Diagnostics;
using SistemaWebEficienciaOperativa.Utils;

namespace SistemaWebEficienciaOperativa.Services
{
    public class PedidoService
    {
        private readonly List<int> ESTADOS_PEDIDO_ACTIVO = new List<int> { 1, 2, 3, 4 };

        public List<tbPedidos> ListarPedidosActivos(int idSucursal)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                var pedidosActivos = _dbContext.tbPedidos
                    .Include(p => p.tbMesas)
                    .Include(p => p.tbEstadosPedidos)
                    .Include(p => p.tbUsuarios) // Para mostrar nombre de usuario en el Index
                    .Where(p => p.idEstadoPedido.HasValue &&
                                ESTADOS_PEDIDO_ACTIVO.Contains(p.idEstadoPedido.Value) &&
                                p.tbMesas.idSucursal == idSucursal)
                    .OrderByDescending(p => p.fechaPedido) // Mostrar los más recientes primero
                    .ToList();
                return pedidosActivos;
            }
        }

        public List<tbMesas> ListarMesasDisponiblesYActual(int idSucursal, string codMesaActual = null)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                var codMesasOcupadas = _dbContext.tbPedidos
                                        .Where(p => p.tbMesas.idSucursal == idSucursal &&
                                                    p.idEstadoPedido.HasValue &&
                                                    ESTADOS_PEDIDO_ACTIVO.Contains(p.idEstadoPedido.Value) &&
                                                    p.codMesa != codMesaActual) // Excluir la mesa actual de las "ocupadas" si se proporciona
                                        .Select(p => p.codMesa)
                                        .Distinct()
                                        .ToList();

                var mesasDisponibles = _dbContext.tbMesas
                                        .Where(m => m.idSucursal == idSucursal && !codMesasOcupadas.Contains(m.codMesa))
                                        .OrderBy(m => m.codMesa)
                                        .ToList();
                return mesasDisponibles;
            }
        }

        public void CrearPedido(int idUsuario, string codMesa, List<DetallePedidoViewModel> detallesVM, int idSucursal)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                decimal totalPedido = detallesVM.Sum(d => d.Subtotal);
                var pedido = new tbPedidos
                {
                    codMesa = codMesa,
                    fechaPedido = TimeProvider.Now,
                    idUsuario = idUsuario,
                    total = totalPedido,
                    idEstadoPedido = 1, // "En espera"
                };
                _dbContext.tbPedidos.Add(pedido);
                _dbContext.SaveChanges();

                foreach (var detalleVM in detallesVM)
                {
                    var detallePedido = new tbDetallePedido
                    {
                        idPedido = pedido.idPedido,
                        idPrecio = detalleVM.IdPrecio,
                        cantidad = detalleVM.Cantidad,
                        subtotal = detalleVM.Subtotal,
                    };
                    _dbContext.tbDetallePedido.Add(detallePedido);
                }
                _dbContext.SaveChanges();
            }
        }

        public tbPedidos ObtenerPedidoPorId(int idPedido)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbPedidos
                    .Include(p => p.tbMesas)
                    .Include(p => p.tbEstadosPedidos)
                    .Include(p => p.tbUsuarios)
                    .Include(p => p.tbDetallePedido.Select(dp => dp.tbPrecios.tbProductos.tbCategorias))
                    .Include(p => p.tbDetallePedido.Select(dp => dp.tbPrecios.tbMedidas))
                    .FirstOrDefault(p => p.idPedido == idPedido);
            }
        }

        public List<tbEstadosPedidos> ListarEstadosPedido()
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbEstadosPedidos.OrderBy(e => e.idEstadoPedido).ToList();
            }
        }

        public bool ActualizarPedido(ActualizarPedidoInputViewModel model, int idUsuarioActual)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var pedidoExistente = _dbContext.tbPedidos.Find(model.IdPedido);
                        if (pedidoExistente == null)
                        {
                            return false; // O lanzar excepción
                        }

                        // Actualizar campos del pedido
                        pedidoExistente.codMesa = model.CodMesa;
                        pedidoExistente.idEstadoPedido = model.IdEstadoPedido;
                        pedidoExistente.total = model.Detalles.Sum(d => d.Subtotal);
                        // Opcional: registrar quién y cuándo modificó (necesitarías campos adicionales en tbPedidos)
                        // pedidoExistente.idUsuarioUltimaModificacion = idUsuarioActual;
                        // pedidoExistente.fechaUltimaModificacion = TimeProvider.Now;

                        // Eliminar detalles antiguos
                        var detallesAntiguos = _dbContext.tbDetallePedido.Where(d => d.idPedido == model.IdPedido);
                        _dbContext.tbDetallePedido.RemoveRange(detallesAntiguos);

                        // Agregar nuevos detalles
                        foreach (var detalleVM in model.Detalles)
                        {
                            var nuevoDetalle = new tbDetallePedido
                            {
                                idPedido = model.IdPedido,
                                idPrecio = detalleVM.IdPrecio,
                                cantidad = detalleVM.Cantidad,
                                subtotal = detalleVM.Subtotal
                            };
                            _dbContext.tbDetallePedido.Add(nuevoDetalle);
                        }

                        _dbContext.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"Error al actualizar pedido: {ex.Message}");
                        // Loggear el error ex
                        return false;
                    }
                }
            }
        }

        public List<tbPrecios> Listar()
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                var precios = _dbContext.tbPrecios
                    .Include(p => p.tbProductos.tbCategorias)
                    .Include(p => p.tbMedidas)
                    .ToList();
                return precios;
            }
        }

        public List<tbPrecios> Buscar(string criterio)
        {
            var precios = new List<tbPrecios>();
            if (string.IsNullOrEmpty(criterio))
            {
                return precios;
            }
            try
            {
                using (var _dbContext = new DB_BUENISIMOEntities())
                {
                    string criterioLower = criterio.ToLower();
                    precios = _dbContext.tbPrecios
                        .Include(p => p.tbProductos.tbCategorias)
                        .Include(p => p.tbMedidas)
                        .Where(p => p.tbProductos.nombre.ToLower().Contains(criterioLower) ||
                                    p.tbProductos.tbCategorias.nombre.ToLower().Contains(criterioLower) ||
                                    p.tbMedidas.nombre.ToLower().Contains(criterioLower))
                        .Take(20)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al buscar: " + ex.Message);
            }
            return precios;
        }

        public tbPrecios ObtenerProducto(int idPrecio)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                var precio = _dbContext.tbPrecios
                    .Include(p => p.tbProductos.tbCategorias)
                    .Include(p => p.tbMedidas)
                    .FirstOrDefault(p => p.IdPrecio == idPrecio);
                return precio;
            }
        }
    }
}
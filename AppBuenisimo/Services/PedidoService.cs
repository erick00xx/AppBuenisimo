using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using System.Diagnostics;
using AppBuenisimo.Utils;

namespace AppBuenisimo.Services
{
    public class PedidoService
    {
        // Campo privado para el DbContext. Será usado por todos los métodos.
        private readonly DB_BUENISIMOEntities _dbContext;
        private readonly List<int> ESTADOS_PEDIDO_ACTIVO = new List<int> { 1, 2, 3, 4 };

        // Constructor por defecto. La aplicación seguirá usando este constructor.
        // Crea una instancia del DbContext para sí misma.
        public PedidoService()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }

        // Constructor para inyección de dependencias. Usado para las pruebas unitarias.
        // Recibe una instancia del DbContext (que en las pruebas será un Mock).
        public PedidoService(DB_BUENISIMOEntities context)
        {
            _dbContext = context;
        }

        public List<tbSucursales> ListarTodasLasSucursales()
        {
            try
            {
                // Usa el campo _dbContext en lugar de crear uno nuevo.
                return _dbContext.tbSucursales.OrderBy(s => s.nombre).ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en ListarTodasLasSucursales: " + ex.ToString());
                return new List<tbSucursales>();
            }
        }

        public List<tbPedidos> ListarPedidosActivos(int idSucursal)
        {
            var pedidosActivos = _dbContext.tbPedidos
                .Include(p => p.tbMesas)
                .Include(p => p.tbEstadosPedidos)
                .Include(p => p.tbUsuarios)
                .Where(p => p.idEstadoPedido.HasValue &&
                            ESTADOS_PEDIDO_ACTIVO.Contains(p.idEstadoPedido.Value) &&
                            p.tbMesas.idSucursal == idSucursal)
                .OrderByDescending(p => p.fechaPedido)
                .ToList();
            return pedidosActivos;
        }

        public List<tbMesas> ListarMesasDisponiblesYActual(int idSucursal, string codMesaActual = null)
        {
            var codMesasOcupadas = _dbContext.tbPedidos
                                    .Where(p => p.tbMesas.idSucursal == idSucursal &&
                                                p.idEstadoPedido.HasValue &&
                                                ESTADOS_PEDIDO_ACTIVO.Contains(p.idEstadoPedido.Value) &&
                                                p.codMesa != codMesaActual)
                                    .Select(p => p.codMesa)
                                    .Distinct()
                                    .ToList();

            var mesasDisponibles = _dbContext.tbMesas
                                    .Where(m => m.idSucursal == idSucursal && !codMesasOcupadas.Contains(m.codMesa))
                                    .OrderBy(m => m.codMesa)
                                    .ToList();
            return mesasDisponibles;
        }

        public void CrearPedido(int idUsuario, string codMesa, List<DetallePedidoViewModel> detallesVM, int idSucursal)
        {
            decimal totalPedido = detallesVM.Sum(d => d.Subtotal);

            int? sucursalId = _dbContext.tbMesas
                        .Where(m => m.codMesa == codMesa)
                        .Select(m => (int?)m.idSucursal)
                        .FirstOrDefault();
            var pedido = new tbPedidos
            {
                codMesa = codMesa,
                fechaPedido = TimeProvider.Now,
                idUsuario = idUsuario,
                total = totalPedido,
                idEstadoPedido = 1,
                idSucursal = sucursalId
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
                    tipoLeche = detalleVM.TipoLeche,
                    tipoAzucar = detalleVM.TipoAzucar,
                    cantidadHielo = detalleVM.CantidadHielo,
                    idAgregado1 = detalleVM.IdAgregado1,
                    idAgregado2 = detalleVM.IdAgregado2,
                    idAgregado3 = detalleVM.IdAgregado3
                };
                _dbContext.tbDetallePedido.Add(detallePedido);
            }
            _dbContext.SaveChanges();
        }

        public tbPedidos ObtenerPedidoPorId(int idPedido)
        {
            return _dbContext.tbPedidos
                .Include(p => p.tbMesas)
                .Include(p => p.tbEstadosPedidos)
                .Include(p => p.tbUsuarios)
                .Include(p => p.tbDetallePedido.Select(dp => dp.tbPrecios.tbProductos.tbCategorias))
                .Include(p => p.tbDetallePedido.Select(dp => dp.tbPrecios.tbMedidas))
                .FirstOrDefault(p => p.idPedido == idPedido);

        }

        public List<tbEstadosPedidos> ListarEstadosPedido()
        {
            return _dbContext.tbEstadosPedidos
                             .Where(e => e.idEstadoPedido != 5)
                             .OrderBy(e => e.idEstadoPedido)
                             .ToList();
        }

        public bool ActualizarPedido(ActualizarPedidoInputViewModel model, int idUsuarioActual)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var pedidoExistente = _dbContext.tbPedidos.Find(model.IdPedido);
                    if (pedidoExistente == null) return false;

                    pedidoExistente.codMesa = model.CodMesa;
                    pedidoExistente.idEstadoPedido = model.IdEstadoPedido;
                    pedidoExistente.total = model.Detalles.Sum(d => d.Subtotal);

                    var detallesAntiguos = _dbContext.tbDetallePedido.Where(d => d.idPedido == model.IdPedido);
                    _dbContext.tbDetallePedido.RemoveRange(detallesAntiguos);
                    _dbContext.SaveChanges();

                    foreach (var detalleVM in model.Detalles)
                    {
                        var nuevoDetalle = new tbDetallePedido
                        {
                            idPedido = model.IdPedido,
                            idPrecio = detalleVM.IdPrecio,
                            cantidad = detalleVM.Cantidad,
                            subtotal = detalleVM.Subtotal,
                            tipoLeche = detalleVM.TipoLeche,
                            tipoAzucar = detalleVM.TipoAzucar,
                            cantidadHielo = detalleVM.CantidadHielo,
                            idAgregado1 = detalleVM.IdAgregado1,
                            idAgregado2 = detalleVM.IdAgregado2,
                            idAgregado3 = detalleVM.IdAgregado3
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
                    return false;
                }
            }
        }
        public bool CulminarPedidoComoVenta(int idPedido, int idMetodoPago)
        {
            try
            {
                // El SP ahora es la única fuente de verdad para esta operación.
                // No necesitamos una transacción explícita aquí porque el SP ya la gestiona internamente.
                var paramPedidoId = new System.Data.SqlClient.SqlParameter("@idPedidoACulminar", idPedido);
                var paramMetodoPagoId = new System.Data.SqlClient.SqlParameter("@idMetodoPago", idMetodoPago);

                _dbContext.Database.ExecuteSqlCommand("EXEC sp_CulminarPedidoYGenerarVenta @idPedidoACulminar, @idMetodoPago",
                    paramPedidoId,
                    paramMetodoPagoId);

                return true;
            }
            catch (Exception ex)
            {
                // Loguea el error real para depuración.
                System.Diagnostics.Debug.WriteLine($"Error al ejecutar sp_CulminarPedidoYGenerarVenta: {ex.Message}");
                // Puedes lanzar la excepción para que el controlador la maneje y devuelva un mensaje más específico.
                throw;
            }
        }

        public List<tbPrecios> Listar()
        {
            var precios = _dbContext.tbPrecios
                .Include(p => p.tbProductos.tbCategorias)
                .Include(p => p.tbMedidas)
                .ToList();
            return precios;
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
            catch (Exception ex)
            {
                Console.WriteLine("Error al buscar: " + ex.Message);
            }
            return precios;
        }

        public tbPrecios ObtenerProducto(int idPrecio)
        {
            var precio = _dbContext.tbPrecios
                .Include(p => p.tbProductos.tbCategorias)
                .Include(p => p.tbMedidas)
                .FirstOrDefault(p => p.IdPrecio == idPrecio);
            return precio;
        }

        public List<tbAgregados> BuscarAgregados(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                return _dbContext.tbAgregados.Take(10).ToList();
            }
            string criterioLower = criterio.ToLower();
            return _dbContext.tbAgregados
                            .Where(a => a.nombre.ToLower().Contains(criterioLower) ||
                                        (a.descripcion != null && a.descripcion.ToLower().Contains(criterioLower)))
                            .Take(10)
                            .ToList();
        }

        public List<tbAgregados> ListarTodosAgregados()
        {
            try
            {
                return _dbContext.tbAgregados.ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en PedidoService.ListarTodosAgregados: " + ex.ToString());
                return new List<tbAgregados>();
            }
        }
    }
}
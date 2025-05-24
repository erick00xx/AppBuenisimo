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
                // El total del pedido ahora se calcula en el frontend y se envía en cada detalle.
                // O se recalcula aquí basado en los precios base + agregados si fuera necesario
                // Por ahora, confiamos en el subtotal enviado desde el frontend, que ya debería incluir agregados.
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
                    idEstadoPedido = 1, // "Pendiente" o el estado inicial que definas
                    idSucursal = sucursalId
                };
                _dbContext.tbPedidos.Add(pedido);
                _dbContext.SaveChanges(); // Guardar pedido para obtener su ID

                foreach (var detalleVM in detallesVM)
                {
                    var detallePedido = new tbDetallePedido
                    {
                        idPedido = pedido.idPedido,
                        idPrecio = detalleVM.IdPrecio,
                        cantidad = detalleVM.Cantidad,
                        subtotal = detalleVM.Subtotal, // Este subtotal YA incluye los agregados

                        // Nuevos campos
                        tipoLeche = detalleVM.TipoLeche,
                        tipoAzucar = detalleVM.TipoAzucar,
                        cantidadHielo = detalleVM.CantidadHielo,
                        idAgregado1 = detalleVM.IdAgregado1,
                        idAgregado2 = detalleVM.IdAgregado2,
                        idAgregado3 = detalleVM.IdAgregado3
                    };
                    _dbContext.tbDetallePedido.Add(detallePedido);
                }
                _dbContext.SaveChanges(); // Guardar todos los detalles
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
                    .Include(p => p.tbDetallePedido.Select(dp => dp.tbPrecios.tbProductos.tbCategorias)) // Asegura tbPrecios y tbProductos
                    .Include(p => p.tbDetallePedido.Select(dp => dp.tbPrecios.tbMedidas))          // Asegura tbMedidas
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
                        if (pedidoExistente == null) return false;

                        pedidoExistente.codMesa = model.CodMesa;
                        pedidoExistente.idEstadoPedido = model.IdEstadoPedido;
                        pedidoExistente.total = model.Detalles.Sum(d => d.Subtotal); // Recalcular total

                        var detallesAntiguos = _dbContext.tbDetallePedido.Where(d => d.idPedido == model.IdPedido);
                        _dbContext.tbDetallePedido.RemoveRange(detallesAntiguos);
                        _dbContext.SaveChanges(); // Aplicar eliminación antes de agregar nuevos para evitar conflictos de PK si se reusan IDs (aunque no debería ser el caso aquí)

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

        // Nueva función para buscar Agregados
        public List<tbAgregados> BuscarAgregados(string criterio)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    // Podrías devolver todos si el criterio es vacío, o una lista vacía.
                    // Por coherencia con el buscador de productos, devolvemos los más populares o una lista vacía.
                    // O simplemente todos los agregados activos.
                    return _dbContext.tbAgregados.Take(10).ToList(); // O todos: .ToList();
                }
                string criterioLower = criterio.ToLower();
                return _dbContext.tbAgregados
                                .Where(a => a.nombre.ToLower().Contains(criterioLower) ||
                                            (a.descripcion != null && a.descripcion.ToLower().Contains(criterioLower)))
                                .Take(10) // Limitar resultados
                                .ToList();
            }
        }
        // Función para obtener todos los agregados (puede ser útil para un dropdown inicial o si el buscador está vacío)
        public List<tbAgregados> ListarTodosAgregados()
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                try
                {
                    var agregados = _dbContext.tbAgregados.ToList();
                    // Punto de depuración aquí: verifica que 'agregados' contiene los datos esperados
                    // y que los campos 'idAgregado', 'nombre' y 'precio' tienen valores.
                    return agregados;
                }
                catch (Exception ex)
                {
                    // Loggear este error es importante si ocurre
                    System.Diagnostics.Trace.WriteLine("Error en PedidoService.ListarTodosAgregados: " + ex.ToString());
                    return new List<tbAgregados>(); // Devolver lista vacía en caso de error
                }
            }
        }
    }
}
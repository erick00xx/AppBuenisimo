// SistemaWebEficienciaOperativa.Services/PedidoService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Models.ViewModels; // Asegúrate que esta ruta es correcta
using System.Data.Entity; // Para EntityState.Modified

namespace SistemaWebEficienciaOperativa.Services
{
    public class PedidoService
    {
        private readonly DB_BUENISIMOEntities _dbContext;
        private readonly MesaService _mesaService; // Inyectar MesaService

        public PedidoService()
        {
            _dbContext = new DB_BUENISIMOEntities();
            _mesaService = new MesaService(); // Instanciar MesaService
        }

        public List<ProductoPrecioDTO> BuscarProductos(string terminoBusqueda)
        {
            // ... (código existente)
            var Fecha = DateTime.Now.ToString("dd/MM/yyyy");
            var Hora = DateTime.Now.ToString("HH:mm:ss");
            return _dbContext.tbPrecios
                .Where(p => p.tbProductos.nombre.Contains(terminoBusqueda) && p.tbProductos.estado == "activo")
                .Select(p => new ProductoPrecioDTO
                {
                    Id = p.IdPrecio, // Este es IdPrecio
                    Producto = p.tbProductos.nombre,
                    TipoProducto = p.tbProductos.tbCategorias.tbTiposProductos.nombre,
                    Categoria = p.tbProductos.tbCategorias.nombre,
                    TipoMedida = p.tbProductos.tbTiposMedidas.nombre, // Puede ser null
                    Medida = p.tbMedidas.nombre, // Puede ser null
                    Precio = p.Precio,
                    // Usuario, Fecha, Hora probablemente no son necesarios en el DTO para la búsqueda
                })
                .ToList();
        }

        public int GuardarNuevoPedido(string detallePedidoJson, int idUsuario, int idMesa)
        {
            var productosSeleccionados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DetallePedidoViewModel>>(detallePedidoJson);

            if (productosSeleccionados == null || !productosSeleccionados.Any())
            {
                throw new ArgumentException("El detalle del pedido no puede estar vacío.");
            }

            // Obtener el id del estado "En espera"
            var estadoEnEspera = _dbContext.tbEstadosPedidos.FirstOrDefault(e => e.estado == "En espera");
            if (estadoEnEspera == null)
            {
                // Considerar crear el estado si no existe o lanzar un error más específico
                throw new InvalidOperationException("El estado de pedido 'En espera' no está configurado en la base de datos.");
            }

            var nuevoPedido = new tbPedidos
            {
                idMesa = idMesa,
                fechaPedido = DateTime.Now,
                idEstadoPedido = estadoEnEspera.idEstadoPedido,
                idUsuario = idUsuario,
                total = 0 // Se calculará después
            };

            _dbContext.tbPedidos.Add(nuevoPedido);
            _dbContext.SaveChanges(); // Guardar para obtener el nuevoPedido.idPedido

            decimal totalPedido = 0;

            foreach (var productoVM in productosSeleccionados)
            {
                var precioObj = _dbContext.tbPrecios.Find(productoVM.idPrecio);
                if (precioObj == null)
                {
                    // Rollback o manejo de error si un producto no se encuentra
                    // Por simplicidad, lanzamos excepción. En un sistema real, podrías querer una transacción.
                    throw new KeyNotFoundException($"No se encontró el precio con ID {productoVM.idPrecio}.");
                }

                var subtotal = productoVM.cantidad * precioObj.Precio;
                var detalleDb = new tbDetallePedido
                {
                    idPedido = nuevoPedido.idPedido,
                    idPrecio = productoVM.idPrecio,
                    cantidad = productoVM.cantidad,
                    subtotal = subtotal
                };
                _dbContext.tbDetallePedido.Add(detalleDb);
                totalPedido += subtotal;
            }

            nuevoPedido.total = totalPedido;
            // _dbContext.Entry(nuevoPedido).State = EntityState.Modified; // No es necesario si el objeto ya está rastreado y se llama SaveChanges
            _dbContext.SaveChanges(); // Guardar el total y los detalles

            // Actualizar estado de la mesa
            _mesaService.ActualizarEstadoMesa(idMesa, "Ocupada");

            return nuevoPedido.idPedido;
        }

        public PedidoCompletoViewModel ObtenerPedidoCompleto(int idPedido)
        {
            var pedidoDb = _dbContext.tbPedidos
                .Include(p => p.tbMesas)
                .Include(p => p.tbUsuarios)
                .Include(p => p.tbEstadosPedidos)
                .Include(p => p.tbDetallePedido.Select(d => d.tbPrecios.tbProductos.tbCategorias.tbTiposProductos)) // Para tipo producto
                .Include(p => p.tbDetallePedido.Select(d => d.tbPrecios.tbMedidas)) // Para medida
                .AsNoTracking() // Bueno para vistas de solo lectura o cuando se gestionará el estado manualmente
                .FirstOrDefault(p => p.idPedido == idPedido);

            if (pedidoDb == null) return null;

            return new PedidoCompletoViewModel
            {
                IdPedido = pedidoDb.idPedido,
                IdMesa = pedidoDb.idMesa,
                NumeroMesa = pedidoDb.tbMesas?.numeroMesa ?? 0,
                NombreUsuario = $"{pedidoDb.tbUsuarios?.nombre} {pedidoDb.tbUsuarios?.apellido}".Trim(),
                FechaPedido = pedidoDb.fechaPedido ?? DateTime.MinValue,
                IdEstadoPedido = pedidoDb.idEstadoPedido ?? 0,
                EstadoPedido = pedidoDb.tbEstadosPedidos?.estado,
                TotalPedido = pedidoDb.total ?? 0,
                Items = pedidoDb.tbDetallePedido.Select(d => new DetallePedidoItemViewModel
                {
                    IdDetalle = d.idDetalle,
                    IdPrecio = d.idPrecio ?? 0,
                    NombreProducto = d.tbPrecios?.tbProductos?.nombre,
                    Medida = d.tbPrecios?.tbMedidas?.nombre, // Puede ser null
                    Cantidad = d.cantidad ?? 0,
                    PrecioUnitario = d.tbPrecios?.Precio ?? 0,
                    Subtotal = d.subtotal ?? 0
                }).ToList()
            };
        }

        /// <summary>
        /// Obtiene el pedido activo (no Entregado/Cancelado) más reciente de una mesa.
        /// </summary>
        public tbPedidos ObtenerPedidoActivoPorMesa(int idMesa)
        {
            return _dbContext.tbPedidos
                .Include(p => p.tbEstadosPedidos)
                .Where(p => p.idMesa == idMesa &&
                             p.tbEstadosPedidos.estado != "Entregado" &&
                             p.tbEstadosPedidos.estado != "Cancelado")
                .OrderByDescending(p => p.fechaPedido)
                .FirstOrDefault();
        }

        public List<tbEstadosPedidos> ObtenerTodosLosEstadosPedido()
        {
            return _dbContext.tbEstadosPedidos.OrderBy(e => e.idEstadoPedido).ToList();
        }

        public bool ActualizarEstadoPedido(int idPedido, int idNuevoEstado, int idUsuarioQueActualiza)
        {
            var pedido = _dbContext.tbPedidos.Include(p => p.tbMesas).FirstOrDefault(p => p.idPedido == idPedido);
            if (pedido == null) return false;

            var estadoAnteriorId = pedido.idEstadoPedido;
            pedido.idEstadoPedido = idNuevoEstado;
            // Opcional: pedido.idUsuarioUltimaModificacion = idUsuarioQueActualiza;
            // Opcional: pedido.fechaUltimaModificacion = DateTime.Now;
            _dbContext.Entry(pedido).State = EntityState.Modified;
            _dbContext.SaveChanges();

            // Si el nuevo estado es "Entregado" o "Cancelado", verificar si la mesa debe pasar a "Disponible"
            var nuevoEstadoObj = _dbContext.tbEstadosPedidos.Find(idNuevoEstado);
            if (nuevoEstadoObj != null && (nuevoEstadoObj.estado == "Entregado" || nuevoEstadoObj.estado == "Cancelado"))
            {
                if (pedido.idMesa.HasValue)
                {
                    // Verificar si hay OTROS pedidos activos para esta mesa
                    bool hayOtrosPedidosActivos = _dbContext.tbPedidos
                        .Any(p => p.idMesa == pedido.idMesa &&
                                    p.idPedido != idPedido && // Excluir el actual
                                    p.tbEstadosPedidos.estado != "Entregado" &&
                                    p.tbEstadosPedidos.estado != "Cancelado");

                    if (!hayOtrosPedidosActivos)
                    {
                        _mesaService.ActualizarEstadoMesa(pedido.idMesa.Value, "Disponible");
                    }
                }
            }
            // Si se pasó de Entregado/Cancelado a un estado activo, y la mesa estaba Disponible, marcarla Ocupada
            else if (pedido.idMesa.HasValue)
            {
                var estadoMesaActual = _mesaService.ObtenerMesaPorId(pedido.idMesa.Value)?.estado;
                if (estadoMesaActual == "Disponible")
                {
                    _mesaService.ActualizarEstadoMesa(pedido.idMesa.Value, "Ocupada");
                }
            }


            return true;
        }

        public void ActualizarPedidoCompleto(PedidoCompletoViewModel pedidoVM, int idUsuarioQueActualiza)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var pedidoDb = _dbContext.tbPedidos
                                        .Include(p => p.tbDetallePedido) // Cargar detalles para poder modificarlos
                                        .FirstOrDefault(p => p.idPedido == pedidoVM.IdPedido);

                    if (pedidoDb == null) throw new KeyNotFoundException("Pedido no encontrado para actualizar.");

                    // 1. Actualizar estado del pedido (si es parte del VM y cambió)
                    if (pedidoDb.idEstadoPedido != pedidoVM.IdEstadoPedido)
                    {
                        ActualizarEstadoPedido(pedidoVM.IdPedido, pedidoVM.IdEstadoPedido, idUsuarioQueActualiza);
                        // Recargar pedidoDb para reflejar el cambio de estado antes de procesar items
                        // o asumir que ActualizarEstadoPedido no afecta los items directamente.
                        // Por simplicidad, lo dejamos así, pero en escenarios complejos, se debe tener cuidado.
                    }

                    // 2. Sincronizar detalles del pedido
                    var idsDetalleEnVM = pedidoVM.Items.Where(i => i.IdDetalle > 0).Select(i => i.IdDetalle).ToList();

                    // Eliminar detalles que ya no están en el ViewModel
                    var detallesParaEliminar = pedidoDb.tbDetallePedido
                                                    .Where(dDb => !idsDetalleEnVM.Contains(dDb.idDetalle))
                                                    .ToList();
                    foreach (var detalleObsoleto in detallesParaEliminar)
                    {
                        _dbContext.tbDetallePedido.Remove(detalleObsoleto);
                    }

                    decimal nuevoTotalPedido = 0;

                    // Actualizar detalles existentes y añadir nuevos
                    foreach (var itemVM in pedidoVM.Items)
                    {
                        var precioObj = _dbContext.tbPrecios.Find(itemVM.IdPrecio);
                        if (precioObj == null) throw new KeyNotFoundException($"Precio ID {itemVM.IdPrecio} no encontrado para el producto '{itemVM.NombreProducto}'.");

                        var subtotalCalculado = itemVM.Cantidad * precioObj.Precio;

                        if (itemVM.IdDetalle > 0) // Detalle existente
                        {
                            var detalleDbExistente = pedidoDb.tbDetallePedido.FirstOrDefault(d => d.idDetalle == itemVM.IdDetalle);
                            if (detalleDbExistente != null)
                            {
                                detalleDbExistente.cantidad = itemVM.Cantidad;
                                detalleDbExistente.subtotal = subtotalCalculado;
                                // No se debería cambiar idPrecio de un item existente, si cambia, es un item nuevo y el viejo se borra.
                                // O se maneja como un reemplazo.
                            }
                            // Si no se encuentra (raro, pero posible si se manipuló el form), se trataría como nuevo o error.
                        }
                        else // Nuevo detalle (IdDetalle es 0 o no existe)
                        {
                            var nuevoDetalleDb = new tbDetallePedido
                            {
                                idPedido = pedidoDb.idPedido,
                                idPrecio = itemVM.IdPrecio,
                                cantidad = itemVM.Cantidad,
                                subtotal = subtotalCalculado
                            };
                            _dbContext.tbDetallePedido.Add(nuevoDetalleDb);
                        }
                        nuevoTotalPedido += subtotalCalculado;
                    }

                    pedidoDb.total = nuevoTotalPedido;
                    // pedidoDb.idUsuarioUltimaModificacion = idUsuarioQueActualiza;
                    // pedidoDb.fechaUltimaModificacion = DateTime.Now;

                    _dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; // Re-lanzar la excepción para que el controlador la maneje
                }
            }
        }

        public List<PedidoCompletoViewModel> ObtenerPedidosParaPanelCocina()
        {
            var estadosRelevantes = new List<string> { "En espera", "En preparación" };

            var pedidosDb = _dbContext.tbPedidos
                .Include(p => p.tbMesas)
                .Include(p => p.tbUsuarios)
                .Include(p => p.tbEstadosPedidos)
                .Include(p => p.tbDetallePedido.Select(d => d.tbPrecios.tbProductos))
                .Include(p => p.tbDetallePedido.Select(d => d.tbPrecios.tbMedidas))
                .Where(p => estadosRelevantes.Contains(p.tbEstadosPedidos.estado))
                .OrderBy(p => p.fechaPedido) // Los más antiguos primero
                .AsNoTracking()
                .ToList();

            return pedidosDb.Select(pedidoDb => new PedidoCompletoViewModel // Reutilizar el ViewModel
            {
                IdPedido = pedidoDb.idPedido,
                IdMesa = pedidoDb.idMesa,
                NumeroMesa = pedidoDb.tbMesas?.numeroMesa ?? 0,
                FechaPedido = pedidoDb.fechaPedido ?? DateTime.MinValue,
                EstadoPedido = pedidoDb.tbEstadosPedidos?.estado,
                IdEstadoPedido = pedidoDb.idEstadoPedido ?? 0,
                // NombreUsuario no es tan crucial para cocina, pero puede ser útil
                NombreUsuario = $"{pedidoDb.tbUsuarios?.nombre} {pedidoDb.tbUsuarios?.apellido}".Trim(),
                Items = pedidoDb.tbDetallePedido.Select(d => new DetallePedidoItemViewModel
                {
                    IdDetalle = d.idDetalle, // No tan útil para cocina, pero lo tenemos
                    IdPrecio = d.idPrecio ?? 0,
                    NombreProducto = d.tbPrecios?.tbProductos?.nombre,
                    Medida = d.tbPrecios?.tbMedidas?.nombre,
                    Cantidad = d.cantidad ?? 0,
                    // PrecioUnitario y Subtotal no son cruciales para cocina
                }).ToList()
            }).ToList();
        }
    }
}
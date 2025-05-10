using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Models.ViewModels;

namespace SistemaWebEficienciaOperativa.Services
{
    public class PedidoService
    {
        private readonly DB_BUENISIMOEntities _dbContext;
        public PedidoService()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }

        public List<ProductoPrecioDTO> BuscarProductos(string terminoBusqueda)
        {
            var Fecha = DateTime.Now.ToString("dd/MM/yyyy");
            var Hora = DateTime.Now.ToString("HH:mm:ss");
            return _dbContext.tbPrecios
                .Where(p => p.tbProductos.nombre.Contains(terminoBusqueda))
                .Select(p => new ProductoPrecioDTO
                {
                    Id = p.IdPrecio,
                    Producto = p.tbProductos.nombre,
                    TipoProducto = p.tbProductos.tbCategorias.tbTiposProductos.nombre,
                    Categoria = p.tbProductos.tbCategorias.nombre,
                    TipoMedida = p.tbProductos.tbTiposMedidas.nombre,
                    Medida = p.tbMedidas.nombre,
                    Precio = p.Precio,
                    Usuario = 1,
                    Fecha = Fecha,
                    Hora = Hora

                })
                .ToList();
        }

        public void GuardarPedido(string detallePedido)
        {

            // Convertir el JSON recibido en una lista de objetos
            var productosSeleccionados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DetallePedidoViewModel>>(detallePedido);

            // Crear un nuevo pedido
            var nuevoPedido = new tbPedidos
            {
                idMesa = 1,  // Aquí deberías obtener el ID de la mesa, dependiendo de tu flujo
                fechaPedido = DateTime.Now,
                estado = "En espera",
                usuarioId = 1,  // Aquí deberías obtener el ID del usuario que realiza el pedido
                total = 0  // Este valor se calculará después
            };

            _dbContext.tbPedidos.Add(nuevoPedido);
            _dbContext.SaveChanges();

            // Insertar los detalles del pedido
            decimal totalPedido = 0;

            foreach (var producto in productosSeleccionados)
            {
                // Obtener el precio del producto
                var precioProducto = _dbContext.tbPrecios.FirstOrDefault(p => p.IdPrecio == producto.idPrecio)?.Precio ?? 0;

                // Calcular el subtotal
                var subtotal = producto.cantidad * precioProducto;

                // Crear el detalle del pedido
                var detallePedidoDb = new tbDetallePedido
                {
                    idPedido = nuevoPedido.idPedido,
                    idPrecio = producto.idPrecio,
                    cantidad = producto.cantidad,
                    subtotal = subtotal // Asignar el subtotal calculado
                };

                // Sumar al total del pedido
                totalPedido += subtotal;

                // Añadir el detalle del pedido a la base de datos
                _dbContext.tbDetallePedido.Add(detallePedidoDb);
            }

            // Actualizar el total del pedido
            nuevoPedido.total = totalPedido;

            _dbContext.SaveChanges();
        }

    }
}
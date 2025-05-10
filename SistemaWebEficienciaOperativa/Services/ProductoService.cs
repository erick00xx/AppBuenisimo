using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Models.ViewModels;

namespace SistemaWebEficienciaOperativa.Services
{
    public class ProductoServices
    {
        private readonly DB_BUENISIMOEntities _dbContext;

        public ProductoServices()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }
        // SESCCION DE AGREGAR PRODUCTOS NUEVOS
        public List<tbTiposProductos> TiposProductos()
        {
            return _dbContext.tbTiposProductos.ToList();
        }
        public List<tbCategorias> CategoriasPorTipo(int idTipoProducto)
        {
            return _dbContext.tbCategorias
                .Where(c => c.idTipoProducto == idTipoProducto).ToList();
        }
        public List<tbTiposMedidas> TiposMedidas()
        {
            return _dbContext.tbTiposMedidas.ToList();
        }
        public void GuardarProducto(tbProductos producto)
        {
            _dbContext.tbProductos.Add(producto);
            _dbContext.SaveChanges();
        }

        //SECCION LISTAR PRODUCTOS
        public List<ProductoDTO> ListarProductos()
        {
            var productos = _dbContext.tbProductos
                .Select(p => new ProductoDTO
                {
                    IdProducto = p.idProducto,
                    NombreProducto = p.nombre,
                    Descripcion = string.IsNullOrEmpty(p.descripcion) ? "Producto de café garantizado por la marca de Café Buenísimo" : p.descripcion,
                    CategoriaNombre = p.tbCategorias.nombre,
                    TipoProductoNombre = p.tbCategorias.tbTiposProductos.nombre,
                    TipoMedidaNombre = p.tbTiposMedidas.nombre,
                    Estado = p.estado
                }).ToList();

            return productos;
        }

        public bool OcultarProducto(int id)
        {
            var producto = _dbContext.tbProductos.FirstOrDefault(p => p.idProducto == id);
            if (producto != null)
            {
                producto.estado = "inactivo";
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool MostrarProducto(int id)
        {
            var producto = _dbContext.tbProductos.FirstOrDefault(p => p.idProducto == id);
            if (producto != null)
            {
                producto.estado = "activo";
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
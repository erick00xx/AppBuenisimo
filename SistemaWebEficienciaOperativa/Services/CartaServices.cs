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
                    Descripcion = "Producto verificado por Buenísimo",
                    CategoriaNombre = p.tbCategorias.nombre,
                    TipoProductoNombre = p.tbCategorias.tbTiposProductos.nombre,
                    TipoMedidaNombre = p.tbTiposMedidas.nombre,
                }).ToList();

            return productos;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using OfficeOpenXml;
using System.IO;
using System.ComponentModel;
using LicenseContextOfficeOpenXml = OfficeOpenXml.LicenseContext;
using LicenseContextSystem = System.ComponentModel.LicenseContext;


namespace AppBuenisimo.Services
{
    public class ProductoServices
    {
        private readonly DB_BUENISIMOEntities _dbContext;

        public ProductoServices()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }

        // SESION DE AGREGAR PRODUCTOS NUEVOS
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

        // SECCIÓN LISTAR PRODUCTOS
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

        // NUEVO: REGISTRO MASIVO DE PRODUCTOS DESDE EXCEL
        public List<string> CargarProductosDesdeExcel(HttpPostedFileBase archivo)
        {
            ExcelPackage.LicenseContext = LicenseContextOfficeOpenXml.NonCommercial;
            List<string> errores = new List<string>();

            if (archivo != null && archivo.ContentLength > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(archivo.InputStream))
                    {
                        var hoja = package.Workbook.Worksheets.First();
                        int fila = 2;

                        while (hoja.Cells[fila, 1].Value != null)
                        {
                            try
                            {
                                string nombreInsumo = hoja.Cells[fila, 1].Text.Trim();
                                string nombreUnidad = hoja.Cells[fila, 2].Text.Trim();
                                string cantidadTexto = hoja.Cells[fila, 3].Text.Trim();
                                string nombreSucursal = hoja.Cells[fila, 4].Text.Trim();
                                string fechaCompraTexto = hoja.Cells[fila, 5].Text.Trim();
                                string fechaVencimientoTexto = hoja.Cells[fila, 6].Text.Trim();
                                string nombreProveedor = hoja.Cells[fila, 7].Text.Trim();
                                string lote = hoja.Cells[fila, 8].Text.Trim();
                                string observaciones = hoja.Cells[fila, 9].Text.Trim();

                                var insumo = _dbContext.tbInsumos.FirstOrDefault(i => i.nombre == nombreInsumo);
                                var unidad = _dbContext.tbUnidades.FirstOrDefault(u => u.nombre == nombreUnidad || u.abreviatura == nombreUnidad);
                                var sucursal = _dbContext.tbSucursales.FirstOrDefault(s => s.nombre == nombreSucursal);
                                var proveedor = string.IsNullOrWhiteSpace(nombreProveedor) ? null : _dbContext.tbProveedores.FirstOrDefault(p => p.nombreEmpresa == nombreProveedor);

                                if (insumo == null || unidad == null || sucursal == null)
                                {
                                    errores.Add($"Fila {fila}: Insumo, unidad o sucursal no válidos.");
                                    fila++;
                                    continue;
                                }

                                if (!decimal.TryParse(cantidadTexto, out decimal cantidad))
                                {
                                    errores.Add($"Fila {fila}: Cantidad inválida.");
                                    fila++;
                                    continue;
                                }

                                if (!DateTime.TryParse(fechaCompraTexto, out DateTime fechaCompra))
                                {
                                    errores.Add($"Fila {fila}: Fecha de compra inválida.");
                                    fila++;
                                    continue;
                                }

                                if (!DateTime.TryParse(fechaVencimientoTexto, out DateTime fechaVencimiento))
                                {
                                    errores.Add($"Fila {fila}: Fecha de vencimiento inválida.");
                                    fila++;
                                    continue;
                                }

                                var ingreso = new tbIngresosInsumos
                                {
                                    idInsumo = insumo.idInsumo,
                                    idUnidad = unidad.idUnidad,
                                    cantidad = cantidad,
                                    idSucursal = sucursal.idSucursal,
                                    fechaCompra = fechaCompra,
                                    fechaVencimiento = fechaVencimiento,
                                    idProveedor = proveedor?.idProveedor,
                                    lote = string.IsNullOrWhiteSpace(lote) ? null : lote,
                                    observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones
                                };

                                _dbContext.tbIngresosInsumos.Add(ingreso);
                            }
                            catch (Exception exFila)
                            {
                                errores.Add($"Fila {fila}: {exFila.Message}");
                            }

                            fila++;
                        }

                        _dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    errores.Add("Error al procesar el archivo: " + ex.Message);
                }
            }
            else
            {
                errores.Add("Archivo no válido o vacío.");
            }

            return errores;
        }

    }
}

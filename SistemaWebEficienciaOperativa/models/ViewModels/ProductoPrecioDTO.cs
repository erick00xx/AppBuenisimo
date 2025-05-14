using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// SistemaWebEficienciaOperativa.Models.ViewModels/ProductoPrecioDTO.cs
namespace SistemaWebEficienciaOperativa.Models.ViewModels
{
    public class ProductoPrecioDTO
    {
        public int Id { get; set; } // Corresponde a IdPrecio
        public string Producto { get; set; }
        public string TipoProducto { get; set; }
        public string Categoria { get; set; }
        public string TipoMedida { get; set; }
        public string Medida { get; set; }
        public decimal Precio { get; set; }
        // Usuario, Fecha, Hora, si no se usan en la UI de búsqueda, se pueden quitar para aligerar.
    }
}
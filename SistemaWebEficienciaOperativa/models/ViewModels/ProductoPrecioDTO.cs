using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models.ViewModels
{
    public class ProductoPrecioDTO
    {
        public int Id { get; set; }
        public string Producto { get; set; }
        public string TipoProducto { get; set; }
        public string Categoria { get; set; }
        public string TipoMedida { get; set; }
        public string Medida { get; set; }
        public decimal Precio { get; set; }
        public int Usuario { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaWebEficienciaOperativa.Models.ViewModels
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public string CategoriaNombre { get; set; }
        public string TipoProductoNombre { get; set; }
        public string TipoMedidaNombre { get; set; }
    }
}
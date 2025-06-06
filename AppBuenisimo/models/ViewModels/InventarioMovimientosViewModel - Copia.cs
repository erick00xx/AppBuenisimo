using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebGrease.Css.Ast.Selectors;
namespace AppBuenisimo.Models
{
    public class CompraViewModel
    {
        public int Id { get; set; }
        public string Insumo { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Proveedor { get; set; }
        public string Sucursal { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class DesechoViewModel
    {
        public int Id { get; set; }
        public string Insumo { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Motivo { get; set; }
        public string Sucursal { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class InventarioMovimientosViewModel
    {
        public List<CompraViewModel> UltimasCompras { get; set; }
        public List<DesechoViewModel> UltimosDesechos { get; set; }
    }
}
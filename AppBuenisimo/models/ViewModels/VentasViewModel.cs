using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppBuenisimo.Models.ViewModels
{
    // VentaIndexViewModel.cs
    // Para mostrar la lista de ventas de forma eficiente.
    public class VentaIndexViewModel
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreSucursal { get; set; }
        public bool FueModificada { get; set; }
    }

    // VentaDetalleViewModel.cs
    // Para ver los detalles completos y para editar la venta.
    public class VentaDetalleViewModel
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public string CodMesa { get; set; }
        public string NombreUsuarioVenta { get; set; }
        public string NombreSucursal { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        public string MetodoPago { get; set; }

        // Lista de productos "congelados" en el momento de la venta
        public List<tbDetalleVenta> DetallesVenta { get; set; }

        // Campos de Auditoría
        public bool FueModificada { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string NombreUsuarioModificacion { get; set; }

        [Required(ErrorMessage = "Debe proporcionar un motivo para la modificación.")]
        public string MotivoModificacion { get; set; }

        public VentaDetalleViewModel()
        {
            DetallesVenta = new List<tbDetalleVenta>();
        }
    }
}
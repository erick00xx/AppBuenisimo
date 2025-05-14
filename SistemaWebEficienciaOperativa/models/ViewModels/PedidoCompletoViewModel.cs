// SistemaWebEficienciaOperativa.Models.ViewModels/PedidoCompletoViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para DataAnnotations si las usas

namespace SistemaWebEficienciaOperativa.Models.ViewModels
{
    public class PedidoCompletoViewModel
    {
        public int IdPedido { get; set; }
        public int? IdMesa { get; set; } // Hacerlo nullable si un pedido podría no tener mesa (ej. para llevar)
        public int NumeroMesa { get; set; } // Para mostrar en la vista
        public string NombreUsuario { get; set; }
        public DateTime FechaPedido { get; set; }

        [Display(Name = "Estado del Pedido")]
        public int IdEstadoPedido { get; set; }
        public string EstadoPedido { get; set; } // Para mostrar
        public decimal TotalPedido { get; set; }
        public List<DetallePedidoItemViewModel> Items { get; set; }

        public PedidoCompletoViewModel()
        {
            Items = new List<DetallePedidoItemViewModel>();
        }
    }

    public class DetallePedidoItemViewModel
    {
        public int IdDetalle { get; set; } // 0 si es un ítem nuevo al editar
        [Required]
        public int IdPrecio { get; set; }
        public string NombreProducto { get; set; } // Para mostrar
        public string Medida { get; set; } // Para mostrar
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } // Para mostrar
        public decimal Subtotal { get; set; } // Para mostrar
    }
}
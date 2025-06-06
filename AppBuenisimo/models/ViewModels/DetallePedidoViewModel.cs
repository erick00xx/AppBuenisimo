// Models/ViewModels/DetallePedidoViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AppBuenisimo.Models.ViewModels
{
    public class DetallePedidoViewModel // Este es el que usa CrearPedidoInputViewModel
    {
        public int IdPrecio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; } // El subtotal ahora incluirá el precio de los agregados

        // Nuevos campos
        public string TipoLeche { get; set; }
        public string TipoAzucar { get; set; }
        public string CantidadHielo { get; set; }
        public int? IdAgregado1 { get; set; }
        public int? IdAgregado2 { get; set; }
        public int? IdAgregado3 { get; set; }
    }
}

// Models/ViewModels/NuevoPedidoViewModel.cs (Opcional, puedes usar ViewBag para las mesas)
// Si decides usarlo, el controlador lo poblaría.
namespace AppBuenisimo.Models.ViewModels
{
    public class NuevoPedidoPageViewModel // Renombrado para evitar confusión con el que se envía
    {
        public List<System.Web.Mvc.SelectListItem> MesasDisponibles { get; set; }
        // Podrías añadir más data necesaria para la página aquí
    }
}

// Models/ViewModels/CrearPedidoInputViewModel.cs (Para recibir los datos del POST)
namespace AppBuenisimo.Models.ViewModels
{
    public class CrearPedidoInputViewModel
    {
        public string CodMesa { get; set; }
        public List<DetallePedidoViewModel> Detalles { get; set; }
    }
}

//////
namespace AppBuenisimo.Models.ViewModels
{
    public class DetallePedidoPageViewModel
    {
        public tbPedidos Pedido { get; set; }
        // Usamos el DetallePedidoViewModel para los ítems,
        // ya que contiene la información formateada que necesitamos en la vista.
        public List<DetallePedidoViewModel> DetallesRenderizar { get; set; }
        public List<SelectListItem> EstadosPedidoPosibles { get; set; }
        // Podrías añadir idSucursal aquí si necesitas filtrar la búsqueda de nuevos productos
        // public int IdSucursal {get; set;}
    }

    //public class ActualizarPedidoInputViewModel
    //{
    //    [Required]
    //    public int IdPedido { get; set; }

    //    [Required(ErrorMessage = "Debe seleccionar una mesa.")]
    //    public string CodMesa { get; set; } // Podrías querer cambiar la mesa

    //    [Required(ErrorMessage = "Debe seleccionar un estado para el pedido.")]
    //    public int IdEstadoPedido { get; set; }

    //    public List<DetallePedidoViewModel> Detalles { get; set; } // Reutilizamos el ViewModel existente

    //    public ActualizarPedidoInputViewModel()
    //    {
    //        Detalles = new List<DetallePedidoViewModel>();
    //    }
    //}
    public class ActualizarPedidoInputViewModel
    {
        public int IdPedido { get; set; }
        public string CodMesa { get; set; }
        public int IdEstadoPedido { get; set; }
        public List<DetallePedidoViewModel> Detalles { get; set; }
    }
}


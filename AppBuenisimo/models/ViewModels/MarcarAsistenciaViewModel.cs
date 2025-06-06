// En tu carpeta ViewModels
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AppBuenisimo.Services;
using AppBuenisimo.Utils; // Para SelectList

namespace AppBuenisimo.Models.ViewModels
{
    public class MarcarAsistenciaViewModel
    {
        public int IdUsuario { get; set; } // Se obtendrá del usuario logueado
        public string NombreUsuario { get; set; }
        public DateTime FechaActual { get; set; } = TimeProvider.Today;
        public string HoraActual { get { return TimeProvider.Now.ToString("HH:mm:ss"); } }

        public bool PuedeMarcarEntrada { get; set; }
        public string MensajeEntrada { get; set; } // Ej: "Puedes marcar tu entrada", "Aún no es hora", "Tienes X minutos de tardanza"
        public int MinutosTardanzaEstimados { get; set; }

        public bool PuedeMarcarSalida { get; set; }
        public string MensajeSalida { get; set; } // Ej: "Puedes marcar tu salida", "¿Seguro que quieres salir tan temprano?"
        public bool RequiereConfirmacionSalidaTemprana { get; set; }

        public bool YaMarcoEntradaHoy { get; set; }
        public bool YaMarcoSalidaHoy { get; set; }
        public string HoraEntradaMarcada { get; set; }
        public string HoraSalidaMarcada { get; set; }

        public string HorarioEsperadoHoy { get; set; } // Ej: "09:00 - 18:00"

        // Para el POST
        [Required(ErrorMessage = "Debe seleccionar una sucursal.")]
        [Display(Name = "Sucursal de Marcación")]
        public int IdSucursalSeleccionada { get; set; }
        public SelectList SucursalesDisponibles { get; set; }

        public bool ConfirmarSalidaTemprana { get; set; } // Checkbox para confirmar
    }

    // (Opcional) Si necesitas mostrar un historial de asistencias
    public class HistorialAsistenciaViewModel
    {
        public DateTime Fecha { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }
        public string Observacion { get; set; }
        public int MinutosTardanza { get; set; }
        public string Sucursal { get; set; }
    }
}
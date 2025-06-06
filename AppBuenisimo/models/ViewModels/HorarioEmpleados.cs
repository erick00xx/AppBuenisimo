// En una carpeta ViewModels
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AppBuenisimo.Utils; // Para SelectList

namespace AppBuenisimo.Models.ViewModels
{
    // ViewModel para mostrar la lista de usuarios y un enlace a sus horarios
    public class GestionEmpleadoIndexViewModel
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string CorreoElectronico { get; set; }
        public string Dni { get; set; }
        public string Rol { get; set; }
    }

    // ViewModel para la página de gestión de horarios de un usuario específico
    public class UsuarioHorariosViewModel
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public List<HorarioDetalleViewModel> HorariosAsignados { get; set; }
        public HorarioFormViewModel NuevoHorario { get; set; } // Para el formulario de agregar
    }

    // ViewModel para mostrar detalles de un horario específico
    public class HorarioDetalleViewModel
    {
        public int IdHorario { get; set; }
        public string DiaSemanaTexto { get; set; } // Lunes, Martes, etc.
        public byte DiaSemanaValor { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan HoraEntrada { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan HoraSalida { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PagoPorHora { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInicioVigencia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? FechaFinVigencia { get; set; }
        public bool Activo { get; set; }
    }


    // ViewModel para el formulario de creación/edición de horarios
    public class HorarioFormViewModel
    {
        public int IdHorario { get; set; } // 0 para nuevo

        [Required]
        public int IdUsuario { get; set; } // Se asignará automáticamente

        // Esta propiedad se usará internamente al pasar al servicio para un solo día.
        // No necesita atributos de validación aquí si DiasSeleccionados se valida en el controlador.
        public byte DiaSemana { get; set; } // 0-6, para la lógica de guardado de un día individual

        [Display(Name = "Días de la Semana")]
        // La validación para asegurar que al menos un día es seleccionado se hará en el controlador.
        public List<string> DiasSeleccionados { get; set; } // Para los checkboxes. Los valores serán "0", "1", "2", etc.

        [Required(ErrorMessage = "La hora de entrada es obligatoria.")]
        [Display(Name = "Hora de Entrada")]
        [DataType(DataType.Time)]
        public TimeSpan HoraEntrada { get; set; }

        [Required(ErrorMessage = "La hora de salida es obligatoria.")]
        [Display(Name = "Hora de Salida")]
        [DataType(DataType.Time)]
        public TimeSpan HoraSalida { get; set; }

        [Required(ErrorMessage = "El pago por hora es obligatorio.")]
        [Display(Name = "Pago por Hora")]
        [Range(0.01, 10000.00, ErrorMessage = "El pago por hora debe ser un valor positivo.")]
        [DataType(DataType.Currency)]
        public decimal PagoPorHora { get; set; }

        [Required(ErrorMessage = "La fecha de inicio de vigencia es obligatoria.")]
        [Display(Name = "Inicio Vigencia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicioVigencia { get; set; } = TimeProvider.Today; // Asegúrate que TimeProvider.Today está disponible y configurado. Si no, usa DateTime.Today

        [Display(Name = "Fin Vigencia (Opcional)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFinVigencia { get; set; }

        public bool Activo { get; set; } = true;

        public SelectList DiasSemanaOptions { get; private set; } // Hacerlo private set para que solo se inicialice en el constructor
        public string NombreUsuario { get; set; } // Para mostrar en el formulario a quién se asigna

        public HorarioFormViewModel()
        {
            DiasSeleccionados = new List<string>(); // Inicializar la lista
            FechaInicioVigencia = TimeProvider.Today; // O DateTime.Today
            Activo = true;
            DiasSemanaOptions = new SelectList(
                new[]
                {
                    new { Value = "1", Text = "Lunes" },
                    new { Value = "2", Text = "Martes" },
                    new { Value = "3", Text = "Miércoles" },
                    new { Value = "4", Text = "Jueves" },
                    new { Value = "5", Text = "Viernes" },
                    new { Value = "6", Text = "Sábado" },
                    new { Value = "0", Text = "Domingo" }, // Asegúrate que el valor 0 para Domingo es consistente con tu DB y lógica.
                }, "Value", "Text");
        }
    }
}
using SistemaWebEficienciaOperativa.Models.ViewModels;
using SistemaWebEficienciaOperativa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaWebEficienciaOperativa.Utils;

namespace SistemaWebEficienciaOperativa.Services
{
    public class EmpleadoService
    {
        private readonly DB_BUENISIMOEntities _dbContext;

        public EmpleadoService()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }

        public ReporteEmpleadoModel GenerarReporte(string quincena, string mes, int anio)
        {
            var fechaInicioQuincena = GetFechaInicioQuincena(quincena, mes, anio);
            var fechaFinQuincena = GetFechaFinQuincena(quincena, mes, anio);

            // Obtener empleados con sus asistencias en el rango de fechas
            var empleados = _dbContext.tbUsuarios
                .Where(e => e.idRol != 1) // Excluir administradores
                .Select(e => new
                {
                    e.idUsuario,
                    e.nombre,
                    e.apellido,
                    e.tbRoles.nombreRol,
                    e.tbAsistencias
                })
                .ToList() // Trae los datos a memoria
                .Select(e => new EmpleadoDTO
                {
                    IdEmpleado = e.idUsuario,
                    NombreCompleto = e.nombre + " " + e.apellido,
                    Puesto = e.nombreRol,

                    // Contar tardanzas (idObservacionAsistencia == 2)
                    Tardanzas = e.tbAsistencias.Count(a =>
                        a.fecha >= fechaInicioQuincena &&
                        a.fecha <= fechaFinQuincena &&
                        a.idObservacionAsistencia == 2),

                    // Contar faltas (idObservacionAsistencia == 4)
                    Faltas = e.tbAsistencias.Count(a =>
                        a.fecha >= fechaInicioQuincena &&
                        a.fecha <= fechaFinQuincena &&
                        a.idObservacionAsistencia == 4),

                    // Calcular pago quincenal (ejemplo básico)
                    PagoQuincenal = CalcularPagoQuincenal(e.idUsuario, fechaInicioQuincena, fechaFinQuincena),

                    // Manejar estado del empleado (si no hay columna 'estado', puedes hacerlo desde otra forma)
                    Estado = e.tbAsistencias.Any(a =>
                        a.fecha >= fechaInicioQuincena &&
                        a.fecha <= fechaFinQuincena &&
                        a.idObservacionAsistencia == 5)
                        ? "Vacaciones"
                        : "Activo"
                }).ToList();

            var totalEmpleados = empleados.Count;
            var tardanzasTotales = empleados.Sum(e => e.Tardanzas);
            var faltasTotales = empleados.Sum(e => e.Faltas);
            var nominaTotal = empleados.Sum(e => e.PagoQuincenal);

            return new ReporteEmpleadoModel
            {
                Quincena = quincena,
                Mes = mes,
                Anio = anio,
                TotalEmpleados = totalEmpleados,
                TardanzasTotales = tardanzasTotales,
                FaltasTotales = faltasTotales,
                NominaTotal = nominaTotal,
                Empleados = empleados
            };
        }

        public DateTime GetFechaInicioQuincena(string quincena, string mes, int anio)
        {
            int dia = quincena == "Primera quincena" ? 1 : 16;
            return new DateTime(anio, MesToNumero(mes), dia);
        }

        public DateTime GetFechaFinQuincena(string quincena, string mes, int anio)
        {
            int ultimoDia = quincena == "Primera quincena" ? 15 : DateTime.DaysInMonth(anio, MesToNumero(mes));
            return new DateTime(anio, MesToNumero(mes), ultimoDia);
        }

        private int MesToNumero(string mes)
        {
            switch (mes)
            {
                case "Enero": return 1;
                case "Febrero": return 2;
                case "Marzo": return 3;
                case "Abril": return 4;
                case "Mayo": return 5;
                case "Junio": return 6;
                case "Julio": return 7;
                case "Agosto": return 8;
                case "Setiembre": return 9;
                case "Octubre": return 10;
                case "Noviembre": return 11;
                case "Diciembre": return 12;
                default: return TimeProvider.Now.Month;
            }
        }


        private decimal CalcularPagoQuincenal(int idEmpleado, DateTime fechaInicioQuincena, DateTime fechaFinQuincena)
        {
            // Ejemplo básico: salario base - descuentos + bonificaciones
            const decimal salarioBase = 3000m; // Mensual
            decimal diasTrabajados = _dbContext.tbAsistencias
                .Count(a => a.idUsuario == idEmpleado &&
                            a.fecha >= fechaInicioQuincena &&
                            a.fecha <= fechaFinQuincena &&
                            a.idObservacionAsistencia == 1); // 1 = Asistencia normal

            decimal pagoDiario = salarioBase / 30;
            decimal pagoQuincenal = diasTrabajados * pagoDiario;

            // Aplicar descuento por faltas
            decimal faltas = _dbContext.tbAsistencias.Count(a =>
                a.idUsuario == idEmpleado &&
                a.fecha >= fechaInicioQuincena &&
                a.fecha <= fechaFinQuincena &&
                a.idObservacionAsistencia == 4); // 4 = Falta injustificada

            pagoQuincenal -= faltas * (salarioBase / 30 * 2); // Ejemplo: cada falta cuesta 2 días

            return Math.Round(pagoQuincenal, 2);
        }

        public DetalleEmpleadoModalDTO ObtenerDetalleEmpleado(int idEmpleado, DateTime fechaInicioQuincena, DateTime fechaFinQuincena)
        {
            var empleado = _dbContext.tbUsuarios.Find(idEmpleado);
            if (empleado == null) return null;

            var asistencias = _dbContext.tbAsistencias
                .Where(a => a.idUsuario == idEmpleado &&
                            a.fecha >= fechaInicioQuincena &&
                            a.fecha <= fechaFinQuincena)
                .ToList();

            var listaTardanzas = new List<TardanzaDTO>();
            var listaFaltas = new List<FaltaDTO>();

            foreach (var a in asistencias)
            {
                if (a.idObservacionAsistencia == 2 && a.horaEntrada.HasValue)
                {
                    // Tardanza: si llegó tarde
                    var horaLaboralEsperada = new TimeSpan(9, 0, 0); // 9:00 AM
                    var diferencia = a.horaEntrada.Value.TimeOfDay - horaLaboralEsperada;

                    if (diferencia > TimeSpan.Zero)
                    {
                        listaTardanzas.Add(new TardanzaDTO
                        {
                            Fecha = a.fecha,
                            HoraEntrada = a.horaEntrada.Value.TimeOfDay,
                            DiferenciaTardanza = diferencia
                        });
                    }
                }

                if (a.idObservacionAsistencia == 4 || a.idObservacionAsistencia == 5)
                {
                    // Falta justificada o injustificada
                    string motivo = a.idObservacionAsistencia == 4 ? "Falta Justificada" : "Falta Injustificada";

                    listaFaltas.Add(new FaltaDTO
                    {
                        Fecha = a.fecha,
                        Motivo = motivo
                    });
                }
            }

            decimal sueldoBase = 600m;
            decimal descuentoPorTardanza = 10m;
            decimal descuentoPorFalta = 30m;

            decimal totalDescuentos =
                listaTardanzas.Count * descuentoPorTardanza +
                listaFaltas.Count * descuentoPorFalta;

            decimal sueldoNeto = Math.Max(sueldoBase - totalDescuentos, 0);

            var desglosePago = new List<PagoConceptoDTO>
    {
        new PagoConceptoDTO
        {
            Concepto = "Salario Base Quincenal",
            Valor = sueldoBase,
            Tipo = "Ingreso"
        },
        new PagoConceptoDTO
        {
            Concepto = $"Descuento por Tardanzas ({listaTardanzas.Count} días)",
            Valor = -descuentoPorTardanza * listaTardanzas.Count(),
            Tipo = "Descuento"
        },
        new PagoConceptoDTO
        {
            Concepto = $"Descuento por Faltas ({listaFaltas.Count} días)",
            Valor = -descuentoPorFalta * listaFaltas.Count(),
            Tipo = "Descuento"
        }
    };

            return new DetalleEmpleadoModalDTO
            {
                Nombre = $"{empleado.nombre} {empleado.apellido}",
                Puesto = empleado.tbRoles.nombreRol,
                SalarioBaseQuincenal = sueldoBase,
                Tardanzas = listaTardanzas,
                Faltas = listaFaltas,
                DesglosePago = desglosePago,
                SueldoNeto = sueldoNeto
            };
        }
    }
}
﻿using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuenisimo.Services
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

            var asistencias = _dbContext.tbAsistencias
                .Where(a => a.fecha >= fechaInicioQuincena && a.fecha <= fechaFinQuincena)
                .ToList();

            var empleados = _dbContext.tbUsuarios
                .Where(e => e.idRol != 1)
                .ToList()
                .Select(e =>
                {
                    decimal pago = CalcularPagoPorHoras(e.idUsuario, fechaInicioQuincena, fechaFinQuincena);
                    return new EmpleadoDTO
                    {
                        IdEmpleado = e.idUsuario,
                        NombreCompleto = e.nombre + " " + e.apellido,
                        Puesto = e.tbRoles.nombreRol,
                        Tardanzas = asistencias.Count(a => a.idUsuario == e.idUsuario &&
                            (a.idObservacionAsistencia == 2 || a.idObservacionAsistencia == 3)),
                        Faltas = asistencias.Count(a => a.idUsuario == e.idUsuario &&
                            (a.idObservacionAsistencia == 7 || a.idObservacionAsistencia == 8)),
                        PagoQuincenal = pago,
                        Estado = asistencias.Any(a => a.idUsuario == e.idUsuario && a.idObservacionAsistencia == 10) ? "Vacaciones" : "Activo"
                    };
                }).ToList();

            return new ReporteEmpleadoModel
            {
                Quincena = quincena,
                Mes = mes,
                Anio = anio,
                TotalEmpleados = empleados.Count,
                TardanzasTotales = empleados.Sum(e => e.Tardanzas),
                FaltasTotales = empleados.Sum(e => e.Faltas),
                NominaTotal = empleados.Sum(e => e.PagoQuincenal),
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
                default: return DateTime.Now.Month;
            }
        }

        private decimal CalcularPagoPorHoras(int idEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            var horario = _dbContext.tbHorarios
                .FirstOrDefault(h => h.idUsuario == idEmpleado && h.activo == true &&
                                     h.fechaInicioVigencia <= fechaFin &&
                                     (h.fechaFinVigencia == null || h.fechaFinVigencia >= fechaInicio));

            if (horario == null) return 0;

            var asistencias = _dbContext.tbAsistencias
                .Where(a => a.idUsuario == idEmpleado && a.fecha >= fechaInicio && a.fecha <= fechaFin &&
                            a.horaEntrada.HasValue)
                .ToList();

            double totalHoras = 0;
            foreach (var a in asistencias)
            {
                TimeSpan horaInicio = a.horaEntrada.Value.TimeOfDay;
                TimeSpan horaFin = a.horaSalida.HasValue ? a.horaSalida.Value.TimeOfDay : horario.horaSalida;
                if (horaFin > horario.horaSalida) horaFin = horario.horaSalida;
                if (horaInicio < horario.horaEntrada) horaInicio = horario.horaEntrada;

                if (horaFin > horaInicio)
                    totalHoras += (horaFin - horaInicio).TotalHours;
            }

            return Math.Round((decimal)totalHoras * horario.pagoPorHora, 2);
        }

        public DetalleEmpleadoModalDTO ObtenerDetalleEmpleado(int idEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            var empleado = _dbContext.tbUsuarios.Find(idEmpleado);
            if (empleado == null) return null;

            var horario = _dbContext.tbHorarios
                .FirstOrDefault(h => h.idUsuario == idEmpleado && h.activo == true &&
                                     h.fechaInicioVigencia <= fechaFin &&
                                     (h.fechaFinVigencia == null || h.fechaFinVigencia >= fechaInicio));

            var asistencias = _dbContext.tbAsistencias
                .Where(a => a.idUsuario == idEmpleado && a.fecha >= fechaInicio && a.fecha <= fechaFin)
                .ToList();

            var listaAsistencias = asistencias
                .Where(a => a.horaEntrada.HasValue)
                .Select(a =>
                {
                    TimeSpan horaEntradaHorario = horario?.horaEntrada ?? new TimeSpan(9, 0, 0);
                    TimeSpan horaSalidaHorario = horario?.horaSalida ?? new TimeSpan(18, 0, 0);

                    TimeSpan horaInicio = a.horaEntrada.Value.TimeOfDay;
                    TimeSpan horaFin = a.horaSalida.HasValue ? a.horaSalida.Value.TimeOfDay : horaSalidaHorario;

                    if (horaFin > horaSalidaHorario) horaFin = horaSalidaHorario;
                    if (horaInicio < horaEntradaHorario) horaInicio = horaEntradaHorario;

                    double horasTrabajadas = horaFin > horaInicio ? (horaFin - horaInicio).TotalHours : 0;

                    decimal pagoPorHora = horario?.pagoPorHora ?? 5;
                    decimal pagoDia = Math.Round((decimal)horasTrabajadas * pagoPorHora, 2);

                    int minutosTarde = (int)Math.Max(0, (horaInicio - horaEntradaHorario).TotalMinutes);
                    decimal descuento = Math.Round(((decimal)minutosTarde / 60) * pagoPorHora, 2);

                    return new AsistenciaDTO
                    {
                        Fecha = a.fecha,
                        HoraEntrada = horaInicio,
                        HoraSalida = horaFin,
                        HorasTrabajadas = horasTrabajadas,
                        PagoDelDia = pagoDia,
                        MinutosTardanza = minutosTarde,
                        Descuento = descuento
                    };
                }).ToList();

            var listaTardanzas = asistencias
                .Where(a => (a.idObservacionAsistencia == 2 || a.idObservacionAsistencia == 3) && a.horaEntrada.HasValue)
                .Select(a => new TardanzaDTO
                {
                    Fecha = a.fecha,
                    HoraEntrada = a.horaEntrada.Value.TimeOfDay,
                    DiferenciaTardanza = TimeSpan.FromMinutes(a.minutosTardanza ?? 0),
                    TipoTardanza = a.idObservacionAsistencia == 2 ? "Leve" : "Significativa"
                }).ToList();

            var listaFaltas = asistencias
                .Where(a => a.idObservacionAsistencia == 7 || a.idObservacionAsistencia == 8 || (!a.horaEntrada.HasValue && !a.horaSalida.HasValue))
                .Select(a => new FaltaDTO
                {
                    Fecha = a.fecha,
                    Motivo = (!a.horaEntrada.HasValue && !a.horaSalida.HasValue) ? "Falta por inasistencia total" : (a.idObservacionAsistencia == 7 ? "Falta Justificada" : "Falta Injustificada")
                }).ToList();

            double totalHoras = listaAsistencias.Sum(a => a.HorasTrabajadas);
            decimal sueldoBase = Math.Round((decimal)totalHoras * (horario?.pagoPorHora ?? 5), 2);

            var desglose = new List<PagoConceptoDTO>
            {
                new PagoConceptoDTO
                {
                    Concepto = "Pago por horas trabajadas",
                    Valor = sueldoBase,
                    Tipo = "Ingreso"
                }
            };

            return new DetalleEmpleadoModalDTO
            {
                Nombre = empleado.nombre + " " + empleado.apellido,
                Puesto = empleado.tbRoles.nombreRol,
                SalarioBaseQuincenal = sueldoBase,
                Tardanzas = listaTardanzas,
                Faltas = listaFaltas,
                DesglosePago = desglose,
                SueldoNeto = sueldoBase,
                Asistencias = listaAsistencias,
                HorarioAsignado = horario != null ? string.Format("{0:hh\\:mm} - {1:hh\\:mm}", horario.horaEntrada, horario.horaSalida) : "No asignado"
            };
        }
    }
}

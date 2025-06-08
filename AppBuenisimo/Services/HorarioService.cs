using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Utils;

namespace AppBuenisimo.Services
{
    public class HorarioService
    {
        private readonly DB_BUENISIMOEntities _context;

        // ✅ Constructor para inyección (por pruebas u otros controladores)
        public HorarioService(DB_BUENISIMOEntities context = null)
        {
            _context = context;
        }

        // ✅ Método auxiliar para usar contexto interno o el inyectado
        private DB_BUENISIMOEntities GetContext() => _context ?? new DB_BUENISIMOEntities();

        public List<GestionEmpleadoIndexViewModel> ObtenerTodosLosUsuariosParaGestion()
        {
            var ctx = GetContext();
            return ctx.tbUsuarios
                .Include(u => u.tbRoles)
                .Where(u => u.activo == true)
                .Select(u => new GestionEmpleadoIndexViewModel
                {
                    IdUsuario = u.idUsuario,
                    NombreCompleto = u.nombre + " " + u.apellido,
                    CorreoElectronico = u.correoElectronico,
                    Dni = u.dni,
                    Rol = u.tbRoles != null ? u.tbRoles.nombreRol : "Sin rol"
                })
                .OrderBy(u => u.NombreCompleto)
                .ToList();
        }

        public tbUsuarios ObtenerUsuarioPorId(int idUsuario)
        {
            var ctx = GetContext();
            return ctx.tbUsuarios.Find(idUsuario);
        }

        public List<HorarioDetalleViewModel> ObtenerHorariosPorUsuario(int idUsuario)
        {
            var ctx = GetContext();
            return ctx.tbHorarios
                .Where(h => h.idUsuario == idUsuario)
                .OrderByDescending(h => h.fechaInicioVigencia)
                .ThenBy(h => h.diaSemana)
                .Select(h => new HorarioDetalleViewModel
                {
                    IdHorario = h.idHorario,
                    DiaSemanaValor = h.diaSemana,
                    DiaSemanaTexto = h.diaSemana == 0 ? "Domingo" :
                                     h.diaSemana == 1 ? "Lunes" :
                                     h.diaSemana == 2 ? "Martes" :
                                     h.diaSemana == 3 ? "Miércoles" :
                                     h.diaSemana == 4 ? "Jueves" :
                                     h.diaSemana == 5 ? "Viernes" : "Sábado",
                    HoraEntrada = h.horaEntrada,
                    HoraSalida = h.horaSalida,
                    PagoPorHora = h.pagoPorHora,
                    FechaInicioVigencia = h.fechaInicioVigencia,
                    FechaFinVigencia = h.fechaFinVigencia,
                    Activo = h.activo
                })
                .ToList();
        }

        public tbHorarios ObtenerHorarioPorId(int idHorario)
        {
            var ctx = GetContext();
            return ctx.tbHorarios.Find(idHorario);
        }

        public bool AgregarHorario(HorarioFormViewModel model)
        {
            var ctx = GetContext();

            if (model.Activo)
            {
                var horariosPreviosActivos = ctx.tbHorarios
                    .Where(h => h.idUsuario == model.IdUsuario &&
                                h.diaSemana == model.DiaSemana &&
                                h.activo == true &&
                                (h.fechaFinVigencia == null || h.fechaFinVigencia >= model.FechaInicioVigencia))
                    .ToList();

                foreach (var horarioPrevio in horariosPreviosActivos)
                {
                    if (horarioPrevio.fechaInicioVigencia < model.FechaInicioVigencia)
                    {
                        horarioPrevio.fechaFinVigencia = model.FechaInicioVigencia.AddDays(-1);
                        if (horarioPrevio.fechaFinVigencia < horarioPrevio.fechaInicioVigencia)
                            horarioPrevio.activo = false;
                    }
                    else
                    {
                        horarioPrevio.activo = false;
                    }
                }
            }

            var nuevoHorario = new tbHorarios
            {
                idUsuario = model.IdUsuario,
                diaSemana = model.DiaSemana,
                horaEntrada = model.HoraEntrada,
                horaSalida = model.HoraSalida,
                pagoPorHora = model.PagoPorHora,
                fechaInicioVigencia = model.FechaInicioVigencia,
                fechaFinVigencia = model.FechaFinVigencia,
                activo = model.Activo
            };

            ctx.tbHorarios.Add(nuevoHorario);
            return ctx.SaveChanges() > 0;
        }

        public bool ActualizarHorario(HorarioFormViewModel model)
        {
            var ctx = GetContext();
            var horarioExistente = ctx.tbHorarios.Find(model.IdHorario);
            if (horarioExistente == null) return false;

            horarioExistente.diaSemana = model.DiaSemana;
            horarioExistente.horaEntrada = model.HoraEntrada;
            horarioExistente.horaSalida = model.HoraSalida;
            horarioExistente.pagoPorHora = model.PagoPorHora;
            horarioExistente.fechaInicioVigencia = model.FechaInicioVigencia;
            horarioExistente.fechaFinVigencia = model.FechaFinVigencia;
            horarioExistente.activo = model.Activo;

            ctx.Entry(horarioExistente).State = EntityState.Modified;
            return ctx.SaveChanges() > 0;
        }

        public bool EliminarHorario(int idHorario)
        {
            var ctx = GetContext();
            var horario = ctx.tbHorarios.Find(idHorario);
            if (horario == null) return false;

            horario.activo = false;
            if (horario.fechaFinVigencia == null || horario.fechaFinVigencia > TimeProvider.Today)
                horario.fechaFinVigencia = TimeProvider.Today.AddDays(-1);

            ctx.Entry(horario).State = EntityState.Modified;
            return ctx.SaveChanges() > 0;
        }
    }
}

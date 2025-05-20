// En una carpeta Services
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; // Para Include
using SistemaWebEficienciaOperativa.Models; // Asumiendo que aquí están tus entidades EF (DB_BUENISIMOEntities, tbUsuario, tbHorario)
using SistemaWebEficienciaOperativa.Models.ViewModels; // Para los ViewModels

namespace SistemaWebEficienciaOperativa.Services
{
    

    public class HorarioService
    {
        public List<GestionEmpleadoIndexViewModel> ObtenerTodosLosUsuariosParaGestion()
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbUsuarios
                    .Include(u => u.tbRoles) // Incluir el rol para mostrar su nombre
                    .Where(u => u.activo == true) // Opcional: solo usuarios activos
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
        }

        public tbUsuarios ObtenerUsuarioPorId(int idUsuario)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbUsuarios.Find(idUsuario);
            }
        }
        public List<HorarioDetalleViewModel> ObtenerHorariosPorUsuario(int idUsuario)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbHorarios
                    .Where(h => h.idUsuario == idUsuario)
                    .OrderByDescending(h => h.fechaInicioVigencia) // Más recientes primero
                    .ThenBy(h => h.diaSemana)
                    .Select(h => new HorarioDetalleViewModel
                    {
                        IdHorario = h.idHorario,
                        DiaSemanaValor = h.diaSemana,
                        // Esto se puede mejorar con un helper o mapeo más elegante
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
        }

        public tbHorarios ObtenerHorarioPorId(int idHorario)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbHorarios.Find(idHorario);
            }
        }

        public bool AgregarHorario(HorarioFormViewModel model)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                // Lógica de validación adicional (ej. solapamientos) podría ir aquí
                // Por ejemplo, antes de agregar un nuevo horario activo para un día,
                // podrías desactivar (poner fechaFinVigencia o activo=false)
                // cualquier otro horario activo para ese mismo usuario y día.

                // Desactivar horarios previos para el mismo día y usuario si este es activo
                if (model.Activo)
                {
                    var horariosPreviosActivos = _dbContext.tbHorarios
                        .Where(h => h.idUsuario == model.IdUsuario &&
                                    h.diaSemana == model.DiaSemana &&
                                    h.activo == true &&
                                    (h.fechaFinVigencia == null || h.fechaFinVigencia >= model.FechaInicioVigencia))
                        .ToList();

                    foreach (var horarioPrevio in horariosPreviosActivos)
                    {
                        if (horarioPrevio.fechaInicioVigencia < model.FechaInicioVigencia)
                        {
                            // Si el nuevo horario empieza después, acortamos el viejo
                            horarioPrevio.fechaFinVigencia = model.FechaInicioVigencia.AddDays(-1);
                            if (horarioPrevio.fechaFinVigencia < horarioPrevio.fechaInicioVigencia)
                            {
                                horarioPrevio.activo = false; // O lo marcamos inactivo si el rango es inválido
                            }
                        }
                        else // Si el nuevo horario solapa o es el mismo inicio, desactivamos el viejo
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

                _dbContext.tbHorarios.Add(nuevoHorario);
                return _dbContext.SaveChanges() > 0;
            }
        }

        public bool ActualizarHorario(HorarioFormViewModel model)
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                var horarioExistente = _dbContext.tbHorarios.Find(model.IdHorario);
                if (horarioExistente == null) return false;

                horarioExistente.diaSemana = model.DiaSemana;
                horarioExistente.horaEntrada = model.HoraEntrada;
                horarioExistente.horaSalida = model.HoraSalida;
                horarioExistente.pagoPorHora = model.PagoPorHora;
                horarioExistente.fechaInicioVigencia = model.FechaInicioVigencia;
                horarioExistente.fechaFinVigencia = model.FechaFinVigencia;
                horarioExistente.activo = model.Activo;
                // No modificar idUsuario aquí

                _dbContext.Entry(horarioExistente).State = EntityState.Modified;
                return _dbContext.SaveChanges() > 0;
            }
        }

        public bool EliminarHorario(int idHorario) // O podría ser DesactivarHorario
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                var horario = _dbContext.tbHorarios.Find(idHorario);
                if (horario == null) return false;

                // Opción 1: Borrado físico
                // _dbContext.tbHorarios.Remove(horario);

                // Opción 2: Borrado lógico (recomendado si tienes 'activo' y 'fechaFinVigencia')
                horario.activo = false;
                if (horario.fechaFinVigencia == null || horario.fechaFinVigencia > DateTime.Today)
                {
                    horario.fechaFinVigencia = DateTime.Today.AddDays(-1); // Marcar como finalizado ayer
                }
                _dbContext.Entry(horario).State = EntityState.Modified;

                return _dbContext.SaveChanges() > 0;
            }
        }
    }
}
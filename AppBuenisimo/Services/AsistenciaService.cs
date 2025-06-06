// En tu carpeta Services
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AppBuenisimo.Models; // Tus entidades EF
using AppBuenisimo.Models.ViewModels;
using System.Web.Mvc;
using AppBuenisimo.Utils;

namespace AppBuenisimo.Services
{
    public static class ObservacionAsistenciaIds
    {
        // Estos IDs deben coincidir con los de tu tabla tbObservacionesAsistencias
        public const int ENTRADA_REGISTRADA = 9; // O el ID que hayas asignado
        public const int ASISTENCIA_PUNTUAL = 1;
        public const int TARDANZA_LEVE = 2;
        public const int TARDANZA_SIGNIFICATIVA = 3;
        public const int SALIDA_PUNTUAL = 4; // Podrías tener una observación genérica como "COMPLETADA"
        public const int SALIDA_TEMPRANA = 5;
        public const int DIA_NO_LABORAL = 10;
    }

    public class AsistenciaService
    {
        private readonly DB_BUENISIMOEntities _dbContext;

        public AsistenciaService()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }
        public AsistenciaService(DB_BUENISIMOEntities context) // Para testing
        {
            _dbContext = context;
        }

        public tbHorarios ObtenerHorarioVigenteParaUsuarioHoy(int idUsuario, DateTime fecha)
        {
            byte diaSemana = (byte)fecha.DayOfWeek; // Domingo = 0, Lunes = 1, etc.

            return _dbContext.tbHorarios
                .Where(h => h.idUsuario == idUsuario &&
                            h.diaSemana == diaSemana &&
                            h.activo == true &&
                            h.fechaInicioVigencia <= fecha &&
                            (h.fechaFinVigencia == null || h.fechaFinVigencia >= fecha))
                .OrderByDescending(h => h.fechaInicioVigencia) // En caso de solapamiento (no debería si la lógica de horarios es correcta)
                .FirstOrDefault();
        }

        public tbAsistencias ObtenerAsistenciaDeHoy(int idUsuario, DateTime fecha)
        {
            return _dbContext.tbAsistencias
                .FirstOrDefault(a => a.idUsuario == idUsuario && a.fecha == fecha.Date);
        }

        public MarcarAsistenciaViewModel PrepararViewModelMarcacion(int idUsuario)
        {
            var viewModel = new MarcarAsistenciaViewModel
            {
                IdUsuario = idUsuario,
                FechaActual = TimeProvider.Today
            };
            var usuario = _dbContext.tbUsuarios.Find(idUsuario);
            if (usuario == null) return viewModel; // O lanzar excepción

            viewModel.NombreUsuario = $"{usuario.nombre} {usuario.apellido}";

            var horarioHoy = ObtenerHorarioVigenteParaUsuarioHoy(idUsuario, TimeProvider.Today);
            var asistenciaHoy = ObtenerAsistenciaDeHoy(idUsuario, TimeProvider.Today);

            viewModel.SucursalesDisponibles = new SelectList(_dbContext.tbSucursales.ToList(), "idSucursal", "nombre");

            if (horarioHoy == null)
            {
                viewModel.MensajeEntrada = "No tienes un horario asignado para hoy.";
                viewModel.PuedeMarcarEntrada = false;
                viewModel.PuedeMarcarSalida = false;
                viewModel.HorarioEsperadoHoy = "No laboral";
                return viewModel;
            }

            viewModel.HorarioEsperadoHoy = $"{horarioHoy.horaEntrada:hh\\:mm} - {horarioHoy.horaSalida:hh\\:mm}";
            DateTime ahora = TimeProvider.Now;
            DateTime horaEntradaEsperada = TimeProvider.Today.Add(horarioHoy.horaEntrada);
            DateTime horaSalidaEsperada = TimeProvider.Today.Add(horarioHoy.horaSalida);

            // Lógica de ENTRADA
            if (asistenciaHoy == null || asistenciaHoy.horaEntrada == null)
            {
                viewModel.YaMarcoEntradaHoy = false;
                DateTime inicioPermitidoEntrada = horaEntradaEsperada.AddMinutes(-10); // 10 mins antes
                DateTime finToleranciaEntrada = horaEntradaEsperada.AddMinutes(10);  // 10 mins de tolerancia

                if (ahora >= inicioPermitidoEntrada)
                {
                    viewModel.PuedeMarcarEntrada = true;
                    if (ahora <= horaEntradaEsperada)
                    {
                        viewModel.MensajeEntrada = "Puedes marcar tu entrada (A tiempo).";
                    }
                    else if (ahora <= finToleranciaEntrada)
                    {
                        viewModel.MensajeEntrada = $"Puedes marcar tu entrada (Tolerancia). Llevas {(ahora - horaEntradaEsperada).Minutes} min de retraso.";
                        viewModel.MinutosTardanzaEstimados = (int)(ahora - horaEntradaEsperada).TotalMinutes;
                    }
                    else // Tardanza significativa
                    {
                        // Tardanza se calcula desde finTolerancia + 5 minutos
                        DateTime inicioCalculoTardanzaReal = finToleranciaEntrada.AddMinutes(5);
                        if (ahora > inicioCalculoTardanzaReal)
                        {
                            viewModel.MinutosTardanzaEstimados = (int)(ahora - inicioCalculoTardanzaReal).TotalMinutes;
                            viewModel.MensajeEntrada = $"Puedes marcar tu entrada. Llevas {viewModel.MinutosTardanzaEstimados} minutos de tardanza acumulada hoy.";
                        }
                        else if (ahora > finToleranciaEntrada)
                        {
                            viewModel.MinutosTardanzaEstimados = (int)(ahora - horaEntradaEsperada).TotalMinutes; // Tardanza desde la hora de entrada
                            viewModel.MensajeEntrada = $"Puedes marcar tu entrada. Llevas {viewModel.MinutosTardanzaEstimados} minutos de tardanza.";
                        }
                        else
                        { // Este caso no debería darse por la lógica previa, pero por si acaso
                            viewModel.MinutosTardanzaEstimados = (int)(ahora - horaEntradaEsperada).TotalMinutes;
                            viewModel.MensajeEntrada = $"Puedes marcar tu entrada. Llevas {viewModel.MinutosTardanzaEstimados} minutos de tardanza.";
                        }
                    }
                }
                else
                {
                    viewModel.PuedeMarcarEntrada = false;
                    viewModel.MensajeEntrada = $"Aún no puedes marcar tu entrada. Disponible desde las {inicioPermitidoEntrada:HH:mm}.";
                }
            }
            else // Ya marcó entrada
            {
                viewModel.YaMarcoEntradaHoy = true;
                viewModel.PuedeMarcarEntrada = false;
                viewModel.HoraEntradaMarcada = asistenciaHoy.horaEntrada.Value.ToString("HH:mm:ss");
                viewModel.MensajeEntrada = $"Entrada marcada a las {viewModel.HoraEntradaMarcada}.";

                // Lógica de SALIDA (solo si ya marcó entrada)
                if (asistenciaHoy.horaSalida == null)
                {
                    viewModel.YaMarcoSalidaHoy = false;
                    viewModel.PuedeMarcarSalida = true; // Siempre puede intentar marcar salida

                    // ¿Está saliendo muy temprano? ej. más de 30 minutos antes de su hora de salida
                    if (ahora < horaSalidaEsperada.AddMinutes(-30))
                    {
                        viewModel.RequiereConfirmacionSalidaTemprana = true;
                        viewModel.MensajeSalida = "¿Seguro que deseas marcar tu salida? Es mucho antes de tu hora de salida esperada.";
                    }
                    else
                    {
                        viewModel.MensajeSalida = "Puedes marcar tu salida.";
                    }
                }
                else
                {
                    viewModel.YaMarcoSalidaHoy = true;
                    viewModel.PuedeMarcarSalida = false;
                    viewModel.HoraSalidaMarcada = asistenciaHoy.horaSalida.Value.ToString("HH:mm:ss");
                    viewModel.MensajeSalida = $"Salida marcada a las {viewModel.HoraSalidaMarcada}.";
                }
            }
            return viewModel;
        }

        public Tuple<bool, string> RegistrarEntrada(int idUsuario, int idSucursal)
        {
            DateTime fechaHoy = TimeProvider.Today;
            DateTime ahora = TimeProvider.Now;

            var asistenciaHoy = ObtenerAsistenciaDeHoy(idUsuario, fechaHoy);
            if (asistenciaHoy != null && asistenciaHoy.horaEntrada != null)
            {
                return Tuple.Create(false, "Ya has marcado tu entrada hoy.");
            }

            var horarioHoy = ObtenerHorarioVigenteParaUsuarioHoy(idUsuario, fechaHoy);
            if (horarioHoy == null)
            {
                return Tuple.Create(false, "No tienes un horario asignado para hoy. No se puede marcar entrada.");
            }

            DateTime horaEntradaEsperada = fechaHoy.Add(horarioHoy.horaEntrada);
            DateTime inicioPermitidoEntrada = horaEntradaEsperada.AddMinutes(-10);

            if (ahora < inicioPermitidoEntrada)
            {
                return Tuple.Create(false, $"Aún no puedes marcar tu entrada. Disponible desde las {inicioPermitidoEntrada:HH:mm}.");
            }

            int minutosDeTardanza = 0;
            int idObs;

            DateTime finToleranciaEntrada = horaEntradaEsperada.AddMinutes(10);
            DateTime inicioCalculoTardanzaReal = finToleranciaEntrada.AddMinutes(5);

            if (ahora <= horaEntradaEsperada)
            { // A tiempo
                idObs = ObservacionAsistenciaIds.ASISTENCIA_PUNTUAL;
            }
            else if (ahora <= finToleranciaEntrada)
            { // Dentro de tolerancia
                idObs = ObservacionAsistenciaIds.TARDANZA_LEVE;
                minutosDeTardanza = (int)(ahora - horaEntradaEsperada).TotalMinutes;
            }
            else
            { // Tardanza significativa
                idObs = ObservacionAsistenciaIds.TARDANZA_SIGNIFICATIVA;
                if (ahora > inicioCalculoTardanzaReal)
                {
                    minutosDeTardanza = (int)Math.Max(0, (ahora - inicioCalculoTardanzaReal).TotalMinutes);
                }
                else
                {
                    minutosDeTardanza = (int)Math.Max(0, (ahora - horaEntradaEsperada).TotalMinutes);
                }
            }


            if (asistenciaHoy == null)
            {
                asistenciaHoy = new tbAsistencias
                {
                    idUsuario = idUsuario,
                    idSucursal = idSucursal,
                    fecha = fechaHoy,
                    horaEntrada = ahora,
                    idObservacionAsistencia = idObs, // O una genérica "Entrada Registrada" y luego se actualiza
                    minutosTardanza = minutosDeTardanza,
                    idHorarioAplicado = horarioHoy.idHorario
                };
                _dbContext.tbAsistencias.Add(asistenciaHoy);
            }
            else // Esto no debería pasar si la primera condición (asistenciaHoy != null && asistenciaHoy.horaEntrada != null) es cierta
            {
                asistenciaHoy.horaEntrada = ahora;
                asistenciaHoy.idObservacionAsistencia = idObs;
                asistenciaHoy.minutosTardanza = minutosDeTardanza;
                asistenciaHoy.idHorarioAplicado = horarioHoy.idHorario;
                asistenciaHoy.idSucursal = idSucursal; // Podría cambiar la sucursal si el primer registro fue una "ausencia" cargada por admin
                _dbContext.Entry(asistenciaHoy).State = EntityState.Modified;
            }

            _dbContext.SaveChanges();
            return Tuple.Create(true, $"Entrada registrada a las {ahora:HH:mm:ss}. Minutos de tardanza: {minutosDeTardanza}");
        }

        public Tuple<bool, string> RegistrarSalida(int idUsuario, int idSucursal, bool confirmaSalidaTemprana)
        {
            DateTime fechaHoy = TimeProvider.Today;
            DateTime ahora = TimeProvider.Now;

            var asistenciaHoy = ObtenerAsistenciaDeHoy(idUsuario, fechaHoy);
            if (asistenciaHoy == null || asistenciaHoy.horaEntrada == null)
            {
                return Tuple.Create(false, "Debes marcar tu entrada primero.");
            }
            if (asistenciaHoy.horaSalida != null)
            {
                return Tuple.Create(false, "Ya has marcado tu salida hoy.");
            }

            var horarioHoy = ObtenerHorarioVigenteParaUsuarioHoy(idUsuario, fechaHoy); // O usar asistenciaHoy.idHorarioAplicado
            if (horarioHoy == null && asistenciaHoy.idHorarioAplicado.HasValue)
            {
                horarioHoy = _dbContext.tbHorarios.Find(asistenciaHoy.idHorarioAplicado.Value);
            }

            if (horarioHoy == null)
            {
                // Si no hay horario, permitir marcar salida pero con una observación genérica.
                asistenciaHoy.horaSalida = ahora;
                // Podrías tener una observación específica como "Salida sin horario de referencia"
                // O mantener la observación de la entrada y solo añadir la hora de salida.
                // Aquí simplemente actualizamos la hora de salida.
                // La observación actual podría ser TARDANZA_SIGNIFICATIVA, y eso está bien.
                // O podrías cambiarla a algo como SALIDA_PUNTUAL si la observación actual es de entrada.
                // Por simplicidad, no la cambiamos a menos que sea específico.
            }
            else
            {
                DateTime horaSalidaEsperada = fechaHoy.Add(horarioHoy.horaSalida);
                if (ahora < horaSalidaEsperada.AddMinutes(-30) && !confirmaSalidaTemprana)
                {
                    return Tuple.Create(false, "SALIDA_TEMPRANA_REQUIERE_CONFIRMACION");
                }

                asistenciaHoy.horaSalida = ahora;
                // Actualizar observación si es necesario.
                // Si salió antes, la observación de "Salida Temprana" podría ser más apropiada que la de tardanza de entrada.
                if (ahora < horaSalidaEsperada.AddMinutes(-5)) // Salió más de 5 min antes
                {
                    // Considerar si la observación de la entrada era tardanza, ¿cuál prevalece?
                    // Podríamos tener una lógica más compleja o simplemente dejarla como está o cambiarla a "Salida Temprana"
                    asistenciaHoy.idObservacionAsistencia = ObservacionAsistenciaIds.SALIDA_TEMPRANA;
                }
                else if (asistenciaHoy.idObservacionAsistencia == ObservacionAsistenciaIds.ENTRADA_REGISTRADA ||
                           asistenciaHoy.idObservacionAsistencia == ObservacionAsistenciaIds.ASISTENCIA_PUNTUAL ||
                           asistenciaHoy.idObservacionAsistencia == ObservacionAsistenciaIds.TARDANZA_LEVE)
                {
                    // Si la entrada fue más o menos normal, y la salida es a tiempo o tarde.
                    asistenciaHoy.idObservacionAsistencia = ObservacionAsistenciaIds.SALIDA_PUNTUAL; // O un "COMPLETADA"
                }
                // Si ya tenía una observación de TARDANZA_SIGNIFICATIVA, podría quedarse así.
            }

            asistenciaHoy.idSucursal = idSucursal; // Actualizar sucursal si es diferente a la de entrada.
            _dbContext.Entry(asistenciaHoy).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Tuple.Create(true, $"Salida registrada a las {ahora:HH:mm:ss}.");
        }

        public List<tbSucursales> ObtenerSucursales()
        {
            return _dbContext.tbSucursales.OrderBy(s => s.nombre).ToList();
        }


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
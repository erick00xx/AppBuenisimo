using AppBuenisimo.Models;
using AppBuenisimo.Services;
using AppBuenisimo.Tests.Helpers;
using AppBuenisimo.Utils;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace AppBuenisimo.Tests
{
    [TestClass]
    public class Test_Registro_Asistencia
    {
        private static int _idCounter = 0;
        private const int IdUsuarioPrueba = 1;
        private const int IdSucursalPrueba = 1;

        // --- NUEVAS VARIABLES DE INSTANCIA ---
        private Mock<DB_BUENISIMOEntities> _mockContext;
        private AsistenciaService _asistenciaService;
        private List<tbHorarios> _horariosData;
        private List<tbAsistencias> _asistenciasData;

        [TestInitialize]
        public void Initialize()
        {
            // Este método se ejecuta ANTES de cada prueba.
            // Creamos listas NUEVAS para cada prueba.
            _horariosData = new List<tbHorarios>();
            _asistenciasData = new List<tbAsistencias>();

            // Creamos Mocks NUEVOS para cada prueba.
            var mockHorariosSet = MockDbSetHelper.CreateMockSet(_horariosData.AsQueryable());
            var mockAsistenciasSet = MockDbSetHelper.CreateMockSet(_asistenciasData.AsQueryable());

            // Configuramos el callback para la captura en el mockSet, que es específico de esta inicialización.
            mockAsistenciasSet.Setup(m => m.Add(It.IsAny<tbAsistencias>()))
                              .Callback<tbAsistencias>(a => _asistenciasData.Add(a));

            _mockContext = new Mock<DB_BUENISIMOEntities>();
            _mockContext.Setup(c => c.tbHorarios).Returns(mockHorariosSet.Object);
            _mockContext.Setup(c => c.tbAsistencias).Returns(mockAsistenciasSet.Object);

            // Creamos una NUEVA instancia del servicio para cada prueba.
            _asistenciaService = new AsistenciaService(_mockContext.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Se ejecuta DESPUÉS de cada prueba.
            Utils.TimeProvider.ResetToDefault();
            // Anulamos las referencias para ayudar al recolector de basura y asegurar que no haya fugas.
            _mockContext = null;
            _asistenciaService = null;
            _horariosData = null;
            _asistenciasData = null;
        }

        private tbHorarios CrearHorarioPrueba(DayOfWeek dia, string horaEntrada, string horaSalida)
        {
            return new tbHorarios
            {
                idHorario = System.Threading.Interlocked.Increment(ref _idCounter),
                idUsuario = IdUsuarioPrueba,
                diaSemana = (byte)dia,
                horaEntrada = TimeSpan.Parse(horaEntrada),
                horaSalida = TimeSpan.Parse(horaSalida),
                activo = true,
                fechaInicioVigencia = DateTime.MinValue
            };
        }

        [TestMethod]
        public void RegistrarEntrada_UsuarioSinHorario_DebeFallar()
        {
            // Arrange
            var fechaPrueba = new DateTime(2023, 10, 27);
            Utils.TimeProvider.SetCustomTime(fechaPrueba);
            // El Initialize ya preparó un mock sin horarios. No se necesita más Arrange.

            // Act
            var resultado = _asistenciaService.RegistrarEntrada(IdUsuarioPrueba, IdSucursalPrueba);

            // Assert
            Assert.IsFalse(resultado.Item1, "La marcación no debería ser exitosa.");
            Assert.AreEqual("No tienes un horario asignado para hoy. No se puede marcar entrada.", resultado.Item2);
        }

        [TestMethod]
        public void RegistrarEntrada_MarcandoPuntual_DebeSerExitosoConObservacionPuntual()
        {
            // Arrange
            var horario = CrearHorarioPrueba(DayOfWeek.Friday, "09:00", "18:00");
            _horariosData.Add(horario); // Añadimos el horario a la lista de esta prueba.

            var fechaPrueba = new DateTime(2023, 10, 27, 8, 55, 0);
            Utils.TimeProvider.SetCustomTime(fechaPrueba);

            // Act
            var resultado = _asistenciaService.RegistrarEntrada(IdUsuarioPrueba, IdSucursalPrueba);

            // Assert
            Assert.IsTrue(resultado.Item1, "La operación de registro debería ser exitosa.");
            _mockContext.Verify(c => c.SaveChanges(), Times.Once());

            Assert.AreEqual(1, _asistenciasData.Count, "Debería haber una asistencia registrada.");
            var asistenciaRegistrada = _asistenciasData.First();
            Assert.AreEqual(IdUsuarioPrueba, asistenciaRegistrada.idUsuario);
            Assert.AreEqual(ObservacionAsistenciaIds.ASISTENCIA_PUNTUAL, asistenciaRegistrada.idObservacionAsistencia);
            Assert.AreEqual(0, asistenciaRegistrada.minutosTardanza);
        }

        [TestMethod]
        public void RegistrarEntrada_MarcandoConTardanzaSignificativa_DebeCalcularMinutosCorrectamente()
        {
            // Arrange
            var horario = CrearHorarioPrueba(DayOfWeek.Friday, "09:00", "18:00");
            _horariosData.Add(horario); // Añadimos el horario a la lista de esta prueba.

            var fechaPrueba = new DateTime(2023, 10, 27, 9, 30, 0);
            Utils.TimeProvider.SetCustomTime(fechaPrueba);

            // Act
            _asistenciaService.RegistrarEntrada(IdUsuarioPrueba, IdSucursalPrueba);

            // Assert
            _mockContext.Verify(c => c.SaveChanges(), Times.Once());

            Assert.AreEqual(1, _asistenciasData.Count, "Debería haber una asistencia registrada.");
            var asistenciaRegistrada = _asistenciasData.First();
            Assert.AreEqual(ObservacionAsistenciaIds.TARDANZA_SIGNIFICATIVA, asistenciaRegistrada.idObservacionAsistencia);
            Assert.AreEqual(15, asistenciaRegistrada.minutosTardanza);
        }

        [TestMethod]
        public void RegistrarSalida_SalidaTempranaSinConfirmacion_DebeRequerirConfirmacion()
        {
            // Arrange
            var horario = CrearHorarioPrueba(DayOfWeek.Friday, "09:00", "18:00");
            _horariosData.Add(horario);

            var asistenciaPrevia = new tbAsistencias { idUsuario = IdUsuarioPrueba, fecha = new DateTime(2023, 10, 27).Date, horaEntrada = new DateTime(2023, 10, 27).Date.AddHours(9) };
            _asistenciasData.Add(asistenciaPrevia);

            var fechaPrueba = new DateTime(2023, 10, 27, 16, 0, 0);
            Utils.TimeProvider.SetCustomTime(fechaPrueba);

            // Act
            var resultado = _asistenciaService.RegistrarSalida(IdUsuarioPrueba, IdSucursalPrueba, confirmaSalidaTemprana: false);

            // Assert
            Assert.IsFalse(resultado.Item1);
            Assert.AreEqual("SALIDA_TEMPRANA_REQUIERE_CONFIRMACION", resultado.Item2);
            _mockContext.Verify(c => c.SaveChanges(), Times.Never());
        }
    }
}
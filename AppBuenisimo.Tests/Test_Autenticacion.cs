using AppBuenisimo.Models;
using AppBuenisimo.Services;
using AppBuenisimo.Tests.Mocks;
using AppBuenisimo.Utils;

namespace AppBuenisimo.Tests
{
    [TestClass]
    public sealed class Test_Autenticacion
    {
        // Escenario 1: Usuario no existe, debe devolver null.
        [TestMethod]
        public void Autenticar_UsuarioNoExistente_DebeDevolverNull()
        {
            // Arrange (Preparar)
            var mockRepo = new UsuarioRepositoryMock(new List<tbUsuarios>()); // Lista vacía
            var loginService = new LoginService(mockRepo);

            // Act (Actuar)
            var resultado = loginService.Autenticar("12345678A", "password123");

            // Assert (Verificar)
            Assert.IsNull(resultado, "El resultado debería ser nulo si el usuario no existe.");
        }

        // Escenario 2: Usuario existe con contraseña en texto plano CORRECTA.
        [TestMethod]
        public void Autenticar_ContrasenaPlanaCorrecta_DebeDevolverUsuarioYHashearPass()
        {
            // Arrange
            var dniPrueba = "11111111B";
            var passPrueba = "secreta";
            var usuarioDePrueba = new tbUsuarios { dni = dniPrueba, contrasena = passPrueba };

            var mockRepo = new UsuarioRepositoryMock(new List<tbUsuarios> { usuarioDePrueba });
            var loginService = new LoginService(mockRepo);

            // Act
            var resultado = loginService.Autenticar(dniPrueba, passPrueba);

            // Assert
            Assert.IsNotNull(resultado, "El usuario no debería ser nulo.");
            Assert.AreEqual(dniPrueba, resultado.dni, "El DNI del usuario devuelto no es el correcto.");
            Assert.IsTrue(mockRepo.GuardarCambiosLlamado, "Se debería haber llamado a GuardarCambios para actualizar el hash.");
            Assert.AreNotEqual(passPrueba, resultado.contrasena, "La contraseña en el objeto debería estar hasheada, no en texto plano.");
        }

        // Escenario 3: Usuario existe con contraseña en texto plano INCORRECTA.
        [TestMethod]
        public void Autenticar_ContrasenaPlanaIncorrecta_DebeDevolverNull()
        {
            // Arrange
            var dniPrueba = "22222222C";
            var passCorrecta = "password";
            var passIncorrecta = "incorrecta";
            var usuarioDePrueba = new tbUsuarios { dni = dniPrueba, contrasena = passCorrecta };

            var mockRepo = new UsuarioRepositoryMock(new List<tbUsuarios> { usuarioDePrueba });
            var loginService = new LoginService(mockRepo);

            // Act
            var resultado = loginService.Autenticar(dniPrueba, passIncorrecta);

            // Assert
            Assert.IsNull(resultado);
            Assert.IsFalse(mockRepo.GuardarCambiosLlamado, "GuardarCambios no debería ser llamado si la contraseña es incorrecta.");
        }

        // Escenario 4: Usuario existe con contraseña hasheada CORRECTA.
        [TestMethod]
        public void Autenticar_ContrasenaHasheadaCorrecta_DebeDevolverUsuario()
        {
            // Arrange
            var dniPrueba = "33333333D";
            var passPrueba = "miPasswordSuperSegura";
            var passHasheada = PasswordHasher.HashPassword(passPrueba);
            var usuarioDePrueba = new tbUsuarios { dni = dniPrueba, contrasena = passHasheada };

            var mockRepo = new UsuarioRepositoryMock(new List<tbUsuarios> { usuarioDePrueba });
            var loginService = new LoginService(mockRepo);

            // Act
            var resultado = loginService.Autenticar(dniPrueba, passPrueba);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(dniPrueba, resultado.dni);
            Assert.IsFalse(mockRepo.GuardarCambiosLlamado, "GuardarCambios no debe llamarse si la contraseña ya está hasheada.");
        }

        // Escenario 5: Usuario existe con contraseña hasheada INCORRECTA.
        [TestMethod]
        public void Autenticar_ContrasenaHasheadaIncorrecta_DebeDevolverNull()
        {
            // Arrange
            var dniPrueba = "44444444E";
            var passCorrecta = "passwordCorrecto";
            var passIncorrecta = "passwordIncorrecto";
            var passHasheada = PasswordHasher.HashPassword(passCorrecta);
            var usuarioDePrueba = new tbUsuarios { dni = dniPrueba, contrasena = passHasheada };

            var mockRepo = new UsuarioRepositoryMock(new List<tbUsuarios> { usuarioDePrueba });
            var loginService = new LoginService(mockRepo);

            // Act
            var resultado = loginService.Autenticar(dniPrueba, passIncorrecta);

            // Assert
            Assert.IsNull(resultado);
        }
    }
}

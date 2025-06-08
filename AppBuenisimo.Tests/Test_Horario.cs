using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AppBuenisimo.Tests
{
    [TestClass]
    public class Test_Horario
    {
        private Mock<DbSet<tbUsuarios>> _mockUsuarios;
        private Mock<DbSet<tbRoles>> _mockRoles;
        private Mock<DbSet<tbHorarios>> _mockHorarios;
        private Mock<DB_BUENISIMOEntities> _mockContext;

        private List<tbUsuarios> usuarios;
        private List<tbRoles> roles;
        private List<tbHorarios> horarios;

        [TestInitialize]
        public void Setup()
        {
            roles = new List<tbRoles>
    {
        new tbRoles { idRol = 1, nombreRol = "Administrador" }
    };

            usuarios = new List<tbUsuarios>
    {
        new tbUsuarios
        {
            idUsuario = 1,
            nombre = "Juan",
            apellido = "Pérez",
            correoElectronico = "juan.perez@example.com",
            dni = "12345678",
            activo = true,
            idRol = 1
        }
    };

            horarios = new List<tbHorarios>
    {
        new tbHorarios
        {
            idHorario = 1,
            idUsuario = 1,
            diaSemana = 1,
            horaEntrada = new TimeSpan(8, 0, 0),
            horaSalida = new TimeSpan(17, 0, 0),
            pagoPorHora = 10,
            fechaInicioVigencia = DateTime.Today.AddDays(-5),
            activo = true
        }
    };

            // Asignar manualmente el rol al usuario porque Include no se simula automáticamente
            foreach (var usuario in usuarios)
            {
                usuario.tbRoles = roles.FirstOrDefault(r => r.idRol == usuario.idRol);
            }

            _mockUsuarios = MockDbSet(usuarios, u => ((tbUsuarios)(object)u).idUsuario);
            _mockRoles = MockDbSet(roles, r => ((tbRoles)(object)r).idRol);
            _mockHorarios = MockDbSet(horarios, h => ((tbHorarios)(object)h).idHorario);

            _mockContext = new Mock<DB_BUENISIMOEntities>();
            _mockContext.Setup(c => c.tbUsuarios).Returns(_mockUsuarios.Object);
            _mockContext.Setup(c => c.tbRoles).Returns(_mockRoles.Object);
            _mockContext.Setup(c => c.tbHorarios).Returns(_mockHorarios.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
        }


        private Mock<DbSet<T>> MockDbSet<T>(List<T> data, Func<T, object> getKey) where T : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(data.Add);

            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids =>
            {
                var id = ids.First();
                return data.FirstOrDefault(d => getKey(d).Equals(id));
            });

            return mockSet;
        }



        [TestMethod]
        public void ObtenerUsuarioPorId_DeberiaRetornarUsuarioCorrecto()
        {
            var service = new HorarioService(_mockContext.Object);
            var usuario = service.ObtenerUsuarioPorId(1);
            Assert.IsNotNull(usuario);
            Assert.AreEqual("Juan", usuario.nombre);
        }

        [TestMethod]
        public void ObtenerHorariosPorUsuario_DeberiaRetornarLista()
        {
            var service = new HorarioService(_mockContext.Object);
            var resultado = service.ObtenerHorariosPorUsuario(1);
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Lunes", resultado[0].DiaSemanaTexto);
        }

        [TestMethod]
        public void ObtenerHorarioPorId_DeberiaRetornarCorrecto()
        {
            var service = new HorarioService(_mockContext.Object);
            var horario = service.ObtenerHorarioPorId(1);
            Assert.IsNotNull(horario);
            Assert.AreEqual(1, horario.idUsuario);
        }

        [TestMethod]
        public void AgregarHorario_DeberiaAgregarCorrectamente()
        {
            var service = new HorarioService(_mockContext.Object);
            var model = new HorarioFormViewModel
            {
                IdUsuario = 1,
                DiaSemana = 2,
                HoraEntrada = new TimeSpan(9, 0, 0),
                HoraSalida = new TimeSpan(18, 0, 0),
                PagoPorHora = 12,
                FechaInicioVigencia = DateTime.Today,
                Activo = true
            };

            var resultado = service.AgregarHorario(model);
            Assert.IsTrue(resultado);
        }
    }

    // Clases auxiliares para simular consultas async con Entity Framework
    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression) => new TestDbAsyncEnumerable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestDbAsyncEnumerable<TElement>(expression);
        public object Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute(expression));
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestDbAsyncEnumerable(Expression expression) : base(expression) { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator() => new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();
        IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this);
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose() => _inner.Dispose();
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken) => Task.FromResult(_inner.MoveNext());
        public T Current => _inner.Current;
        object IDbAsyncEnumerator.Current => Current;
    }
}

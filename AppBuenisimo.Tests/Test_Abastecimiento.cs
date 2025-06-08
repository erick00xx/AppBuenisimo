using AppBuenisimo.Models;
using AppBuenisimo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AppBuenisimo.Tests
{
    [TestClass]
    public class Test_Abastecimiento
    {
        private Mock<DbSet<tbProductos>> _mockProductosSet;
        private Mock<DbSet<tbTiposProductos>> _mockTiposSet;
        private Mock<DbSet<tbCategorias>> _mockCategoriasSet;
        private Mock<DbSet<tbTiposMedidas>> _mockMedidasSet;
        private Mock<DB_BUENISIMOEntities> _mockContext;
        private ProductoServices _service;

        private List<tbProductos> _productos;
        private List<tbTiposProductos> _tipos;
        private List<tbCategorias> _categorias;
        private List<tbTiposMedidas> _medidas;

        [TestInitialize]
        public void Setup()
        {
            _productos = new List<tbProductos>
            {
                new tbProductos
                {
                    idProducto = 1,
                    nombre = "Café Premium",
                    estado = "activo",
                    tbCategorias = new tbCategorias
                    {
                        nombre = "Clásico",
                        tbTiposProductos = new tbTiposProductos { nombre = "Café" }
                    },
                    tbTiposMedidas = new tbTiposMedidas { nombre = "Gramos" }
                },
                new tbProductos
                {
                    idProducto = 2,
                    nombre = "Café Expreso",
                    estado = "activo",
                    tbCategorias = new tbCategorias
                    {
                        nombre = "Fuerte",
                        tbTiposProductos = new tbTiposProductos { nombre = "Café" }
                    },
                    tbTiposMedidas = new tbTiposMedidas { nombre = "Mililitros" }
                }
            };

            _tipos = new List<tbTiposProductos>
            {
                new tbTiposProductos { idTipoProducto = 1, nombre = "Bebidas" }
            };

            _categorias = new List<tbCategorias>
            {
                new tbCategorias { idCategoria = 1, idTipoProducto = 1, nombre = "Frappé" },
                new tbCategorias { idCategoria = 2, idTipoProducto = 2, nombre = "Capuchino" }
            };

            _medidas = new List<tbTiposMedidas>
            {
                new tbTiposMedidas { idTipoMedida = 1, nombre = "Gramos" }
            };

            // Mock productos
            var queryableProductos = _productos.AsQueryable();
            _mockProductosSet = new Mock<DbSet<tbProductos>>();
            _mockProductosSet.As<IQueryable<tbProductos>>().Setup(m => m.Provider).Returns(queryableProductos.Provider);
            _mockProductosSet.As<IQueryable<tbProductos>>().Setup(m => m.Expression).Returns(queryableProductos.Expression);
            _mockProductosSet.As<IQueryable<tbProductos>>().Setup(m => m.ElementType).Returns(queryableProductos.ElementType);
            _mockProductosSet.As<IQueryable<tbProductos>>().Setup(m => m.GetEnumerator()).Returns(queryableProductos.GetEnumerator());
            _mockProductosSet.Setup(m => m.Add(It.IsAny<tbProductos>())).Callback<tbProductos>(p => _productos.Add(p));

            // Mock tipos
            var queryableTipos = _tipos.AsQueryable();
            _mockTiposSet = new Mock<DbSet<tbTiposProductos>>();
            _mockTiposSet.As<IQueryable<tbTiposProductos>>().Setup(m => m.Provider).Returns(queryableTipos.Provider);
            _mockTiposSet.As<IQueryable<tbTiposProductos>>().Setup(m => m.Expression).Returns(queryableTipos.Expression);
            _mockTiposSet.As<IQueryable<tbTiposProductos>>().Setup(m => m.ElementType).Returns(queryableTipos.ElementType);
            _mockTiposSet.As<IQueryable<tbTiposProductos>>().Setup(m => m.GetEnumerator()).Returns(queryableTipos.GetEnumerator());

            // Mock categorias
            var queryableCategorias = _categorias.AsQueryable();
            _mockCategoriasSet = new Mock<DbSet<tbCategorias>>();
            _mockCategoriasSet.As<IQueryable<tbCategorias>>().Setup(m => m.Provider).Returns(queryableCategorias.Provider);
            _mockCategoriasSet.As<IQueryable<tbCategorias>>().Setup(m => m.Expression).Returns(queryableCategorias.Expression);
            _mockCategoriasSet.As<IQueryable<tbCategorias>>().Setup(m => m.ElementType).Returns(queryableCategorias.ElementType);
            _mockCategoriasSet.As<IQueryable<tbCategorias>>().Setup(m => m.GetEnumerator()).Returns(queryableCategorias.GetEnumerator());

            // Mock medidas
            var queryableMedidas = _medidas.AsQueryable();
            _mockMedidasSet = new Mock<DbSet<tbTiposMedidas>>();
            _mockMedidasSet.As<IQueryable<tbTiposMedidas>>().Setup(m => m.Provider).Returns(queryableMedidas.Provider);
            _mockMedidasSet.As<IQueryable<tbTiposMedidas>>().Setup(m => m.Expression).Returns(queryableMedidas.Expression);
            _mockMedidasSet.As<IQueryable<tbTiposMedidas>>().Setup(m => m.ElementType).Returns(queryableMedidas.ElementType);
            _mockMedidasSet.As<IQueryable<tbTiposMedidas>>().Setup(m => m.GetEnumerator()).Returns(queryableMedidas.GetEnumerator());

            _mockContext = new Mock<DB_BUENISIMOEntities>();
            _mockContext.Setup(c => c.tbProductos).Returns(_mockProductosSet.Object);
            _mockContext.Setup(c => c.tbTiposProductos).Returns(_mockTiposSet.Object);
            _mockContext.Setup(c => c.tbCategorias).Returns(_mockCategoriasSet.Object);
            _mockContext.Setup(c => c.tbTiposMedidas).Returns(_mockMedidasSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Callback(() => { });

            _service = new ProductoServices(_mockContext.Object);
        }

        [TestMethod]
        public void OcultarProducto_ProductoExiste_EstadoCambiadoAInactivo()
        {
            var result = _service.OcultarProducto(1);
            Assert.IsTrue(result);
            Assert.AreEqual("inactivo", _productos.First(p => p.idProducto == 1).estado);
        }

        [TestMethod]
        public void OcultarProducto_ProductoNoExiste_DevuelveFalse()
        {
            var result = _service.OcultarProducto(999);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MostrarProducto_ProductoExiste_EstadoCambiadoAActivo()
        {
            _productos[0].estado = "inactivo";
            var result = _service.MostrarProducto(1);
            Assert.IsTrue(result);
            Assert.AreEqual("activo", _productos.First(p => p.idProducto == 1).estado);
        }

        [TestMethod]
        public void MostrarProducto_ProductoNoExiste_DevuelveFalse()
        {
            var result = _service.MostrarProducto(999);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TiposProductos_DevuelveCorrecto()
        {
            var result = _service.TiposProductos();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Bebidas", result[0].nombre);
        }

        [TestMethod]
        public void CategoriasPorTipo_FiltraCorrectamente()
        {
            var result = _service.CategoriasPorTipo(1);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Frappé", result[0].nombre);
        }

        [TestMethod]
        public void TiposMedidas_DevuelveCorrecto()
        {
            var result = _service.TiposMedidas();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Gramos", result[0].nombre);
        }

        [TestMethod]
        public void GuardarProducto_AgregaYGuarda()
        {
            var nuevo = new tbProductos { idProducto = 3, nombre = "Capuchino" };
            _service.GuardarProducto(nuevo);
            Assert.IsTrue(_productos.Any(p => p.nombre == "Capuchino"));
        }

        [TestMethod]
        public void ListarProductos_DevuelveDTOCorrectos()
        {
            var result = _service.ListarProductos();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Café Premium", result[0].NombreProducto);
            Assert.AreEqual("Café", result[0].TipoProductoNombre);
            Assert.AreEqual("Clásico", result[0].CategoriaNombre);
            Assert.AreEqual("Gramos", result[0].TipoMedidaNombre);
        }
    }
}

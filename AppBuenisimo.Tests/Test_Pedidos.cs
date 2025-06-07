using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using AppBuenisimo.Services;
using AppBuenisimo.Tests.Helpers;
using AppBuenisimo.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace AppBuenisimo.Tests
{
    [TestClass]
    public class Test_Pedidos
    {
        private Mock<DB_BUENISIMOEntities> _mockContext;
        private PedidoService _pedidoService;

        // Listas de datos en memoria para simular las tablas de la BD
        private List<tbMesas> _mesasData;
        private List<tbPedidos> _pedidosData;
        private List<tbDetallePedido> _detallesPedidoData;

        [TestInitialize]
        public void Initialize()
        {
            // Se ejecuta ANTES de cada prueba
            _mesasData = new List<tbMesas>();
            _pedidosData = new List<tbPedidos>();
            _detallesPedidoData = new List<tbDetallePedido>();

            var mockMesasSet = MockDbSetHelper.CreateMockSet(_mesasData.AsQueryable());
            var mockPedidosSet = MockDbSetHelper.CreateMockSet(_pedidosData.AsQueryable());
            var mockDetallesSet = MockDbSetHelper.CreateMockSet(_detallesPedidoData.AsQueryable());

            // Configurar los callbacks para que Add() modifique nuestras listas en memoria
            mockPedidosSet.Setup(m => m.Add(It.IsAny<tbPedidos>()))
                          .Callback<tbPedidos>(p =>
                          {
                              p.idPedido = _pedidosData.Count + 1; // Simular autoincremento de ID
                              _pedidosData.Add(p);
                          });

            mockDetallesSet.Setup(m => m.Add(It.IsAny<tbDetallePedido>()))
                           .Callback<tbDetallePedido>(d => _detallesPedidoData.Add(d));

            _mockContext = new Mock<DB_BUENISIMOEntities>();
            _mockContext.Setup(c => c.tbMesas).Returns(mockMesasSet.Object);
            _mockContext.Setup(c => c.tbPedidos).Returns(mockPedidosSet.Object);
            _mockContext.Setup(c => c.tbDetallePedido).Returns(mockDetallesSet.Object);

            _pedidoService = new PedidoService(_mockContext.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Limpia el tiempo estático para no afectar otras clases de prueba
            Utils.TimeProvider.ResetToDefault();
        }

        // === PRUEBAS PARA CrearPedido ===

        [TestMethod]
        public void CrearPedido_ConDatosValidos_DebeCrearPedidoYDetallesCorrectamente()
        {
            // Arrange
            int idUsuario = 1;
            string codMesa = "M01";
            int idSucursal = 1;
            Utils.TimeProvider.SetCustomTime(new System.DateTime(2023, 1, 1, 10, 0, 0));

            // Simular una mesa existente
            _mesasData.Add(new tbMesas { codMesa = codMesa, idSucursal = idSucursal });

            var detallesVM = new List<DetallePedidoViewModel>
            {
                new DetallePedidoViewModel { IdPrecio = 101, Cantidad = 2, Subtotal = 10.00m },
                new DetallePedidoViewModel { IdPrecio = 102, Cantidad = 1, Subtotal = 7.50m }
            };

            // Act
            _pedidoService.CrearPedido(idUsuario, codMesa, detallesVM, idSucursal);

            // Assert
            // 1. Verificar que SaveChanges se llamó dos veces (una para el pedido, otra para los detalles)
            _mockContext.Verify(c => c.SaveChanges(), Times.Exactly(2), "SaveChanges debe ser llamado 2 veces.");

            // 2. Verificar que se creó un pedido
            Assert.AreEqual(1, _pedidosData.Count, "Se debería haber creado un pedido en la lista de datos.");
            var pedidoCreado = _pedidosData.First();
            Assert.AreEqual(codMesa, pedidoCreado.codMesa);
            Assert.AreEqual(idUsuario, pedidoCreado.idUsuario);
            Assert.AreEqual(17.50m, pedidoCreado.total, "El total del pedido es incorrecto.");
            Assert.AreEqual(1, pedidoCreado.idEstadoPedido, "El estado inicial del pedido debe ser 1 (Pendiente).");

            // 3. Verificar que se crearon los detalles del pedido
            Assert.AreEqual(2, _detallesPedidoData.Count, "Se deberían haber creado dos detalles de pedido.");
            Assert.IsTrue(_detallesPedidoData.All(d => d.idPedido == pedidoCreado.idPedido), "Todos los detalles deben estar asociados al nuevo pedido.");
            Assert.IsNotNull(_detallesPedidoData.FirstOrDefault(d => d.idPrecio == 101 && d.cantidad == 2), "Falta el detalle del producto 101.");
            Assert.IsNotNull(_detallesPedidoData.FirstOrDefault(d => d.idPrecio == 102 && d.cantidad == 1), "Falta el detalle del producto 102.");
        }

        // === PRUEBAS PARA ListarMesasDisponiblesYActual ===

        [TestMethod]
        public void ListarMesasDisponibles_SinPedidosActivos_DebeDevolverTodasLasMesasDeLaSucursal()
        {
            // Arrange
            int idSucursal = 1;
            _mesasData.Add(new tbMesas { codMesa = "M01", idSucursal = idSucursal });
            _mesasData.Add(new tbMesas { codMesa = "M02", idSucursal = idSucursal });
            _mesasData.Add(new tbMesas { codMesa = "M03", idSucursal = 2 }); // Otra sucursal, no debe aparecer

            // Act
            var mesasDisponibles = _pedidoService.ListarMesasDisponiblesYActual(idSucursal);

            // Assert
            Assert.AreEqual(2, mesasDisponibles.Count, "Debería devolver solo las 2 mesas de la sucursal 1.");
            Assert.IsNotNull(mesasDisponibles.FirstOrDefault(m => m.codMesa == "M01"));
            Assert.IsNotNull(mesasDisponibles.FirstOrDefault(m => m.codMesa == "M02"));
        }

        [TestMethod]
        public void ListarMesasDisponibles_ConUnaMesaOcupada_DebeExcluirLaMesaOcupada()
        {
            // Arrange
            int idSucursal = 1;
            var mesa1 = new tbMesas { codMesa = "M01", idSucursal = idSucursal };
            var mesa2 = new tbMesas { codMesa = "M02", idSucursal = idSucursal };
            _mesasData.Add(mesa1);
            _mesasData.Add(mesa2);

            // Simular un pedido activo en la mesa M01
            _pedidosData.Add(new tbPedidos
            {
                codMesa = "M01",
                idEstadoPedido = 1, // Estado activo
                tbMesas = mesa1 // Relación para que el Where funcione
            });

            // Act
            var mesasDisponibles = _pedidoService.ListarMesasDisponiblesYActual(idSucursal);

            // Assert
            Assert.AreEqual(1, mesasDisponibles.Count, "Solo la mesa M02 debería estar disponible.");
            Assert.AreEqual("M02", mesasDisponibles.First().codMesa);
        }

        [TestMethod]
        public void ListarMesasDisponibles_EditandoPedido_DebeIncluirLaMesaActualDelPedido()
        {
            // Arrange
            int idSucursal = 1;
            string mesaActualEnEdicion = "M01";

            var mesa1 = new tbMesas { codMesa = "M01", idSucursal = idSucursal };
            var mesa2 = new tbMesas { codMesa = "M02", idSucursal = idSucursal };
            _mesasData.Add(mesa1);
            _mesasData.Add(mesa2);

            // Simular que M01 y M02 están ocupadas por pedidos activos
            _pedidosData.Add(new tbPedidos { codMesa = "M01", idEstadoPedido = 1, tbMesas = mesa1 });
            _pedidosData.Add(new tbPedidos { codMesa = "M02", idEstadoPedido = 2, tbMesas = mesa2 });

            // Act
            // Llamamos al método indicando que estamos editando el pedido de la mesa M01
            var mesasDisponibles = _pedidoService.ListarMesasDisponiblesYActual(idSucursal, mesaActualEnEdicion);

            // Assert
            Assert.AreEqual(1, mesasDisponibles.Count, "Solo la mesa M01 (la que se está editando) debería estar en la lista.");
            Assert.AreEqual("M01", mesasDisponibles.First().codMesa, "La mesa devuelta debe ser la que se está editando.");
        }
    }
}
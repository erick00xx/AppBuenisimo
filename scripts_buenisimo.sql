create database DB_BUENISIMO;

USE DB_BUENISIMO;

----------- GESTIONAR CUENTAS
create table tbRoles(
	idRol int identity(1,1) primary key,
	nombreRol nvarchar(50)  null unique
);
CREATE TABLE tbObservacionesAsistencias(
	idObservacionAsistencia int identity(1,1) primary key,
	descripcion nvarchar(100) not null unique
);

CREATE TABLE tbUsuarios (
    idUsuario INT IDENTITY(1,1) PRIMARY KEY,
	idRol int not null,
    nombre NVARCHAR(100) NULL,
    apellido NVARCHAR(100) NULL,
    correoElectronico NVARCHAR(150) UNIQUE NOT NULL,
    contrasena NVARCHAR(255) NOT NULL,
    fechaRegistro DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1,
	dni NVARCHAR(20) NULL UNIQUE
	CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (idRol) REFERENCES tbRoles(idRol)
);

create table tbSucursales(
	idSucursal int primary key,
	nombre nvarchar(100) not null
);

--------------------- GESTIONAR ASISTENCIAS
create table tbAsistencias(
	idAsistencia int identity(1,1) primary key,
	idUsuario int not null,
	idSucursal int not null,
	idObservacionAsistencia int not null,
	fecha date not null,
	horaEntrada datetime null,
	horaSalida datetime null,
	CONSTRAINT FK_Asistencias_Usuarios FOREIGN KEY (idUsuario) REFERENCES tbUsuarios(idUsuario),
	CONSTRAINT FK_Asistencias_Sucursales FOREIGN KEY (idSucursal) REFERENCES tbSucursales(idSucursal),
	CONSTRAINT FK_Asistencias_Observaciones FOREIGN KEY (idObservacionAsistencia) REFERENCES tbObservacionesAsistencias(idObservacionAsistencia)
);

CREATE TABLE tbHorarios(
	idHorario int identity(1,1) primary key,
	idUsuario int not null,
	diaSemana TINYINT not null,
	horaEntradaEsperada Time not null,
	horaSalidaEsperada Time not null,
	CONSTRAINT FK_Horarios_Usuarios FOREIGN KEY(idUsuario) REFERENCES tbUsuarios(idUsuario)
);

-- Insertar roles
INSERT INTO tbRoles (nombreRol) VALUES 
('Admin'),
('Encargado'),
('Empleado');

-- Insertar observaciones de asistencia
INSERT INTO tbObservacionesAsistencias (descripcion) VALUES 
('Asistencia normal'),
('Llegada tarde'),
('Salida anticipada'),
('Inasistencia justificada'),
('Inasistencia injustificada');

-- Insertar sucursales
INSERT INTO tbSucursales (idSucursal, nombre) VALUES 
(1, 'Buenisimo 1'),
(2, 'Buenisimo 2')

-- Insertar usuarios
INSERT INTO tbUsuarios (idRol, nombre, apellido, correoElectronico, contrasena, dni) VALUES 
(1, 'Juan', 'Pérez', 'juan.perez@example.com', '123', '12345678'),
(2, 'María', 'López', 'maria.lopez@example.com', '123', '87654321'),
(3, 'Carlos', 'Gómez', 'carlos.gomez@example.com', '123', '11223344'),
(3, 'Ana', 'Martínez', 'ana.martinez@example.com', '123', '44332211');

-- Insertar horarios
INSERT INTO tbHorarios (idUsuario, diaSemana, horaEntradaEsperada, horaSalidaEsperada) VALUES 
(3, 1, '08:00', '17:00'),
(3, 2, '08:00', '17:00'),
(3, 3, '08:00', '17:00'),
(4, 1, '09:00', '18:00'),
(4, 2, '09:00', '18:00');

-- Insertar asistencias
INSERT INTO tbAsistencias (idUsuario, idSucursal, idObservacionAsistencia, fecha) VALUES 
(3, 1, 1, '2025-04-28'),
(3, 1, 2, '2025-04-29'),
(4, 2, 1, '2025-04-28');





------------------- PARA GESTION DE INVENTARIO

-- Tabla: unidades de medida
CREATE TABLE tbUnidades (
    idUnidad INT identity(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL unique,
    tipo VARCHAR(20) NOT NULL, -- peso, volumen, cantidad
    abreviatura VARCHAR(10) NOT NULL
);

-- Tabla: insumos
CREATE TABLE tbInsumos (
    idInsumo INT identity(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL unique,
    idUnidad INT NOT NULL,
    tipoInsumo VARCHAR(50), -- materia prima, empaque, perecedero, etc.
    descripcion TEXT,
    FOREIGN KEY (idUnidad) REFERENCES tbUnidades(idUnidad)
);

-- Tabla: proveedores
CREATE TABLE tbProveedores (
    idProveedor INT identity(1,1) PRIMARY KEY,
    nombreEmpresa VARCHAR(100) NOT NULL,
    contacto VARCHAR(100),
    telefono VARCHAR(20),
    email VARCHAR(100)
);

-- Tabla: ingresos de insumos (compras)
CREATE TABLE tbIngresosInsumos (
    idIngresoInsumo INT identity(1,1) PRIMARY KEY,
	idSucursal INT,
    idInsumo INT NOT NULL,
    cantidad DECIMAL(10,2) NOT NULL,
    idUnidad INT NOT NULL,
    fechaCompra DATE NOT NULL,
    fechaVencimiento DATE,
    idProveedor INT,
    lote VARCHAR(50),
    observaciones TEXT,
	FOREIGN KEY (idSucursal) REFERENCES tbSucursales(idSucursal),
    FOREIGN KEY (idInsumo) REFERENCES tbInsumos(idInsumo),
    FOREIGN KEY (idUnidad) REFERENCES tbUnidades(idUnidad),
    FOREIGN KEY (idProveedor) REFERENCES tbProveedores(idProveedor)
);

-- Tabla: desechos de insumos
CREATE TABLE tbDesechosInsumos (
    idDesechoInsumo INT identity(1,1) PRIMARY KEY,
	idSucursal INT,
    idInsumo INT NOT NULL,
    cantidad DECIMAL(10,2) NOT NULL,
    idUnidad INT NOT NULL,
    fechaDesecho DATE NOT NULL,
    motivo VARCHAR(100) NOT NULL,
    observaciones TEXT,
    idIngresoInsumo INT,
	FOREIGN KEY (idSucursal) REFERENCES tbSucursales(idSucursal),
    FOREIGN KEY (idInsumo) REFERENCES tbInsumos(idInsumo),
    FOREIGN KEY (idUnidad) REFERENCES tbUnidades(idUnidad),
    FOREIGN KEY (idIngresoInsumo) REFERENCES tbIngresosInsumos(idIngresoInsumo)
);


-- Insertar unidades de medida
INSERT INTO tbUnidades (nombre, tipo, abreviatura) VALUES
('Unidad', 'cantidad', 'un'),
('Kilogramo', 'peso', 'kg'),
('Litro', 'volumen', 'L'),
('Gramo', 'peso', 'g'),
('Mililitro', 'volumen', 'ml');

-- Insertar insumos
INSERT INTO tbInsumos (nombre, idUnidad, tipoInsumo, descripcion) VALUES
('Café en grano', 2, 'materia prima', 'Café Arábica Premium'),
('Vaso descartable 8oz', 1, 'empaque', 'Vaso térmico desechable'),
('Leche entera', 3, 'perecedero', 'Leche pasteurizada de vaca'),
('Azúcar blanca', 2, 'materia prima', 'Azúcar refinada'),
('Jarabe de vainilla', 5, 'materia prima', 'Saborizante líquido');

-- Insertar proveedores
INSERT INTO tbProveedores (nombreEmpresa, contacto, telefono, email) VALUES
('Café Perú S.A.', 'Carlos Méndez', '987654321', 'ventas@cafeperu.com'),
('Lácteos del Sur', 'Juan Torres', '912345678', 'ventas@lacteosur.com'),
('Empaques Express', 'Lucía Díaz', '934567890', 'contacto@empaquesexpress.com');

-- Insertar ingresos de insumos (compras)
INSERT INTO tbIngresosInsumos (idInsumo, cantidad, idUnidad, fechaCompra, fechaVencimiento, idProveedor, lote, observaciones) VALUES
(1, 5.00, 2, '2025-05-01', NULL, 1, 'CG202505', 'Café tostado en grano'),
(2, 200, 1, '2025-05-01', NULL, 3, 'VD202505', 'Caja con 200 vasos'),
(3, 10.00, 3, '2025-05-01', '2025-06-01', 2, 'LT202505', 'Refrigerado, mantener frío'),
(4, 20.00, 2, '2025-05-01', NULL, 1, 'AZ202505', 'Saco de 20kg'),
(5, 2.50, 5, '2025-05-01', '2025-07-15', 1, 'JV202505', 'Botellas de 500ml');

-- Insertar desechos de insumos
INSERT INTO tbDesechosInsumos (idInsumo, cantidad, idUnidad, fechaDesecho, motivo, observaciones, idIngresoInsumo) VALUES
(3, 2.00, 3, '2025-05-03', 'vencimiento', 'Lote caducado', 3),
(1, 0.50, 2, '2025-05-03', 'mal estado', 'Se derramó parte del saco', 1),
(5, 0.25, 5, '2025-05-03', 'contaminación', 'Sabor alterado', 5);


------------------------------------ CARTA DE BUENISIMO
CREATE TABLE tbTiposProductos(
	idTipoProducto INT PRIMARY KEY,
	nombre NVARCHAR(30)
);

CREATE TABLE tbCategorias (
    idCategoria INT PRIMARY KEY IDENTITY,
    nombre NVARCHAR(50) NOT NULL,
	idTipoProducto INT FOREIGN KEY REFERENCES tbTiposProductos(idTipoProducto)
);
-- Define cómo se vende cada producto
CREATE TABLE tbTiposMedidas (
    idTipoMedida INT PRIMARY KEY IDENTITY,
    nombre NVARCHAR(30) NOT NULL -- Ej: Tamaño, Tipo (Simple/Doble), Único
);
-- Define valores posibles por tipo de medida
CREATE TABLE tbMedidas (
    idMedida INT PRIMARY KEY IDENTITY,
    idTipoMedida INT FOREIGN KEY REFERENCES tbTiposMedidas(idTipoMedida),
    nombre NVARCHAR(30) NOT NULL -- Ej: REG, MED, GR, Simple, Doble
);
CREATE TABLE tbProductos (
    idProducto INT PRIMARY KEY IDENTITY,
    nombre NVARCHAR(100) NOT NULL,
    idCategoria INT FOREIGN KEY REFERENCES tbCategorias(idCategoria),
    idTipoMedida INT FOREIGN KEY REFERENCES tbTiposMedidas(idTipoMedida),
    descripcion NVARCHAR(255)
);
CREATE TABLE tbPrecios (
    IdPrecio INT PRIMARY KEY IDENTITY,
    IdProducto INT FOREIGN KEY REFERENCES tbProductos(IdProducto),
    IdMedida INT FOREIGN KEY REFERENCES tbMedidas(IdMedida) NULL, -- Puede ser NULL si no tiene variantes
    Precio DECIMAL(6,2) NOT NULL
);

ALTER TABLE tbProductos
ADD estado NVARCHAR(10) NOT NULL DEFAULT 'activo';




-----------------------------------ORDEN DE PEDIDO



CREATE TABLE tbMesas (
    idMesa INT PRIMARY KEY IDENTITY,
    numeroMesa INT NOT NULL,       -- Número de la mesa, ejemplo: mesa 1, mesa 2, etc.
    estado NVARCHAR(20) NULL   -- Puede ser "Libre", "Ocupada", "Reservada", etc.
);
CREATE TABLE tbPedidos (
    idPedido INT PRIMARY KEY IDENTITY,
    idMesa INT FOREIGN KEY REFERENCES tbMesas(idMesa),  -- Relacionado con la mesa en la que se realizó el pedido
    fechaPedido DATETIME NULL,                      -- Fecha y hora en que se hizo el pedido
    estado NVARCHAR(20)NULL,                        -- Estado del pedido, ejemplo: "En preparación", "Entregado"
    usuarioId INT  NULL,                               -- Puede ser el ID del usuario que hizo el pedido
	total DECIMAL(6,2)
);

CREATE TABLE tbDetallePedido (
    idDetalle INT PRIMARY KEY IDENTITY,
    idPedido INT FOREIGN KEY REFERENCES tbPedidos(idPedido),  -- Relacionado con el pedido
    idPrecio INT FOREIGN KEY REFERENCES tbPrecios(IdPrecio),  -- Relacionado con el producto que se pidió
    cantidad INT NULL,                            -- La cantidad de productos solicitados
    subtotal DECIMAL(6,2) NULL,                     -- El precio del producto en ese momento
);
CREATE TABLE tbEstadosPedidos (
    idEstadoPedido INT PRIMARY KEY IDENTITY,
    estado NVARCHAR(50) NOT NULL  -- Ejemplo: "En espera", "En preparación", "Listo para entregar", "Entregado"
);




--Para hacer pedidos ( tbMesas, tbPedidos, tbDetallePedido


-- 1. Renombrar la columna 'usuarioId' a 'idUsuario'
EXEC sp_rename 'tbPedidos.usuarioId', 'idUsuario', 'COLUMN';

-- 2. Agregar la clave foránea para relacionar con tbUsuario(idUsuario)
ALTER TABLE tbPedidos
ADD CONSTRAINT FK_tbPedidos_tbUsuario
FOREIGN KEY (idUsuario) REFERENCES tbUsuarios(idUsuario);


-- 1. Eliminar la columna 'estado'
ALTER TABLE tbPedidos
DROP COLUMN estado;

-- 2. Agregar la nueva columna 'idEstadoPedido'
ALTER TABLE tbPedidos
ADD idEstadoPedido INT;

-- 3. Crear la clave foránea que relaciona con 'tbEstadosPedidos(idEstadoPedido)'
ALTER TABLE tbPedidos
ADD CONSTRAINT FK_tbPedidos_tbEstadosPedidos
FOREIGN KEY (idEstadoPedido) REFERENCES tbEstadosPedidos(idEstadoPedido);







--=================================================================
-- TABLAS PARA GESTIÓN DE VENTAS
--=================================================================

-- Tabla para los métodos de pago (Opcional pero muy recomendada)
CREATE TABLE [tbMetodosPago] (
    [idMetodoPago] INT IDENTITY(1,1) NOT NULL,
    [nombre] NVARCHAR(50) NOT NULL,
    PRIMARY KEY ([idMetodoPago]),
    UNIQUE ([nombre])
);
GO

-- Datos iniciales para métodos de pago
INSERT INTO [tbMetodosPago] ([nombre]) VALUES
('Efectivo'),
('Tarjeta de Crédito/Débito'),
('Yape'),
('Plin');
GO


-- Tabla principal de Ventas (Cabecera)
-- Aquí se guarda una copia de la información del pedido cuando se culmina.
CREATE TABLE [tbVentas] (
    [idVenta] INT IDENTITY(1,1) NOT NULL,
    [idPedidoOriginal] INT NULL,              -- FK al pedido original para trazabilidad.
    [codMesa] NVARCHAR(20) NULL,
    [idSucursal] INT NOT NULL,
    [idUsuarioVenta] INT NOT NULL,            -- Usuario que registró la venta (puede ser el mismo del pedido).
    [fechaVenta] DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora en que se concretó la venta.
    [total] DECIMAL(10,2) NOT NULL,
    [idMetodoPago] INT NULL,

    -- Campos para Auditoría de modificaciones
    [fueModificada] BIT NOT NULL DEFAULT 0,
    [fechaModificacion] DATETIME NULL,
    [idUsuarioModificacion] INT NULL,         -- Quien modificó la venta.
    [motivoModificacion] NVARCHAR(255) NULL,

    PRIMARY KEY ([idVenta]),
    CONSTRAINT [FK_tbVentas_tbPedidos] FOREIGN KEY ([idPedidoOriginal]) REFERENCES [tbPedidos]([idPedido]),
    CONSTRAINT [FK_tbVentas_tbSucursales] FOREIGN KEY ([idSucursal]) REFERENCES [tbSucursales]([idSucursal]),
    CONSTRAINT [FK_tbVentas_tbUsuarios_Venta] FOREIGN KEY ([idUsuarioVenta]) REFERENCES [tbUsuarios]([idUsuario]),
    CONSTRAINT [FK_tbVentas_tbMetodosPago] FOREIGN KEY ([idMetodoPago]) REFERENCES [tbMetodosPago]([idMetodoPago]),
    CONSTRAINT [FK_tbVentas_tbUsuarios_Modificacion] FOREIGN KEY ([idUsuarioModificacion]) REFERENCES [tbUsuarios]([idUsuario]),
    -- Para asegurar que un pedido no se convierta en venta más de una vez
    UNIQUE ([idPedidoOriginal])
);
GO


-- Tabla de Detalle de Venta
-- Es CRUCIAL para guardar los detalles tal como estaban en el momento de la venta.
CREATE TABLE [tbDetalleVenta] (
    [idDetalleVenta] INT IDENTITY(1,1) NOT NULL,
    [idVenta] INT NOT NULL,

    -- Datos 'congelados' del producto
    [idProductoOriginal] INT NULL,          -- Referencia al producto, puede ser nulo si el producto se borra.
    [descripcionProducto] NVARCHAR(100) NOT NULL, -- El nombre del producto en ese momento.
    [cantidad] INT NOT NULL,
    [precioUnitario] DECIMAL(6,2) NOT NULL, -- El precio unitario en ese momento.

    -- Datos 'congelados' de los agregados
    [descripcionAgregados] NVARCHAR(200) NULL, -- Ej: "Leche, Caramelo, Escn. Vainilla"
    [precioTotalAgregados] DECIMAL(6,2) NOT NULL DEFAULT 0, -- La suma del precio de todos los agregados.

    -- Datos 'congelados' de otras personalizaciones
    [tipoLeche] VARCHAR(20) NULL,
    [tipoAzucar] VARCHAR(20) NULL,
    [cantidadHielo] VARCHAR(20) NULL,

    [subtotal] DECIMAL(8,2) NOT NULL,

    PRIMARY KEY ([idDetalleVenta]),
    CONSTRAINT [FK_tbDetalleVenta_tbVentas] FOREIGN KEY ([idVenta]) REFERENCES [tbVentas]([idVenta]) ON DELETE CASCADE
);
GO



CREATE PROCEDURE sp_CulminarPedidoYGenerarVenta
    @idPedidoACulminar INT,
    @idMetodoPago INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificamos que el pedido exista y no esté ya culminado
    IF NOT EXISTS (SELECT 1 FROM tbPedidos WHERE idPedido = @idPedidoACulminar AND idEstadoPedido <> 5)
    BEGIN
        -- Lanzamos un error que la aplicación puede capturar
        RAISERROR('El pedido no existe o ya ha sido culminado.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. Actualizamos el estado del pedido a 'Culminado'
        UPDATE tbPedidos
        SET idEstadoPedido = 5
        WHERE idPedido = @idPedidoACulminar;

        -- 2. Insertamos la cabecera de la venta, ahora con el método de pago correcto
        INSERT INTO tbVentas (idPedidoOriginal, codMesa, idSucursal, idUsuarioVenta, total, idMetodoPago)
        SELECT
            p.idPedido, p.codMesa, p.idSucursal, p.idUsuario, p.total, @idMetodoPago
        FROM
            tbPedidos p
        WHERE
            p.idPedido = @idPedidoACulminar;

        DECLARE @NuevaVentaID INT = SCOPE_IDENTITY();

        -- 3. Insertamos los detalles de la venta (la lógica de "congelar" los datos)
        INSERT INTO tbDetalleVenta (
            idVenta, idProductoOriginal, descripcionProducto, cantidad, precioUnitario,
            descripcionAgregados, precioTotalAgregados,
            tipoLeche, tipoAzucar, cantidadHielo, subtotal
        )
        SELECT
            @NuevaVentaID,
            prod.idProducto,
            prod.nombre AS descripcionProducto,
            dp.cantidad,
            pre.Precio AS precioUnitario,
            ISNULL(ag1.nombre, '') + IIF(ag2.nombre IS NULL, '', ', ' + ag2.nombre) + IIF(ag3.nombre IS NULL, '', ', ' + ag3.nombre),
            ISNULL(ag1.precio, 0) + ISNULL(ag2.precio, 0) + ISNULL(ag3.precio, 0),
            dp.tipoLeche, dp.tipoAzucar, dp.cantidadHielo, dp.subtotal
        FROM
            tbDetallePedido dp
        JOIN tbPrecios pre ON dp.idPrecio = pre.IdPrecio
        JOIN tbProductos prod ON pre.IdProducto = prod.idProducto
        LEFT JOIN tbAgregados ag1 ON dp.idAgregado1 = ag1.idAgregado
        LEFT JOIN tbAgregados ag2 ON dp.idAgregado2 = ag2.idAgregado
        LEFT JOIN tbAgregados ag3 ON dp.idAgregado3 = ag3.idAgregado
        WHERE
            dp.idPedido = @idPedidoACulminar;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        -- Re-lanzamos el error para que la aplicación se entere de que algo salió mal
        THROW;
    END CATCH
END
GO


-- Ejecuta el SP
EXEC sp_CulminarPedidoYGenerarVenta @idPedidoACulminar = 34, @idMetodoPago = 1;




-- Encontrar ventas cuyo usuario ya no existe
SELECT * 
FROM tbVentas v
LEFT JOIN tbUsuarios u ON v.idUsuarioVenta = u.idUsuario
WHERE u.idUsuario IS NULL;

-- Encontrar ventas cuya sucursal ya no existe
SELECT *
FROM tbVentas v
LEFT JOIN tbSucursales s ON v.idSucursal = s.idSucursal
WHERE s.idSucursal IS NULL;

-- Encontrar ventas cuyo usuario de modificación ya no existe
SELECT * 
FROM tbVentas v
LEFT JOIN tbUsuarios u ON v.idUsuarioModificacion = u.idUsuario
WHERE v.idUsuarioModificacion IS NOT NULL AND u.idUsuario IS NULL;





-- esta tabla aun nooo
CREATE TABLE tbStockActual (
    idInsumo INT PRIMARY KEY,
    cantidadActual DECIMAL(10,2) NOT NULL DEFAULT 0,
    idUnidad INT NOT NULL,
    FOREIGN KEY (idInsumo) REFERENCES tbInsumos(idInsumo),
    FOREIGN KEY (idUnidad) REFERENCES tbUnidades(idUnidad)
);

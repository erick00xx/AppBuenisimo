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














-- esta tabla aun nooo
CREATE TABLE tbStockActual (
    idInsumo INT PRIMARY KEY,
    cantidadActual DECIMAL(10,2) NOT NULL DEFAULT 0,
    idUnidad INT NOT NULL,
    FOREIGN KEY (idInsumo) REFERENCES tbInsumos(idInsumo),
    FOREIGN KEY (idUnidad) REFERENCES tbUnidades(idUnidad)
);
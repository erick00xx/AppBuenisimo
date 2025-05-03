create database DB_BUENISIMO;
USE DB_BUENISIMO;

create table tbRoles(
	idRol int identity(1,1) primary key,
	nombreRol nvarchar(50) not null unique
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
INSERT INTO tbAsistencias (idUsuario, idSucursal, idObservacionAsistencia, fecha, horaEntrada, horaSalida) VALUES 
(3, 1, 1, '2025-04-28', '2025-04-28 08:01:00', '2025-04-28 17:02:00'),
(3, 1, 2, '2025-04-29', '2025-04-29 08:20:00', '2025-04-29 17:00:00'),
(4, 2, 1, '2025-04-28', '2025-04-28 09:00:00', '2025-04-28 18:01:00');

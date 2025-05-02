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


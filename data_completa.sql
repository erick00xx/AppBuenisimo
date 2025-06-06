
USE [DB_BUENISIMO]
GO
/****** Object:  Table [dbo].[tbAsistencias]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbAsistencias](
	[idAsistencia] [int] IDENTITY(1,1) NOT NULL,
	[idUsuario] [int] NOT NULL,
	[idSucursal] [int] NOT NULL,
	[idObservacionAsistencia] [int] NOT NULL,
	[fecha] [date] NOT NULL,
	[horaEntrada] [datetime] NULL,
	[horaSalida] [datetime] NULL,
	[minutosTardanza] [int] NULL,
	[idHorarioAplicado] [int] NULL,
	[comentariosAdicionales] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[idAsistencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbCategorias]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbCategorias](
	[idCategoria] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](50) NOT NULL,
	[idTipoProducto] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbDesechosInsumos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbDesechosInsumos](
	[idDesechoInsumo] [int] IDENTITY(1,1) NOT NULL,
	[idSucursal] [int] NULL,
	[idInsumo] [int] NOT NULL,
	[cantidad] [decimal](10, 2) NOT NULL,
	[idUnidad] [int] NOT NULL,
	[fechaDesecho] [date] NOT NULL,
	[motivo] [varchar](100) NOT NULL,
	[observaciones] [text] NULL,
	[idIngresoInsumo] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idDesechoInsumo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbDetallePedido]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbDetallePedido](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idPedido] [int] NULL,
	[idPrecio] [int] NULL,
	[cantidad] [int] NULL,
	[subtotal] [decimal](6, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbEstadosPedidos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbEstadosPedidos](
	[idEstadoPedido] [int] IDENTITY(1,1) NOT NULL,
	[estado] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idEstadoPedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbHorarios]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbHorarios](
	[idHorario] [int] IDENTITY(1,1) NOT NULL,
	[idUsuario] [int] NOT NULL,
	[diaSemana] [tinyint] NOT NULL,
	[horaEntrada] [time](7) NOT NULL,
	[horaSalida] [time](7) NOT NULL,
	[pagoPorHora] [decimal](10, 2) NOT NULL,
	[fechaInicioVigencia] [date] NOT NULL,
	[fechaFinVigencia] [date] NULL,
	[activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idHorario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbIngresosInsumos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbIngresosInsumos](
	[idIngresoInsumo] [int] IDENTITY(1,1) NOT NULL,
	[idSucursal] [int] NULL,
	[idInsumo] [int] NOT NULL,
	[cantidad] [decimal](10, 2) NOT NULL,
	[idUnidad] [int] NOT NULL,
	[fechaCompra] [date] NOT NULL,
	[fechaVencimiento] [date] NULL,
	[idProveedor] [int] NULL,
	[lote] [varchar](50) NULL,
	[observaciones] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[idIngresoInsumo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbInsumos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbInsumos](
	[idInsumo] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NOT NULL,
	[idUnidad] [int] NOT NULL,
	[tipoInsumo] [varchar](50) NULL,
	[descripcion] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[idInsumo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbMedidas]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbMedidas](
	[idMedida] [int] IDENTITY(1,1) NOT NULL,
	[idTipoMedida] [int] NULL,
	[nombre] [nvarchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idMedida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbMesas]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbMesas](
	[codMesa] [nvarchar](20) NOT NULL,
	[idSucursal] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[codMesa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbObservacionesAsistencias]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbObservacionesAsistencias](
	[idObservacionAsistencia] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idObservacionAsistencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPedidos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPedidos](
	[idPedido] [int] IDENTITY(1,1) NOT NULL,
	[codMesa] [nvarchar](20) NULL,
	[fechaPedido] [datetime] NULL,
	[idUsuario] [int] NULL,
	[total] [decimal](6, 2) NULL,
	[idEstadoPedido] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idPedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbPrecios]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPrecios](
	[IdPrecio] [int] IDENTITY(1,1) NOT NULL,
	[IdProducto] [int] NULL,
	[IdMedida] [int] NULL,
	[Precio] [decimal](6, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdPrecio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbProductos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProductos](
	[idProducto] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NOT NULL,
	[idCategoria] [int] NULL,
	[idTipoMedida] [int] NULL,
	[descripcion] [nvarchar](255) NULL,
	[estado] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbProveedores]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbProveedores](
	[idProveedor] [int] IDENTITY(1,1) NOT NULL,
	[nombreEmpresa] [varchar](100) NOT NULL,
	[contacto] [varchar](100) NULL,
	[telefono] [varchar](20) NULL,
	[email] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[idProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbRoles]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbRoles](
	[idRol] [int] IDENTITY(1,1) NOT NULL,
	[nombreRol] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[idRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbSucursales]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbSucursales](
	[idSucursal] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idSucursal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbTiposMedidas]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbTiposMedidas](
	[idTipoMedida] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idTipoMedida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbTiposProductos]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbTiposProductos](
	[idTipoProducto] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[idTipoProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbUnidades]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbUnidades](
	[idUnidad] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](50) NOT NULL,
	[tipo] [varchar](20) NOT NULL,
	[abreviatura] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idUnidad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbUsuarios]    Script Date: 20/05/2025 10:19:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbUsuarios](
	[idUsuario] [int] IDENTITY(1,1) NOT NULL,
	[idRol] [int] NOT NULL,
	[nombre] [nvarchar](100) NULL,
	[apellido] [nvarchar](100) NULL,
	[correoElectronico] [nvarchar](150) NOT NULL,
	[contrasena] [nvarchar](255) NOT NULL,
	[fechaRegistro] [datetime] NULL,
	[activo] [bit] NULL,
	[dni] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[idUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tbAsistencias] ON 

INSERT [dbo].[tbAsistencias] ([idAsistencia], [idUsuario], [idSucursal], [idObservacionAsistencia], [fecha], [horaEntrada], [horaSalida], [minutosTardanza], [idHorarioAplicado], [comentariosAdicionales]) VALUES (1, 3, 1, 5, CAST(N'2025-05-20' AS Date), CAST(N'2025-05-20T05:07:34.887' AS DateTime), CAST(N'2025-05-20T05:08:36.640' AS DateTime), 0, 5, NULL)
INSERT [dbo].[tbAsistencias] ([idAsistencia], [idUsuario], [idSucursal], [idObservacionAsistencia], [fecha], [horaEntrada], [horaSalida], [minutosTardanza], [idHorarioAplicado], [comentariosAdicionales]) VALUES (2, 1, 1, 5, CAST(N'2025-05-21' AS Date), CAST(N'2025-05-21T00:47:20.830' AS DateTime), CAST(N'2025-05-21T00:47:38.290' AS DateTime), 0, 6, NULL)
SET IDENTITY_INSERT [dbo].[tbAsistencias] OFF
GO
SET IDENTITY_INSERT [dbo].[tbCategorias] ON 

INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (1, N'FRAPUCCINNOS', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (2, N'FRAPPÉS', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (3, N'EXTRAS', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (4, N'ESPRESSOS FRÍOS', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (5, N'COLD BREWED', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (6, N'ESPRESSO FIZZ', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (7, N'ESPRESSO MOJITO', 1)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (8, N'SLOW BAR', 2)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (9, N'NUESTRO CAFÉ', 2)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (10, N'TÉ NEGRO', 2)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (11, N'ESPRESSOS', 2)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (12, N'EXPERIENCIA CAFETERA', 2)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (13, N'Mixto americano', 3)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (14, N'Hawaiano', 3)
INSERT [dbo].[tbCategorias] ([idCategoria], [nombre], [idTipoProducto]) VALUES (15, N'Croissant de pollo', 3)
SET IDENTITY_INSERT [dbo].[tbCategorias] OFF
GO
SET IDENTITY_INSERT [dbo].[tbDesechosInsumos] ON 

INSERT [dbo].[tbDesechosInsumos] ([idDesechoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaDesecho], [motivo], [observaciones], [idIngresoInsumo]) VALUES (1, NULL, 3, CAST(2.00 AS Decimal(10, 2)), 3, CAST(N'2025-05-03' AS Date), N'vencimiento', N'Lote caducado', 3)
INSERT [dbo].[tbDesechosInsumos] ([idDesechoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaDesecho], [motivo], [observaciones], [idIngresoInsumo]) VALUES (2, NULL, 1, CAST(0.50 AS Decimal(10, 2)), 2, CAST(N'2025-05-03' AS Date), N'mal estado', N'Se derramó parte del saco', 1)
INSERT [dbo].[tbDesechosInsumos] ([idDesechoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaDesecho], [motivo], [observaciones], [idIngresoInsumo]) VALUES (3, NULL, 5, CAST(0.25 AS Decimal(10, 2)), 5, CAST(N'2025-05-03' AS Date), N'contaminación', N'Sabor alterado', 5)
SET IDENTITY_INSERT [dbo].[tbDesechosInsumos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbDetallePedido] ON 

INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (14, 9, 22, 2, CAST(22.00 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (19, 11, 83, 7, CAST(80.50 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (20, 11, 102, 2, CAST(25.00 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (21, 10, 27, 4, CAST(70.00 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (27, 12, 1, 3, CAST(23.70 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (28, 13, 67, 2, CAST(22.00 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (29, 13, 74, 2, CAST(26.00 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (31, 14, 64, 2, CAST(22.00 AS Decimal(6, 2)))
INSERT [dbo].[tbDetallePedido] ([idDetalle], [idPedido], [idPrecio], [cantidad], [subtotal]) VALUES (33, 15, 22, 1, CAST(11.00 AS Decimal(6, 2)))
SET IDENTITY_INSERT [dbo].[tbDetallePedido] OFF
GO
SET IDENTITY_INSERT [dbo].[tbEstadosPedidos] ON 

INSERT [dbo].[tbEstadosPedidos] ([idEstadoPedido], [estado]) VALUES (1, N'Pendiente')
INSERT [dbo].[tbEstadosPedidos] ([idEstadoPedido], [estado]) VALUES (2, N'En preparación')
INSERT [dbo].[tbEstadosPedidos] ([idEstadoPedido], [estado]) VALUES (3, N'Listo para entregar')
INSERT [dbo].[tbEstadosPedidos] ([idEstadoPedido], [estado]) VALUES (4, N'Entregado')
INSERT [dbo].[tbEstadosPedidos] ([idEstadoPedido], [estado]) VALUES (5, N'Culminado')
SET IDENTITY_INSERT [dbo].[tbEstadosPedidos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbHorarios] ON 

INSERT [dbo].[tbHorarios] ([idHorario], [idUsuario], [diaSemana], [horaEntrada], [horaSalida], [pagoPorHora], [fechaInicioVigencia], [fechaFinVigencia], [activo]) VALUES (1, 4, 0, CAST(N'10:00:00' AS Time), CAST(N'22:00:00' AS Time), CAST(12.00 AS Decimal(10, 2)), CAST(N'2025-05-20' AS Date), CAST(N'2025-05-19' AS Date), 0)
INSERT [dbo].[tbHorarios] ([idHorario], [idUsuario], [diaSemana], [horaEntrada], [horaSalida], [pagoPorHora], [fechaInicioVigencia], [fechaFinVigencia], [activo]) VALUES (2, 4, 0, CAST(N'10:00:00' AS Time), CAST(N'14:00:00' AS Time), CAST(12.00 AS Decimal(10, 2)), CAST(N'2025-05-20' AS Date), NULL, 0)
INSERT [dbo].[tbHorarios] ([idHorario], [idUsuario], [diaSemana], [horaEntrada], [horaSalida], [pagoPorHora], [fechaInicioVigencia], [fechaFinVigencia], [activo]) VALUES (3, 4, 0, CAST(N'10:00:00' AS Time), CAST(N'15:00:00' AS Time), CAST(5.00 AS Decimal(10, 2)), CAST(N'2025-05-20' AS Date), CAST(N'2025-05-21' AS Date), 1)
INSERT [dbo].[tbHorarios] ([idHorario], [idUsuario], [diaSemana], [horaEntrada], [horaSalida], [pagoPorHora], [fechaInicioVigencia], [fechaFinVigencia], [activo]) VALUES (4, 4, 1, CAST(N'10:00:00' AS Time), CAST(N'15:04:00' AS Time), CAST(5.00 AS Decimal(10, 2)), CAST(N'2025-05-20' AS Date), CAST(N'2025-05-22' AS Date), 1)
INSERT [dbo].[tbHorarios] ([idHorario], [idUsuario], [diaSemana], [horaEntrada], [horaSalida], [pagoPorHora], [fechaInicioVigencia], [fechaFinVigencia], [activo]) VALUES (5, 3, 2, CAST(N'05:17:00' AS Time), CAST(N'17:00:00' AS Time), CAST(5.00 AS Decimal(10, 2)), CAST(N'2025-05-20' AS Date), NULL, 1)
INSERT [dbo].[tbHorarios] ([idHorario], [idUsuario], [diaSemana], [horaEntrada], [horaSalida], [pagoPorHora], [fechaInicioVigencia], [fechaFinVigencia], [activo]) VALUES (6, 1, 3, CAST(N'00:51:00' AS Time), CAST(N'20:20:00' AS Time), CAST(5.00 AS Decimal(10, 2)), CAST(N'2025-05-18' AS Date), NULL, 1)
SET IDENTITY_INSERT [dbo].[tbHorarios] OFF
GO
SET IDENTITY_INSERT [dbo].[tbIngresosInsumos] ON 

INSERT [dbo].[tbIngresosInsumos] ([idIngresoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaCompra], [fechaVencimiento], [idProveedor], [lote], [observaciones]) VALUES (1, NULL, 1, CAST(5.00 AS Decimal(10, 2)), 2, CAST(N'2025-05-01' AS Date), NULL, 1, N'CG202505', N'Café tostado en grano')
INSERT [dbo].[tbIngresosInsumos] ([idIngresoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaCompra], [fechaVencimiento], [idProveedor], [lote], [observaciones]) VALUES (2, NULL, 2, CAST(200.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-01' AS Date), NULL, 3, N'VD202505', N'Caja con 200 vasos')
INSERT [dbo].[tbIngresosInsumos] ([idIngresoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaCompra], [fechaVencimiento], [idProveedor], [lote], [observaciones]) VALUES (3, NULL, 3, CAST(10.00 AS Decimal(10, 2)), 3, CAST(N'2025-05-01' AS Date), CAST(N'2025-06-01' AS Date), 2, N'LT202505', N'Refrigerado, mantener frío')
INSERT [dbo].[tbIngresosInsumos] ([idIngresoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaCompra], [fechaVencimiento], [idProveedor], [lote], [observaciones]) VALUES (4, NULL, 4, CAST(20.00 AS Decimal(10, 2)), 2, CAST(N'2025-05-01' AS Date), NULL, 1, N'AZ202505', N'Saco de 20kg')
INSERT [dbo].[tbIngresosInsumos] ([idIngresoInsumo], [idSucursal], [idInsumo], [cantidad], [idUnidad], [fechaCompra], [fechaVencimiento], [idProveedor], [lote], [observaciones]) VALUES (5, NULL, 5, CAST(2.50 AS Decimal(10, 2)), 5, CAST(N'2025-05-01' AS Date), CAST(N'2025-07-15' AS Date), 1, N'JV202505', N'Botellas de 500ml')
SET IDENTITY_INSERT [dbo].[tbIngresosInsumos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbInsumos] ON 

INSERT [dbo].[tbInsumos] ([idInsumo], [nombre], [idUnidad], [tipoInsumo], [descripcion]) VALUES (1, N'Café en grano', 2, N'materia prima', N'Café Arábica Premium')
INSERT [dbo].[tbInsumos] ([idInsumo], [nombre], [idUnidad], [tipoInsumo], [descripcion]) VALUES (2, N'Vaso descartable 8oz', 1, N'empaque', N'Vaso térmico desechable')
INSERT [dbo].[tbInsumos] ([idInsumo], [nombre], [idUnidad], [tipoInsumo], [descripcion]) VALUES (3, N'Leche entera', 3, N'perecedero', N'Leche pasteurizada de vaca')
INSERT [dbo].[tbInsumos] ([idInsumo], [nombre], [idUnidad], [tipoInsumo], [descripcion]) VALUES (4, N'Azúcar blanca', 2, N'materia prima', N'Azúcar refinada')
INSERT [dbo].[tbInsumos] ([idInsumo], [nombre], [idUnidad], [tipoInsumo], [descripcion]) VALUES (5, N'Jarabe de vainilla', 5, N'materia prima', N'Saborizante líquido')
SET IDENTITY_INSERT [dbo].[tbInsumos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbMedidas] ON 

INSERT [dbo].[tbMedidas] ([idMedida], [idTipoMedida], [nombre]) VALUES (1, 1, N'Reg')
INSERT [dbo].[tbMedidas] ([idMedida], [idTipoMedida], [nombre]) VALUES (2, 1, N'Med')
INSERT [dbo].[tbMedidas] ([idMedida], [idTipoMedida], [nombre]) VALUES (3, 1, N'Gr')
INSERT [dbo].[tbMedidas] ([idMedida], [idTipoMedida], [nombre]) VALUES (4, 2, N'Simple')
INSERT [dbo].[tbMedidas] ([idMedida], [idTipoMedida], [nombre]) VALUES (5, 2, N'Doble')
INSERT [dbo].[tbMedidas] ([idMedida], [idTipoMedida], [nombre]) VALUES (6, 3, N'Precio Unico')
SET IDENTITY_INSERT [dbo].[tbMedidas] OFF
GO
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa1-B1', 1)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa1-B2', 2)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa2-B1', 1)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa2-B2', 2)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa3-B1', 1)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa3-B3', 2)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa4-B1', 1)
INSERT [dbo].[tbMesas] ([codMesa], [idSucursal]) VALUES (N'Mesa4-B4', 2)
GO
SET IDENTITY_INSERT [dbo].[tbObservacionesAsistencias] ON 

INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (1, N'Asistencia Puntual')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (8, N'Ausencia Injustificada')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (7, N'Ausencia Justificada')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (10, N'Día No Laboral Programado')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (9, N'Entrada Registrada')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (4, N'Salida Puntual')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (6, N'Salida Tardía')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (5, N'Salida Temprana')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (2, N'Tardanza Leve')
INSERT [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia], [descripcion]) VALUES (3, N'Tardanza Significativa')
SET IDENTITY_INSERT [dbo].[tbObservacionesAsistencias] OFF
GO
SET IDENTITY_INSERT [dbo].[tbPedidos] ON 

INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (9, N'Mesa1-B1', CAST(N'2025-05-20T01:01:16.780' AS DateTime), 1, CAST(22.00 AS Decimal(6, 2)), 5)
INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (10, N'Mesa4-B1', CAST(N'2025-05-20T01:20:02.473' AS DateTime), 1, CAST(70.00 AS Decimal(6, 2)), 5)
INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (11, N'Mesa2-B1', CAST(N'2025-05-20T12:07:04.197' AS DateTime), 1, CAST(105.50 AS Decimal(6, 2)), 5)
INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (12, N'Mesa4-B1', CAST(N'2025-05-20T12:10:57.147' AS DateTime), 1, CAST(23.70 AS Decimal(6, 2)), 5)
INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (13, N'Mesa2-B1', CAST(N'2025-05-20T07:38:41.093' AS DateTime), 1, CAST(48.00 AS Decimal(6, 2)), 1)
INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (14, N'Mesa4-B1', CAST(N'2025-05-20T07:40:28.740' AS DateTime), 1, CAST(22.00 AS Decimal(6, 2)), 5)
INSERT [dbo].[tbPedidos] ([idPedido], [codMesa], [fechaPedido], [idUsuario], [total], [idEstadoPedido]) VALUES (15, N'Mesa1-B1', CAST(N'2025-05-20T07:42:34.927' AS DateTime), 3, CAST(11.00 AS Decimal(6, 2)), 3)
SET IDENTITY_INSERT [dbo].[tbPedidos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbPrecios] ON 

INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (1, 1, 1, CAST(7.90 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (2, 1, 2, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (3, 1, 3, CAST(15.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (4, 3, 1, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (5, 3, 2, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (6, 3, 3, CAST(15.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (7, 4, 1, CAST(8.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (8, 4, 2, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (9, 4, 3, CAST(15.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (10, 5, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (11, 5, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (12, 5, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (13, 2, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (14, 2, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (15, 2, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (16, 6, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (17, 6, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (18, 6, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (19, 7, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (20, 7, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (21, 7, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (22, 8, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (23, 8, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (24, 8, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (25, 9, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (26, 9, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (27, 9, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (28, 10, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (29, 10, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (30, 10, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (31, 11, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (32, 11, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (33, 11, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (34, 12, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (35, 12, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (36, 12, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (37, 13, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (38, 13, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (39, 13, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (40, 14, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (41, 14, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (42, 14, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (43, 15, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (44, 15, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (45, 15, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (46, 16, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (47, 16, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (48, 16, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (49, 17, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (50, 17, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (51, 17, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (52, 18, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (53, 18, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (54, 18, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (55, 19, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (56, 19, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (57, 19, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (58, 20, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (59, 20, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (60, 20, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (61, 21, 1, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (62, 21, 2, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (63, 21, 3, CAST(15.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (64, 22, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (65, 22, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (66, 22, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (67, 23, 1, CAST(11.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (68, 23, 2, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (69, 23, 3, CAST(17.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (70, 24, 1, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (71, 24, 2, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (72, 24, 3, CAST(17.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (73, 25, 2, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (74, 26, 6, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (75, 27, 6, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (76, 28, 4, CAST(6.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (77, 28, 5, CAST(8.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (78, 29, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (79, 29, 5, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (80, 30, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (81, 30, 5, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (82, 31, 4, CAST(10.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (83, 32, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (84, 33, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (85, 33, 5, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (86, 34, 4, CAST(8.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (87, 34, 5, CAST(10.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (88, 35, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (89, 35, 5, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (90, 36, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (91, 36, 5, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (92, 37, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (93, 38, 5, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (94, 39, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (95, 39, 5, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (96, 40, 4, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (97, 40, 5, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (98, 41, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (99, 41, 5, CAST(14.50 AS Decimal(6, 2)))
GO
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (100, 42, 4, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (101, 42, 5, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (102, 43, 5, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (103, 44, 5, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (104, 45, 6, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (105, 46, 6, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (106, 47, 6, CAST(15.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (107, 48, 6, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (108, 49, 6, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (109, 50, 6, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (110, 51, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (111, 51, 5, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (112, 52, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (113, 52, 5, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (114, 53, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (115, 53, 5, CAST(14.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (116, 54, 4, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (117, 54, 5, CAST(15.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (118, 55, 6, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (119, 56, 6, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (120, 57, 6, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (121, 58, 6, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (122, 59, 6, CAST(15.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (123, 60, 6, CAST(30.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (124, 61, 6, CAST(56.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (125, 62, 6, CAST(9.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (126, 63, 6, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (127, 64, 6, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (128, 65, 6, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (129, 66, 6, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (130, 67, 4, CAST(6.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (131, 67, 5, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (132, 68, 4, CAST(6.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (133, 68, 5, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (134, 69, 4, CAST(6.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (135, 69, 5, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (136, 70, 4, CAST(7.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (137, 71, 4, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (138, 72, 4, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (139, 73, 4, CAST(8.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (140, 74, 4, CAST(6.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (141, 74, 5, CAST(8.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (142, 75, 4, CAST(8.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (143, 75, 5, CAST(10.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (144, 76, 4, CAST(10.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (145, 77, 4, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (146, 78, 4, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (147, 79, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (148, 80, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (149, 81, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (150, 82, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (151, 83, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (152, 84, 4, CAST(12.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (153, 85, 5, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (154, 86, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (155, 87, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (156, 88, 4, CAST(13.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (157, 89, 4, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (158, 90, 4, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (159, 91, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (160, 92, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (161, 93, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (162, 94, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (163, 95, 4, CAST(13.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (164, 96, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (165, 97, 4, CAST(12.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (166, 98, 4, CAST(11.50 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (167, 98, 5, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (168, 99, 4, CAST(14.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (169, 100, 6, CAST(19.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (170, 101, 6, CAST(19.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (171, 102, 6, CAST(19.00 AS Decimal(6, 2)))
INSERT [dbo].[tbPrecios] ([IdPrecio], [IdProducto], [IdMedida], [Precio]) VALUES (172, 103, 6, CAST(14.50 AS Decimal(6, 2)))
SET IDENTITY_INSERT [dbo].[tbPrecios] OFF
GO
SET IDENTITY_INSERT [dbo].[tbProductos] ON 

INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (1, N'Espresso', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (2, N'Chocobeer', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (3, N'Oreo', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (4, N'Chocolate', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (5, N'Chocomint', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (6, N'Dark Chocochip', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (7, N'Sublime', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (8, N'Caramel', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (9, N'Caramel Coconut', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (10, N'Vainilla', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (11, N'Menta', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (12, N'Mocca', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (13, N'Mocca Cookies', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (14, N'Mocca Caramel', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (15, N'Whisky', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (16, N'Baileys', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (17, N'Canela', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (18, N'Frutos silvestres', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (19, N'Selva Negra', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (20, N'Chai', 1, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (21, N'Fresa', 2, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (22, N'Maracuya', 2, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (23, N'Mango', 2, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (24, N'Pie de limón', 2, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (25, N'Matcha', 2, 1, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (26, N'BERRY AESTHETIC', 3, 3, N'Refrescante de manzana y berries', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (27, N'AFFOGATO', 3, 3, N'Espresso +1 bola de helado de vainilla', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (28, N'Americano fresh', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (29, N'Americano orange', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (30, N'Americano bombón', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (31, N'Cappuccino fresh', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (32, N'Latte fresh', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (33, N'Flat White', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (34, N'Iced Coffe Clásico', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (35, N'Iced Baileys', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (36, N'Iced Caramel', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (37, N'Iced Menta', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (38, N'Iced Canela', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (39, N'Iced Vainilla', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (40, N'Iced Mocca', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (41, N'Iced Mocca Caramel', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (42, N'Iced Chaí', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (43, N'Iced Matcha', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (44, N'Tropical coffe', 4, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (45, N'Botella de 250 ml', 5, 3, N'Café infusionado en frio por 20 horas', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (46, N'Brew Clásico', 5, 3, N'Café infusionado en frio por 20 horas', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (47, N'Nitro Brew', 5, 3, N'Café infusionado en frio por 20 horas', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (48, N'Citric Brew', 5, 3, N'Café infusionado en frio por 20 horas', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (49, N'Vainilla Brew', 5, 3, N'Café infusionado en frio por 20 horas', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (50, N'Orange Brew', 5, 3, N'Café infusionado en frio por 20 horas', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (51, N'ESPRESSO FIZZ MARACUYA', 6, 2, N'Agua gasifica, hielo, syrop a escoger y espresso.', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (52, N'ESPRESSO FIZZ MENTA', 6, 2, N'Agua gasifica, hielo, syrop a escoger y espresso.', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (53, N'ESPRESSO FIZZ FRUTOS SILVESTRES', 6, 2, N'Agua gasifica, hielo, syrop a escoger y espresso.', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (54, N'ESPRESSO MOJITO CLASICO', 7, 2, N'(No contiene Alcohol)', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (55, N'PRENSA FRANCESA', 8, 3, N'Extraccion en reposo, tiempo aprox. 4 minutos', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (56, N'v 60', 8, 3, N'Extracción por goteo, tiempo aprox. 4 minutos (helado o caliente)', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (57, N'CHEMEX', 8, 3, N'Extracción suave por goteo, tiempo aprox. 5 minutos.', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (58, N'INFUSIÓN DE CEREZO DE CAFÉ', 8, 3, N'Tiempo aprox. 5 minutos (helado o caliente)', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (59, N'Bolsa de 100g', 9, 3, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (60, N'Bolsa de 250g', 9, 3, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (61, N'Bolsa de 500g', 9, 3, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (62, N'Té negro clásico', 10, 3, N'Base de té negro y syrops', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (63, N'Maracuyá', 10, 3, N'Base de té negro y syrops', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (64, N'Menta', 10, 3, N'Base de té negro y syrops', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (65, N'Frutos silvestres', 10, 3, N'Base de té negro y syrops', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (66, N'Manzana', 10, 3, N'Base de té negro y syrops', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (67, N'Espresso', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (68, N'Ristretto', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (69, N'Lungo', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (70, N'Macchiato', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (71, N'Macchiato Caramel', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (72, N'Macchiato Mocca', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (73, N'Espresso bombón', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (74, N'Americano', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (75, N'Cortado', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (76, N'Cappuccino', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (77, N'Moccaccino', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (78, N'Dark Moccaccino', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (79, N'Moccaccino Caramel', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (80, N'Cappuccino Caramel', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (81, N'Cappuccino Vainilla', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (82, N'Cappuccino Baileys', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (83, N'Cappuccino Canela', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (84, N'Cappuccino Chaí', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (85, N'Flat White', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (86, N'Latte', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (87, N'Latte Macchiato', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (88, N'Latte Mocca', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (89, N'Dark latte Mocca', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (90, N'Latte Mocca Caramel', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (91, N'Latte Caramel', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (92, N'Latte Vainilla', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (93, N'Latte Canela', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (94, N'Latte Baileys', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (95, N'Latte Chaí', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (96, N'Latte Coco Berry', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (97, N'Latte Matcha', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (98, N'Americano bombón', 11, 2, NULL, N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (99, N'Irish Coffee', 11, 2, NULL, N'activo')
GO
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (100, N'Experiencia espresso', 12, 3, N'Consultar disponibilidad del barista', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (101, N'Experiencia filtrado V60', 12, 3, N'Consultar disponibilidad del barista', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (102, N'Experiencia cold Brew', 12, 3, N'Consultar disponibilidad del barista', N'activo')
INSERT [dbo].[tbProductos] ([idProducto], [nombre], [idCategoria], [idTipoMedida], [descripcion], [estado]) VALUES (103, N'Chocolate Caliente Especial', 8, 3, N'Solo disponible en invierno', N'activo')
SET IDENTITY_INSERT [dbo].[tbProductos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbProveedores] ON 

INSERT [dbo].[tbProveedores] ([idProveedor], [nombreEmpresa], [contacto], [telefono], [email]) VALUES (1, N'Café Perú S.A.', N'Carlos Méndez', N'987654321', N'ventas@cafeperu.com')
INSERT [dbo].[tbProveedores] ([idProveedor], [nombreEmpresa], [contacto], [telefono], [email]) VALUES (2, N'Lácteos del Sur', N'Juan Torres', N'912345678', N'ventas@lacteosur.com')
INSERT [dbo].[tbProveedores] ([idProveedor], [nombreEmpresa], [contacto], [telefono], [email]) VALUES (3, N'Empaques Express', N'Lucía Díaz', N'934567890', N'contacto@empaquesexpress.com')
SET IDENTITY_INSERT [dbo].[tbProveedores] OFF
GO
SET IDENTITY_INSERT [dbo].[tbRoles] ON 

INSERT [dbo].[tbRoles] ([idRol], [nombreRol]) VALUES (1, N'Admin')
INSERT [dbo].[tbRoles] ([idRol], [nombreRol]) VALUES (3, N'Empleado')
INSERT [dbo].[tbRoles] ([idRol], [nombreRol]) VALUES (2, N'Encargado')
SET IDENTITY_INSERT [dbo].[tbRoles] OFF
GO
SET IDENTITY_INSERT [dbo].[tbSucursales] ON 

INSERT [dbo].[tbSucursales] ([idSucursal], [nombre]) VALUES (1, N'Buenisimo 1')
INSERT [dbo].[tbSucursales] ([idSucursal], [nombre]) VALUES (2, N'Buenisimo 2')
SET IDENTITY_INSERT [dbo].[tbSucursales] OFF
GO
SET IDENTITY_INSERT [dbo].[tbTiposMedidas] ON 

INSERT [dbo].[tbTiposMedidas] ([idTipoMedida], [nombre]) VALUES (1, N'Tamaño (Reg/Med/Gr)')
INSERT [dbo].[tbTiposMedidas] ([idTipoMedida], [nombre]) VALUES (2, N'Tipo (Simple/Doble)')
INSERT [dbo].[tbTiposMedidas] ([idTipoMedida], [nombre]) VALUES (3, N'Precio Unico')
SET IDENTITY_INSERT [dbo].[tbTiposMedidas] OFF
GO
SET IDENTITY_INSERT [dbo].[tbTiposProductos] ON 

INSERT [dbo].[tbTiposProductos] ([idTipoProducto], [nombre]) VALUES (1, N'BEBIDAS HELADAS')
INSERT [dbo].[tbTiposProductos] ([idTipoProducto], [nombre]) VALUES (2, N'BEBIDAS CALIENTES')
INSERT [dbo].[tbTiposProductos] ([idTipoProducto], [nombre]) VALUES (3, N'COMESTIBLES')
SET IDENTITY_INSERT [dbo].[tbTiposProductos] OFF
GO
SET IDENTITY_INSERT [dbo].[tbUnidades] ON 

INSERT [dbo].[tbUnidades] ([idUnidad], [nombre], [tipo], [abreviatura]) VALUES (1, N'Unidad', N'cantidad', N'un')
INSERT [dbo].[tbUnidades] ([idUnidad], [nombre], [tipo], [abreviatura]) VALUES (2, N'Kilogramo', N'peso', N'kg')
INSERT [dbo].[tbUnidades] ([idUnidad], [nombre], [tipo], [abreviatura]) VALUES (3, N'Litro', N'volumen', N'L')
INSERT [dbo].[tbUnidades] ([idUnidad], [nombre], [tipo], [abreviatura]) VALUES (4, N'Gramo', N'peso', N'g')
INSERT [dbo].[tbUnidades] ([idUnidad], [nombre], [tipo], [abreviatura]) VALUES (5, N'Mililitro', N'volumen', N'ml')
SET IDENTITY_INSERT [dbo].[tbUnidades] OFF
GO
SET IDENTITY_INSERT [dbo].[tbUsuarios] ON 

INSERT [dbo].[tbUsuarios] ([idUsuario], [idRol], [nombre], [apellido], [correoElectronico], [contrasena], [fechaRegistro], [activo], [dni]) VALUES (1, 1, N'Juan', N'Pérez', N'juan.perez@example.com', N'slujKAcINWWCn1PP/emhxdMETDIGK5VA2z4ij66X8vaIF3voKlDazQhB9ikNP0Z3', CAST(N'2025-05-07T12:01:45.780' AS DateTime), 1, N'111')
INSERT [dbo].[tbUsuarios] ([idUsuario], [idRol], [nombre], [apellido], [correoElectronico], [contrasena], [fechaRegistro], [activo], [dni]) VALUES (2, 2, N'María', N'López', N'maria.lopez@example.com', N'MAzG6oyBkDHtLuTmy5cosleU4ojxxpGq5lGuga6PUgdO45CYB6toIC9rfnKwYNsW', CAST(N'2025-05-07T12:01:45.780' AS DateTime), 1, N'222')
INSERT [dbo].[tbUsuarios] ([idUsuario], [idRol], [nombre], [apellido], [correoElectronico], [contrasena], [fechaRegistro], [activo], [dni]) VALUES (3, 3, N'Carlos', N'Gómez', N'carlos.gomez@example.com', N'jVNgFhClWrPvw5R/ccyQYHgVcChfaQO2MZfJ+9HDPoLD0AEVwfS+Q5fKIEbvCxDy', CAST(N'2025-05-07T12:01:45.780' AS DateTime), 1, N'333')
INSERT [dbo].[tbUsuarios] ([idUsuario], [idRol], [nombre], [apellido], [correoElectronico], [contrasena], [fechaRegistro], [activo], [dni]) VALUES (4, 3, N'Ana', N'Martínezz', N'ana.martinez@example.com', N'111', CAST(N'2025-05-20T06:56:05.550' AS DateTime), 1, N'444')
INSERT [dbo].[tbUsuarios] ([idUsuario], [idRol], [nombre], [apellido], [correoElectronico], [contrasena], [fechaRegistro], [activo], [dni]) VALUES (5, 3, N'Estefano', N'Condori', N'estefano.condori@gmail.com', N'jy91T7xHyKmwFVO8Gs1NHgJt775mm0O7498NdqxHiYh7Lua6SUqHzyx9xQ0Y+T9p', CAST(N'2025-05-20T04:21:24.260' AS DateTime), 1, N'555')
SET IDENTITY_INSERT [dbo].[tbUsuarios] OFF
GO
/****** Object:  Index [IX_tbAsistencias_Usuario_Fecha]    Script Date: 20/05/2025 10:19:45 p. m. ******/
CREATE NONCLUSTERED INDEX [IX_tbAsistencias_Usuario_Fecha] ON [dbo].[tbAsistencias]
(
	[idUsuario] ASC,
	[fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_tbHorarios_idUsuario]    Script Date: 20/05/2025 10:19:45 p. m. ******/
CREATE NONCLUSTERED INDEX [IX_tbHorarios_idUsuario] ON [dbo].[tbHorarios]
(
	[idUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_tbHorarios_Usuario_Activo_Vigencia]    Script Date: 20/05/2025 10:19:45 p. m. ******/
CREATE NONCLUSTERED INDEX [IX_tbHorarios_Usuario_Activo_Vigencia] ON [dbo].[tbHorarios]
(
	[idUsuario] ASC,
	[activo] ASC,
	[fechaInicioVigencia] ASC,
	[fechaFinVigencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_tbInsumos_nombre]    Script Date: 20/05/2025 10:19:45 p. m. ******/
ALTER TABLE [dbo].[tbInsumos] ADD  CONSTRAINT [UQ_tbInsumos_nombre] UNIQUE NONCLUSTERED 
(
	[nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_tbObservacionesAsistencias_descripcion]    Script Date: 20/05/2025 10:19:45 p. m. ******/
ALTER TABLE [dbo].[tbObservacionesAsistencias] ADD  CONSTRAINT [UQ_tbObservacionesAsistencias_descripcion] UNIQUE NONCLUSTERED 
(
	[descripcion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_tbRoles_nombreRol]    Script Date: 20/05/2025 10:19:45 p. m. ******/
ALTER TABLE [dbo].[tbRoles] ADD  CONSTRAINT [UQ_tbRoles_nombreRol] UNIQUE NONCLUSTERED 
(
	[nombreRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_tbUnidades_nombre]    Script Date: 20/05/2025 10:19:45 p. m. ******/
ALTER TABLE [dbo].[tbUnidades] ADD  CONSTRAINT [UQ_tbUnidades_nombre] UNIQUE NONCLUSTERED 
(
	[nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_tbUsuarios_correoElectronico]    Script Date: 20/05/2025 10:19:45 p. m. ******/
ALTER TABLE [dbo].[tbUsuarios] ADD  CONSTRAINT [UQ_tbUsuarios_correoElectronico] UNIQUE NONCLUSTERED 
(
	[correoElectronico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_tbUsuarios_dni]    Script Date: 20/05/2025 10:19:45 p. m. ******/
ALTER TABLE [dbo].[tbUsuarios] ADD  CONSTRAINT [UQ_tbUsuarios_dni] UNIQUE NONCLUSTERED 
(
	[dni] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbAsistencias] ADD  DEFAULT ((0)) FOR [minutosTardanza]
GO
ALTER TABLE [dbo].[tbHorarios] ADD  DEFAULT (getdate()) FOR [fechaInicioVigencia]
GO
ALTER TABLE [dbo].[tbHorarios] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[tbProductos] ADD  DEFAULT ('activo') FOR [estado]
GO
ALTER TABLE [dbo].[tbUsuarios] ADD  DEFAULT (getdate()) FOR [fechaRegistro]
GO
ALTER TABLE [dbo].[tbUsuarios] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[tbAsistencias]  WITH CHECK ADD  CONSTRAINT [FK_tbAsistencias_tbHorarios_idHorarioAplicado] FOREIGN KEY([idHorarioAplicado])
REFERENCES [dbo].[tbHorarios] ([idHorario])
GO
ALTER TABLE [dbo].[tbAsistencias] CHECK CONSTRAINT [FK_tbAsistencias_tbHorarios_idHorarioAplicado]
GO
ALTER TABLE [dbo].[tbAsistencias]  WITH CHECK ADD  CONSTRAINT [FK_tbAsistencias_tbObservacionesAsistencias_idObservacionAsistencia] FOREIGN KEY([idObservacionAsistencia])
REFERENCES [dbo].[tbObservacionesAsistencias] ([idObservacionAsistencia])
GO
ALTER TABLE [dbo].[tbAsistencias] CHECK CONSTRAINT [FK_tbAsistencias_tbObservacionesAsistencias_idObservacionAsistencia]
GO
ALTER TABLE [dbo].[tbAsistencias]  WITH CHECK ADD  CONSTRAINT [FK_tbAsistencias_tbSucursales_idSucursal] FOREIGN KEY([idSucursal])
REFERENCES [dbo].[tbSucursales] ([idSucursal])
GO
ALTER TABLE [dbo].[tbAsistencias] CHECK CONSTRAINT [FK_tbAsistencias_tbSucursales_idSucursal]
GO
ALTER TABLE [dbo].[tbAsistencias]  WITH CHECK ADD  CONSTRAINT [FK_tbAsistencias_tbUsuarios_idUsuario] FOREIGN KEY([idUsuario])
REFERENCES [dbo].[tbUsuarios] ([idUsuario])
GO
ALTER TABLE [dbo].[tbAsistencias] CHECK CONSTRAINT [FK_tbAsistencias_tbUsuarios_idUsuario]
GO
ALTER TABLE [dbo].[tbCategorias]  WITH CHECK ADD  CONSTRAINT [FK_tbCategorias_tbTiposProductos_idTipoProducto] FOREIGN KEY([idTipoProducto])
REFERENCES [dbo].[tbTiposProductos] ([idTipoProducto])
GO
ALTER TABLE [dbo].[tbCategorias] CHECK CONSTRAINT [FK_tbCategorias_tbTiposProductos_idTipoProducto]
GO
ALTER TABLE [dbo].[tbDesechosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbDesechosInsumos_tbIngresosInsumos_idIngresoInsumo] FOREIGN KEY([idIngresoInsumo])
REFERENCES [dbo].[tbIngresosInsumos] ([idIngresoInsumo])
GO
ALTER TABLE [dbo].[tbDesechosInsumos] CHECK CONSTRAINT [FK_tbDesechosInsumos_tbIngresosInsumos_idIngresoInsumo]
GO
ALTER TABLE [dbo].[tbDesechosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbDesechosInsumos_tbInsumos_idInsumo] FOREIGN KEY([idInsumo])
REFERENCES [dbo].[tbInsumos] ([idInsumo])
GO
ALTER TABLE [dbo].[tbDesechosInsumos] CHECK CONSTRAINT [FK_tbDesechosInsumos_tbInsumos_idInsumo]
GO
ALTER TABLE [dbo].[tbDesechosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbDesechosInsumos_tbSucursales_idSucursal] FOREIGN KEY([idSucursal])
REFERENCES [dbo].[tbSucursales] ([idSucursal])
GO
ALTER TABLE [dbo].[tbDesechosInsumos] CHECK CONSTRAINT [FK_tbDesechosInsumos_tbSucursales_idSucursal]
GO
ALTER TABLE [dbo].[tbDesechosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbDesechosInsumos_tbUnidades_idUnidad] FOREIGN KEY([idUnidad])
REFERENCES [dbo].[tbUnidades] ([idUnidad])
GO
ALTER TABLE [dbo].[tbDesechosInsumos] CHECK CONSTRAINT [FK_tbDesechosInsumos_tbUnidades_idUnidad]
GO
ALTER TABLE [dbo].[tbDetallePedido]  WITH CHECK ADD  CONSTRAINT [FK_tbDetallePedido_tbPedidos_idPedido] FOREIGN KEY([idPedido])
REFERENCES [dbo].[tbPedidos] ([idPedido])
GO
ALTER TABLE [dbo].[tbDetallePedido] CHECK CONSTRAINT [FK_tbDetallePedido_tbPedidos_idPedido]
GO
ALTER TABLE [dbo].[tbDetallePedido]  WITH CHECK ADD  CONSTRAINT [FK_tbDetallePedido_tbPrecios_idPrecio] FOREIGN KEY([idPrecio])
REFERENCES [dbo].[tbPrecios] ([IdPrecio])
GO
ALTER TABLE [dbo].[tbDetallePedido] CHECK CONSTRAINT [FK_tbDetallePedido_tbPrecios_idPrecio]
GO
ALTER TABLE [dbo].[tbHorarios]  WITH CHECK ADD  CONSTRAINT [FK_tbHorarios_tbUsuarios_idUsuario] FOREIGN KEY([idUsuario])
REFERENCES [dbo].[tbUsuarios] ([idUsuario])
GO
ALTER TABLE [dbo].[tbHorarios] CHECK CONSTRAINT [FK_tbHorarios_tbUsuarios_idUsuario]
GO
ALTER TABLE [dbo].[tbIngresosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbIngresosInsumos_tbInsumos_idInsumo] FOREIGN KEY([idInsumo])
REFERENCES [dbo].[tbInsumos] ([idInsumo])
GO
ALTER TABLE [dbo].[tbIngresosInsumos] CHECK CONSTRAINT [FK_tbIngresosInsumos_tbInsumos_idInsumo]
GO
ALTER TABLE [dbo].[tbIngresosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbIngresosInsumos_tbProveedores_idProveedor] FOREIGN KEY([idProveedor])
REFERENCES [dbo].[tbProveedores] ([idProveedor])
GO
ALTER TABLE [dbo].[tbIngresosInsumos] CHECK CONSTRAINT [FK_tbIngresosInsumos_tbProveedores_idProveedor]
GO
ALTER TABLE [dbo].[tbIngresosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbIngresosInsumos_tbSucursales_idSucursal] FOREIGN KEY([idSucursal])
REFERENCES [dbo].[tbSucursales] ([idSucursal])
GO
ALTER TABLE [dbo].[tbIngresosInsumos] CHECK CONSTRAINT [FK_tbIngresosInsumos_tbSucursales_idSucursal]
GO
ALTER TABLE [dbo].[tbIngresosInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbIngresosInsumos_tbUnidades_idUnidad] FOREIGN KEY([idUnidad])
REFERENCES [dbo].[tbUnidades] ([idUnidad])
GO
ALTER TABLE [dbo].[tbIngresosInsumos] CHECK CONSTRAINT [FK_tbIngresosInsumos_tbUnidades_idUnidad]
GO
ALTER TABLE [dbo].[tbInsumos]  WITH CHECK ADD  CONSTRAINT [FK_tbInsumos_tbUnidades_idUnidad] FOREIGN KEY([idUnidad])
REFERENCES [dbo].[tbUnidades] ([idUnidad])
GO
ALTER TABLE [dbo].[tbInsumos] CHECK CONSTRAINT [FK_tbInsumos_tbUnidades_idUnidad]
GO
ALTER TABLE [dbo].[tbMedidas]  WITH CHECK ADD  CONSTRAINT [FK_tbMedidas_tbTiposMedidas_idTipoMedida] FOREIGN KEY([idTipoMedida])
REFERENCES [dbo].[tbTiposMedidas] ([idTipoMedida])
GO
ALTER TABLE [dbo].[tbMedidas] CHECK CONSTRAINT [FK_tbMedidas_tbTiposMedidas_idTipoMedida]
GO
ALTER TABLE [dbo].[tbMesas]  WITH CHECK ADD  CONSTRAINT [FK_tbMesas_tbSucursales_idSucursal] FOREIGN KEY([idSucursal])
REFERENCES [dbo].[tbSucursales] ([idSucursal])
GO
ALTER TABLE [dbo].[tbMesas] CHECK CONSTRAINT [FK_tbMesas_tbSucursales_idSucursal]
GO
ALTER TABLE [dbo].[tbPedidos]  WITH CHECK ADD  CONSTRAINT [FK_tbPedidos_tbEstadosPedidos_idEstadoPedido] FOREIGN KEY([idEstadoPedido])
REFERENCES [dbo].[tbEstadosPedidos] ([idEstadoPedido])
GO
ALTER TABLE [dbo].[tbPedidos] CHECK CONSTRAINT [FK_tbPedidos_tbEstadosPedidos_idEstadoPedido]
GO
ALTER TABLE [dbo].[tbPedidos]  WITH CHECK ADD  CONSTRAINT [FK_tbPedidos_tbMesas_codMesa] FOREIGN KEY([codMesa])
REFERENCES [dbo].[tbMesas] ([codMesa])
GO
ALTER TABLE [dbo].[tbPedidos] CHECK CONSTRAINT [FK_tbPedidos_tbMesas_codMesa]
GO
ALTER TABLE [dbo].[tbPedidos]  WITH CHECK ADD  CONSTRAINT [FK_tbPedidos_tbUsuarios_idUsuario] FOREIGN KEY([idUsuario])
REFERENCES [dbo].[tbUsuarios] ([idUsuario])
GO
ALTER TABLE [dbo].[tbPedidos] CHECK CONSTRAINT [FK_tbPedidos_tbUsuarios_idUsuario]
GO
ALTER TABLE [dbo].[tbPrecios]  WITH CHECK ADD  CONSTRAINT [FK_tbPrecios_tbMedidas_IdMedida] FOREIGN KEY([IdMedida])
REFERENCES [dbo].[tbMedidas] ([idMedida])
GO
ALTER TABLE [dbo].[tbPrecios] CHECK CONSTRAINT [FK_tbPrecios_tbMedidas_IdMedida]
GO
ALTER TABLE [dbo].[tbPrecios]  WITH CHECK ADD  CONSTRAINT [FK_tbPrecios_tbProductos_IdProducto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[tbProductos] ([idProducto])
GO
ALTER TABLE [dbo].[tbPrecios] CHECK CONSTRAINT [FK_tbPrecios_tbProductos_IdProducto]
GO
ALTER TABLE [dbo].[tbProductos]  WITH CHECK ADD  CONSTRAINT [FK_tbProductos_tbCategorias_idCategoria] FOREIGN KEY([idCategoria])
REFERENCES [dbo].[tbCategorias] ([idCategoria])
GO
ALTER TABLE [dbo].[tbProductos] CHECK CONSTRAINT [FK_tbProductos_tbCategorias_idCategoria]
GO
ALTER TABLE [dbo].[tbProductos]  WITH CHECK ADD  CONSTRAINT [FK_tbProductos_tbTiposMedidas_idTipoMedida] FOREIGN KEY([idTipoMedida])
REFERENCES [dbo].[tbTiposMedidas] ([idTipoMedida])
GO
ALTER TABLE [dbo].[tbProductos] CHECK CONSTRAINT [FK_tbProductos_tbTiposMedidas_idTipoMedida]
GO
ALTER TABLE [dbo].[tbUsuarios]  WITH CHECK ADD  CONSTRAINT [FK_tbUsuarios_tbRoles_idRol] FOREIGN KEY([idRol])
REFERENCES [dbo].[tbRoles] ([idRol])
GO
ALTER TABLE [dbo].[tbUsuarios] CHECK CONSTRAINT [FK_tbUsuarios_tbRoles_idRol]
GO
ALTER TABLE [dbo].[tbHorarios]  WITH CHECK ADD  CONSTRAINT [CK_tbHorarios_diaSemana] CHECK  (([diaSemana]>=(0) AND [diaSemana]<=(6)))
GO
ALTER TABLE [dbo].[tbHorarios] CHECK CONSTRAINT [CK_tbHorarios_diaSemana]
GO
ALTER TABLE [dbo].[tbHorarios]  WITH CHECK ADD  CONSTRAINT [CK_tbHorarios_horaSalida_mayor_horaEntrada] CHECK  (([horaSalida]>[horaEntrada]))
GO
ALTER TABLE [dbo].[tbHorarios] CHECK CONSTRAINT [CK_tbHorarios_horaSalida_mayor_horaEntrada]
GO
ALTER TABLE [dbo].[tbHorarios]  WITH CHECK ADD  CONSTRAINT [CK_tbHorarios_pagoPorHora_positivo] CHECK  (([pagoPorHora]>(0)))
GO
ALTER TABLE [dbo].[tbHorarios] CHECK CONSTRAINT [CK_tbHorarios_pagoPorHora_positivo]
GO
USE [master]
GO
ALTER DATABASE [DB_BUENISIMO] SET  READ_WRITE 
GO

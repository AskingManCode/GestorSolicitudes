-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SCRIPT: 01. GestorSolicitudesDB.sql
-- DESCRIPCIÓN: Creación de la base de datos y tablas del sistema
-- MOTOR: MariaDB 10.2.1+
-- =======================================================

SET @bd_existe = (
	SELECT COUNT(*) FROM information_schema.SCHEMATA
	WHERE SCHEMA_NAME = 'tiusr20pl_Publica_SA'
);
SET @sql_crear_bd = IF(
	@bd_existe = 0,
	'CREATE DATABASE `tiusr20pl_Publica_SA` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci',
	'DO 0'
);
PREPARE stmt_crear_bd FROM @sql_crear_bd;
EXECUTE stmt_crear_bd;
DEALLOCATE PREPARE stmt_crear_bd;

USE `tiusr20pl_Publica_SA`;


-- Tabla: EstadosUsuario
CREATE TABLE IF NOT EXISTS `EstadosUsuario` (
	`CodigoEstado` VARCHAR(20) NOT NULL PRIMARY KEY,
	`Nombre` VARCHAR(50) NOT NULL UNIQUE,
	`Activo` BOOLEAN NOT NULL DEFAULT TRUE,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	CONSTRAINT `CHK_EstadosUsuario_Codigo`
	   CHECK (`CodigoEstado` REGEXP '^[A-Z_]{2,20}$'),
	
	CONSTRAINT `CHK_EstadosUsuario_Nombre`
		CHECK (
		   CHAR_LENGTH(TRIM(`Nombre`)) >= 2
		   AND CHAR_LENGTH(`Nombre`) = CHAR_LENGTH(TRIM(`Nombre`))
		)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: EstadosSolicitud 
CREATE TABLE IF NOT EXISTS `EstadosSolicitud` (
	`CodigoEstado` VARCHAR(20) NOT NULL PRIMARY KEY,
	`Nombre` VARCHAR(50) NOT NULL UNIQUE,
	`Activo` BOOLEAN NOT NULL DEFAULT TRUE,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	CONSTRAINT `CHK_EstadosSolicitud_Codigo`
		CHECK (`CodigoEstado` REGEXP '^[A-Z_]{2,20}$'),
	
	CONSTRAINT `CHK_EstadosSolicitud_Nombre`
		CHECK (
		   CHAR_LENGTH(TRIM(`Nombre`)) >= 2
		   AND CHAR_LENGTH(`Nombre`) = CHAR_LENGTH(TRIM(`Nombre`))
		)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: Usuarios
CREATE TABLE IF NOT EXISTS `Usuarios` (
	`IdUsuario` INT AUTO_INCREMENT PRIMARY KEY,
	`NombreUsuario` VARCHAR(50) NOT NULL UNIQUE,
	`NombreCompleto` VARCHAR(150) NOT NULL,
	`Correo` VARCHAR(150) NOT NULL,
	`Contrasena` VARCHAR(500) NOT NULL, -- AES-GCM 256
	`CodigoEstado` VARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
	`IntentosFallidos` INT NOT NULL DEFAULT 0,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	CONSTRAINT `UQ_Usuarios_Correo`
		UNIQUE (`Correo`),
	
	CONSTRAINT `FK_Usuarios_EstadosUsuario`
		FOREIGN KEY (`CodigoEstado`)
		REFERENCES `EstadosUsuario` (`CodigoEstado`)
		ON DELETE RESTRICT ON UPDATE CASCADE,
	
	CONSTRAINT `CHK_Usuarios_NombreUsuario`
		CHECK (`NombreUsuario` REGEXP '^[A-Za-z0-9._-]{3,50}$'),
	
	CONSTRAINT `CHK_Usuarios_NombreCompleto`
		CHECK (
		   CHAR_LENGTH(TRIM(`NombreCompleto`)) >= 3
		   AND CHAR_LENGTH(`NombreCompleto`) = CHAR_LENGTH(TRIM(`NombreCompleto`))
		   AND `NombreCompleto` NOT REGEXP '[0-9]'
		),
	
	CONSTRAINT `CHK_Usuarios_Correo`
		CHECK (`Correo` REGEXP '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$'),
	
	CONSTRAINT `CHK_Usuarios_Contrasena`
		CHECK (
		   CHAR_LENGTH(`Contrasena`) >= 24
		   AND `Contrasena` NOT REGEXP '[[:space:]]'
		),
	
	CONSTRAINT `CHK_Usuarios_IntentosFallidos`
		CHECK (`IntentosFallidos` >= 0 AND `IntentosFallidos` <= 3)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: Representantes
CREATE TABLE IF NOT EXISTS `Representantes` (
	`IdRepresentante` INT AUTO_INCREMENT PRIMARY KEY,
	`Nombre` VARCHAR(150) NOT NULL,
	`Email` VARCHAR(150) NOT NULL,
	`Activo` BOOLEAN NOT NULL DEFAULT TRUE,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	CONSTRAINT `UQ_Representantes_Email`
	   UNIQUE (`Email`),
	
	CONSTRAINT `CHK_Representantes_Nombre`
		CHECK (
		   CHAR_LENGTH(TRIM(`Nombre`)) >= 3
		   AND CHAR_LENGTH(`Nombre`) = CHAR_LENGTH(TRIM(`Nombre`))
		   AND `Nombre` NOT REGEXP '[0-9]'
		),
	
	CONSTRAINT `CHK_Representantes_Email`
	   CHECK (`Email` REGEXP '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$')
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: Solicitudes
CREATE TABLE IF NOT EXISTS `Solicitudes` (
	`IdSolicitud` INT AUTO_INCREMENT PRIMARY KEY,
	`ConsecutivoOficio` VARCHAR(50) NOT NULL UNIQUE,
	`DocumentoRespuesta` VARCHAR(255) NULL,
	`DocumentoInicio` VARCHAR(255) NULL,
	`Titulo` VARCHAR(200) NOT NULL,
	`Descripcion` TEXT NULL,
	`IdRepresentante` INT NOT NULL,
	`Observaciones` TEXT NULL,
	`CodigoEstado` VARCHAR(20) NOT NULL DEFAULT 'NUEVA',
	`FechaIngreso` DATE NOT NULL,
	`FechaRespuesta` DATE NULL,
	`FechaInicio` DATE NULL,
	`Activo` BOOLEAN NOT NULL DEFAULT TRUE,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	-- Apoya el proceso automático de vencimiento y el listado por estado
	INDEX `IX_Solicitudes_Estado_FechaIngreso` (`CodigoEstado`, `FechaIngreso`),
	
	CONSTRAINT `FK_Solicitudes_Representantes`
		FOREIGN KEY (`IdRepresentante`)
		REFERENCES `Representantes` (`IdRepresentante`)
		ON DELETE RESTRICT ON UPDATE CASCADE,
	
	CONSTRAINT `FK_Solicitudes_EstadosSolicitud`
		FOREIGN KEY (`CodigoEstado`)
		REFERENCES `EstadosSolicitud` (`CodigoEstado`)
		ON DELETE RESTRICT ON UPDATE CASCADE,
	
	CONSTRAINT `CHK_Solicitudes_Consecutivo`
		CHECK (
		   CHAR_LENGTH(TRIM(`ConsecutivoOficio`)) >= 1
		   AND CHAR_LENGTH(`ConsecutivoOficio`) = CHAR_LENGTH(TRIM(`ConsecutivoOficio`))
		),
	
	CONSTRAINT `CHK_Solicitudes_Titulo`
		CHECK (
		   CHAR_LENGTH(TRIM(`Titulo`)) >= 3
		   AND CHAR_LENGTH(`Titulo`) = CHAR_LENGTH(TRIM(`Titulo`))
		),
	
	-- Opcionales: si entran a la tabla, no pueden ser texto vacío
	CONSTRAINT `CHK_Solicitudes_DocumentoRespuesta`
	   CHECK (`DocumentoRespuesta` IS NULL OR CHAR_LENGTH(TRIM(`DocumentoRespuesta`)) > 0),
	
	CONSTRAINT `CHK_Solicitudes_DocumentoInicio`
	   CHECK (`DocumentoInicio` IS NULL OR CHAR_LENGTH(TRIM(`DocumentoInicio`)) > 0),
	
	CONSTRAINT `CHK_Solicitudes_Descripcion`
	   CHECK (`Descripcion` IS NULL OR CHAR_LENGTH(TRIM(`Descripcion`)) > 0),
	
	CONSTRAINT `CHK_Solicitudes_Observaciones`
	   CHECK (`Observaciones` IS NULL OR CHAR_LENGTH(TRIM(`Observaciones`)) > 0),
	
	CONSTRAINT `CHK_Solicitudes_RangoFechas`
		CHECK (
		   `FechaIngreso` BETWEEN '2000-01-01' AND '2150-12-31'
		   AND (`FechaRespuesta` IS NULL OR `FechaRespuesta` <= '2150-12-31')
		   AND (`FechaInicio` IS NULL OR `FechaInicio` <= '2150-12-31')
		),
	
	CONSTRAINT `CHK_Solicitudes_Fechas`
		CHECK (
		   (`FechaRespuesta` IS NULL OR `FechaRespuesta` >= `FechaIngreso`)
		   AND (`FechaInicio` IS NULL OR `FechaInicio` >= `FechaIngreso`)
		)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: DesglosesSolicitud
CREATE TABLE IF NOT EXISTS `DesglosesSolicitud` (
	`IdDesglose` INT AUTO_INCREMENT PRIMARY KEY,
	`IdSolicitud` INT NOT NULL,
	`Mes` INT NOT NULL,
	`Anio` INT NOT NULL,
	`Horas` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
	`Monto` DECIMAL(14,2) NOT NULL DEFAULT 0.00,
	`Iva` DECIMAL(14,2) NOT NULL DEFAULT 0.00,
	`Total` DECIMAL(14,2) NOT NULL DEFAULT 0.00,
	`PorcentajeCobro` DECIMAL(5,2) NOT NULL DEFAULT 0.00,
	`Observaciones` TEXT NULL,
	`Activo` BOOLEAN NOT NULL DEFAULT TRUE,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	-- Apoya el reporte mensual de facturación
	INDEX `IX_Desgloses_Anio_Mes` (`Anio`, `Mes`),
	
	CONSTRAINT `FK_Desgloses_Solicitudes`
		FOREIGN KEY (`IdSolicitud`)
		REFERENCES `Solicitudes` (`IdSolicitud`)
		ON DELETE RESTRICT ON UPDATE CASCADE,

	CONSTRAINT `UQ_Desglose_Solicitud_Mes_Anio`
	   UNIQUE (`IdSolicitud`, `Mes`, `Anio`),
	
	CONSTRAINT `CHK_Desgloses_Mes`
	   CHECK (`Mes` BETWEEN 1 AND 12),
	
	CONSTRAINT `CHK_Desgloses_Anio`
	   CHECK (`Anio` BETWEEN 2000 AND 2150),
	
	CONSTRAINT `CHK_Desgloses_ValoresPositivos`
		CHECK (
		   `Horas` >= 0
		   AND `Monto` >= 0
		   AND `Iva` >= 0
		   AND `Total` >= 0
		   AND `PorcentajeCobro` >= 0
		   AND `PorcentajeCobro` <= 100
		),
	
	-- IVA = 13% del monto.
	CONSTRAINT `CHK_Desgloses_Iva`
	   CHECK (ABS(`Iva` - (`Monto` * 0.13)) <= 0.005),
	
	-- Total = Monto + IVA
	CONSTRAINT `CHK_Desgloses_Total`
	   CHECK (`Total` = `Monto` + `Iva`),
	
	CONSTRAINT `CHK_Desgloses_Observaciones`
	   CHECK (`Observaciones` IS NULL OR CHAR_LENGTH(TRIM(`Observaciones`)) > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: Tareas
CREATE TABLE IF NOT EXISTS `Tareas` (
	`IdTarea` INT AUTO_INCREMENT PRIMARY KEY,
	`IdSolicitud` INT NOT NULL,
	`IdUsuario` INT NOT NULL,
	`Descripcion` TEXT NOT NULL,
	`Fecha` DATE NOT NULL,
	`HorasInvertidas` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
	`Activo` BOOLEAN NOT NULL DEFAULT TRUE,
	`FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	
	-- Apoyan los reportes por usuario/mes y por solicitud/mes
	INDEX `IX_Tareas_Usuario_Fecha` (`IdUsuario`, `Fecha`),
	INDEX `IX_Tareas_Solicitud_Fecha` (`IdSolicitud`, `Fecha`),
	
	CONSTRAINT `FK_Tareas_Solicitudes`
		FOREIGN KEY (`IdSolicitud`)
		REFERENCES `Solicitudes` (`IdSolicitud`)
		ON DELETE RESTRICT ON UPDATE CASCADE,
	
	CONSTRAINT `FK_Tareas_Usuarios`
		FOREIGN KEY (`IdUsuario`)
		REFERENCES `Usuarios` (`IdUsuario`)
		ON DELETE RESTRICT ON UPDATE CASCADE,
	
	CONSTRAINT `CHK_Tareas_Descripcion`
	   CHECK (CHAR_LENGTH(TRIM(`Descripcion`)) >= 3),
	
	CONSTRAINT `CHK_Tareas_Fecha`
	   CHECK (`Fecha` BETWEEN '2000-01-01' AND '2150-12-31'),
	
	CONSTRAINT `CHK_Tareas_Horas`
	   CHECK (`HorasInvertidas` > 0 AND `HorasInvertidas` <= 24)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla: Bitacoras
CREATE TABLE IF NOT EXISTS `Bitacoras` (
	`IdBitacora` INT AUTO_INCREMENT PRIMARY KEY,
	`FechaBitacora` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`UsuarioEjecuta` VARCHAR(50) NOT NULL,
	`IdUsuario` INT NULL,
	`Accion` VARCHAR(100) NOT NULL,
	`Detalle` LONGTEXT NOT NULL,
	
	INDEX `IX_Bitacoras_Fecha` (`FechaBitacora`),
	INDEX `IX_Bitacoras_Usuario_Fecha` (`UsuarioEjecuta`, `FechaBitacora`),
	
	CONSTRAINT `FK_Bitacoras_Usuario`
		FOREIGN KEY (`IdUsuario`)
		REFERENCES `Usuarios` (`IdUsuario`),
	
	CONSTRAINT `CHK_Bitacoras_UsuarioEjecuta`
	   CHECK (CHAR_LENGTH(TRIM(`UsuarioEjecuta`)) >= 1),
	
	CONSTRAINT `CHK_Bitacoras_Accion`
	   CHECK (CHAR_LENGTH(TRIM(`Accion`)) >= 1),
	
	CONSTRAINT `CHK_Bitacoras_Detalle`
	   CHECK (CHAR_LENGTH(TRIM(`Detalle`)) >= 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;



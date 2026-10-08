-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SCRIPT: 01. GestorSolicitudesDB.sql
-- DESCRIPCIÓN: Creación de la base de datos y tablas del sistema
-- MOTOR: MySQL 8.0+
-- =======================================================

CREATE DATABASE IF NOT EXISTS `gestorsolicitudes_db` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `gestorsolicitudes_db`;

-- 1. Tabla: Usuarios
CREATE TABLE IF NOT EXISTS `Usuarios` (
    `IdUsuario` INT AUTO_INCREMENT PRIMARY KEY,
    `NombreUsuario` VARCHAR(50) NOT NULL UNIQUE,
    `NombreCompleto` VARCHAR(150) NOT NULL,
    `Correo` VARCHAR(150) NOT NULL,
    `Contrasena` VARCHAR(500) NOT NULL,
    `Estado` ENUM('Activo', 'Inactivo', 'Bloqueado') NOT NULL DEFAULT 'Activo',
    `IntentosFallidos` INT NOT NULL DEFAULT 0,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. Tabla: Representantes
CREATE TABLE IF NOT EXISTS `Representantes` (
    `IdRepresentante` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(150) NOT NULL,
    `Email` VARCHAR(150) NOT NULL,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. Tabla: EstadosSolicitud
CREATE TABLE IF NOT EXISTS `EstadosSolicitud` (
    `IdEstado` INT NOT NULL PRIMARY KEY,
    `Nombre` VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4. Tabla: Solicitudes
CREATE TABLE IF NOT EXISTS `Solicitudes` (
    `IdSolicitud` INT AUTO_INCREMENT PRIMARY KEY,
    `ConsecutivoOficio` VARCHAR(50) NOT NULL UNIQUE,
    `DocumentoRespuesta` VARCHAR(255) NULL,
    `DocumentoInicio` VARCHAR(255) NULL,
    `Titulo` VARCHAR(200) NOT NULL,
    `Descripcion` TEXT NULL,
    `IdRepresentante` INT NOT NULL,
    `Observaciones` TEXT NULL,
    `IdEstado` INT NOT NULL,
    `FechaIngreso` DATE NOT NULL,
    `FechaRespuesta` DATE NULL,
    `FechaInicio` DATE NULL,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `FK_Solicitudes_Representantes` FOREIGN KEY (`IdRepresentante`) 
        REFERENCES `Representantes` (`IdRepresentante`) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT `FK_Solicitudes_EstadosSolicitud` FOREIGN KEY (`IdEstado`) 
        REFERENCES `EstadosSolicitud` (`IdEstado`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5. Tabla: DesglosesSolicitud
CREATE TABLE IF NOT EXISTS `DesglosesSolicitud` (
    `IdDesglose` INT AUTO_INCREMENT PRIMARY KEY,
    `IdSolicitud` INT NOT NULL,
    `Mes` INT NOT NULL,
    `Anio` INT NOT NULL,
    `Horas` DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    `Monto` DECIMAL(14, 2) NOT NULL DEFAULT 0.00,
    `Iva` DECIMAL(14, 2) NOT NULL DEFAULT 0.00,
    `Total` DECIMAL(14, 2) NOT NULL DEFAULT 0.00,
    `PorcentajeCobro` DECIMAL(5, 2) NOT NULL DEFAULT 0.00,
    `Observaciones` TEXT NULL,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `FK_Desgloses_Solicitudes` FOREIGN KEY (`IdSolicitud`) 
        REFERENCES `Solicitudes` (`IdSolicitud`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 6. Tabla: Tareas
CREATE TABLE IF NOT EXISTS `Tareas` (
    `IdTarea` INT AUTO_INCREMENT PRIMARY KEY,
    `IdSolicitud` INT NOT NULL,
    `IdUsuario` INT NOT NULL,
    `Descripcion` TEXT NOT NULL,
    `Fecha` DATE NOT NULL,
    `HorasInvertidas` DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `FK_Tareas_Solicitudes` FOREIGN KEY (`IdSolicitud`) 
        REFERENCES `Solicitudes` (`IdSolicitud`) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT `FK_Tareas_Usuarios` FOREIGN KEY (`IdUsuario`) 
        REFERENCES `Usuarios` (`IdUsuario`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 7. Tabla: Bitacoras
CREATE TABLE IF NOT EXISTS `Bitacoras` (
    `IdBitacora` INT AUTO_INCREMENT PRIMARY KEY,
    `FechaBitacora` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UsuarioEjecuta` VARCHAR(50) NOT NULL,
    `Accion` VARCHAR(100) NOT NULL,
    `Detalle` LONGTEXT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

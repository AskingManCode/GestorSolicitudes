-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SCRIPT: 02. DatosIniciales.sql
-- DESCRIPCIÓN: Datos iniciales y catálogos requeridos
-- MOTOR: MySQL 8.0+
-- =======================================================

USE `gestorsolicitudes_db`;

-- 1. Estados requeridos de la solicitud (HU SOL4)
INSERT INTO `EstadosSolicitud` (`IdEstado`, `Nombre`) VALUES
(1, 'Nueva'),
(2, 'Respondida'),
(3, 'Iniciada'),
(4, 'Finalizada'),
(5, 'Vencida')
ON DUPLICATE KEY UPDATE `Nombre` = VALUES(`Nombre`);

-- 2. Representantes iniciales para pruebas (HU SOL3)
INSERT INTO `Representantes` (`IdRepresentante`, `Nombre`, `Email`) VALUES
(1, 'Carlos Alvarado Solano', 'carlos.alvarado@publicasa.cr'),
(2, 'María Fernanda Rojas', 'maria.rojas@publicasa.cr'),
(3, 'Alejandro Morales Vega', 'alejandro.morales@publicasa.cr')
ON DUPLICATE KEY UPDATE `Nombre` = VALUES(`Nombre`), `Email` = VALUES(`Email`);

-- 3. Usuario administrador inicial (HU USR1, USR4)
-- Contraseña cifrada en AES-GCM 256 para 'Admin123*'
INSERT INTO `Usuarios` (`NombreUsuario`, `NombreCompleto`, `Correo`, `Contrasena`, `Estado`, `IntentosFallidos`, `FechaCreacion`)
VALUES
('admin', 'Administrador del Sistema', 'admin@publicasa.cr', 'bDAxvz3QVsWJWBXSYxVIEZ/Fhp7TSVR7v4DEvzYTip3sKyu2BA==', 'Activo', 0, NOW())
ON DUPLICATE KEY UPDATE `NombreCompleto` = VALUES(`NombreCompleto`);

-- 4. Solicitudes de prueba (HU SOL1) para demostración del semáforo y validaciones
INSERT INTO `Solicitudes` (`ConsecutivoOficio`, `Titulo`, `Descripcion`, `IdRepresentante`, `IdEstado`, `FechaIngreso`, `FechaCreacion`)
VALUES 
('OFI-2026-001', 'Mantenimiento preventivo de servidores', 'Revisión y actualización de parches de seguridad', 1, 1, '2026-10-07', NOW()),
('OFI-2026-002', 'Desarrollo de módulo de facturación', 'Integración con pasarela de pagos', 2, 1, '2026-09-30', NOW()),
('OFI-2026-003', 'Renovación de licencias de software', 'Adquisición de paquetes anuales de software de diseño', 3, 5, '2026-09-20', NOW()),
('OFI-2026-004', 'Auditoría de ciberseguridad', 'Evaluación de vulnerabilidades en red interna', 1, 3, '2026-10-02', NOW())
ON DUPLICATE KEY UPDATE `Titulo` = VALUES(`Titulo`);

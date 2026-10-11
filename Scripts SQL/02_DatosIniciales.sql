-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SCRIPT: 02. DatosIniciales.sql
-- DESCRIPCIÓN: Datos iniciales y catálogos requeridos
-- MOTOR: MariaDB 10.2.1+
-- =======================================================

USE `tiusr20pl_Publica_SA`;

-- 1. Estados requeridos del usuario
INSERT INTO `EstadosUsuario` (`CodigoEstado`, `Nombre`) VALUES
('ACTIVO', 'Activo'),
('INACTIVO', 'Inactivo'),
('BLOQUEADO', 'Bloqueado')
ON DUPLICATE KEY UPDATE `Nombre` = VALUES(`Nombre`);

-- 2. Estados requeridos de la solicitud
INSERT INTO `EstadosSolicitud` (`CodigoEstado`, `Nombre`) VALUES
('NUEVA', 'Nueva'),
('RESPONDIDA', 'Respondida'),
('INICIADA', 'Iniciada'),
('FINALIZADA', 'Finalizada'),
('VENCIDA', 'Vencida')
ON DUPLICATE KEY UPDATE `Nombre` = VALUES(`Nombre`);

-- 3. Representantes iniciales para pruebas
INSERT INTO `Representantes` (`IdRepresentante`, `Nombre`, `Email`) VALUES
(1, 'Carlos Alvarado Solano', 'carlos.alvarado@publicasa.cr'),
(2, 'María Fernanda Rojas', 'maria.rojas@publicasa.cr'),
(3, 'Alejandro Morales Vega', 'alejandro.morales@publicasa.cr')
ON DUPLICATE KEY UPDATE `Nombre` = VALUES(`Nombre`), `Email` = VALUES(`Email`);

-- 4. Usuario administrador inicial
-- Contraseña cifrada en AES-GCM 256 para 'Admin123*'
INSERT INTO `Usuarios` (`NombreUsuario`, `NombreCompleto`, `Correo`, `Contrasena`, `CodigoEstado`, `IntentosFallidos`, `FechaCreacion`)
VALUES
('admin', 'Administrador del Sistema', 'admin@publicasa.cr', 'bDAxvz3QVsWJWBXSYxVIEZ/Fhp7TSVR7v4DEvzYTip3sKyu2BA==', 'ACTIVO', 0, NOW())
ON DUPLICATE KEY UPDATE `NombreCompleto` = VALUES(`NombreCompleto`);

-- 5. Solicitudes de prueba para demostración del semáforo y validaciones
INSERT INTO `Solicitudes` (`ConsecutivoOficio`, `Titulo`, `Descripcion`, `IdRepresentante`, `CodigoEstado`, `FechaIngreso`, `FechaCreacion`)
VALUES 
('OFI-2026-001', 'Mantenimiento preventivo de servidores', 'Revisión y actualización de parches de seguridad', 1, 'NUEVA', '2026-10-07', NOW()),
('OFI-2026-002', 'Desarrollo de módulo de facturación', 'Integración con pasarela de pagos', 2, 'NUEVA', '2026-09-30', NOW()),
('OFI-2026-003', 'Renovación de licencias de software', 'Adquisición de paquetes anuales de software de diseño', 3, 'VENCIDA', '2026-09-20', NOW()),
('OFI-2026-004', 'Auditoría de ciberseguridad', 'Evaluación de vulnerabilidades en red interna', 1, 'INICIADA', '2026-10-02', NOW())
ON DUPLICATE KEY UPDATE `Titulo` = VALUES(`Titulo`);
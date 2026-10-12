-- =======================================================
-- SCRIPT: sp_Usuarios_Crear.sql
-- DESCRIPCIÓN: Crea un usuario y devuelve su IdUsuario
-- MOTOR: MariaDB 10.2.1+
-- =======================================================
USE `tiusr20pl_Publica_SA`;

DROP PROCEDURE IF EXISTS `sp_Usuarios_Crear`;

DELIMITER $$

CREATE PROCEDURE `sp_Usuarios_Crear`(
	IN p_NombreUsuario VARCHAR(50),
	IN p_NombreCompleto VARCHAR(150),
	IN p_Correo VARCHAR(150),
	IN p_Contrasena VARCHAR(500),
	IN p_CodigoEstado VARCHAR(20)
)
BEGIN
	IF EXISTS (SELECT 1 FROM `Usuarios` WHERE `NombreUsuario` = p_NombreUsuario) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El nombre de usuario ya está registrado.';
	END IF;

	IF EXISTS (SELECT 1 FROM `Usuarios` WHERE `Correo` = p_Correo) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El correo ya está registrado.';
	END IF;

	IF NOT EXISTS (
		SELECT 1 FROM `EstadosUsuario`
		WHERE `CodigoEstado` = p_CodigoEstado AND `Activo` = TRUE
	) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El estado de usuario indicado no existe o está inactivo.';
	END IF;

	INSERT INTO `Usuarios` (`NombreUsuario`, `NombreCompleto`, `Correo`, `Contrasena`, `CodigoEstado`)
	VALUES (p_NombreUsuario, p_NombreCompleto, p_Correo, p_Contrasena, p_CodigoEstado);

	SELECT LAST_INSERT_ID() AS `IdUsuario`;
END$$

DELIMITER ;
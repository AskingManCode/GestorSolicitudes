USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_Editar`$$

CREATE PROCEDURE `sp_Usuarios_Editar`(
    IN p_IdUsuario INT,
    IN p_NombreCompleto VARCHAR(150),
    IN p_Correo VARCHAR(150),
    IN p_NombreUsuario VARCHAR(50),
    IN p_CodigoEstado VARCHAR(20),
    IN p_ContrasenaNueva VARCHAR(500)  -- Solo la nueva, ya validada
)
BEGIN
    -- Validar que el usuario existe
    IF NOT EXISTS (SELECT 1 FROM `Usuarios` WHERE `IdUsuario` = p_IdUsuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El usuario no existe.';
    END IF;

    -- Validar que el correo no esté en uso por otro usuario
    IF EXISTS (SELECT 1 FROM `Usuarios` WHERE `Correo` = p_Correo AND `IdUsuario` != p_IdUsuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El correo ya está registrado por otro usuario.';
    END IF;

    -- Validar que el nombre de usuario no esté en uso por otro
    IF EXISTS (SELECT 1 FROM `Usuarios` WHERE `NombreUsuario` = p_NombreUsuario AND `IdUsuario` != p_IdUsuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El nombre de usuario ya está registrado.';
    END IF;

    -- Validar estado
    IF NOT EXISTS (SELECT 1 FROM `EstadosUsuario` WHERE `CodigoEstado` = p_CodigoEstado AND `Activo` = TRUE) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El estado indicado no existe o está inactivo.';
    END IF;

    -- Actualizar (con o sin contraseña)
    IF p_ContrasenaNueva IS NOT NULL AND p_ContrasenaNueva != '' THEN
        UPDATE `Usuarios` 
        SET `NombreCompleto` = p_NombreCompleto,
            `Correo` = p_Correo,
            `NombreUsuario` = p_NombreUsuario,
            `CodigoEstado` = p_CodigoEstado,
            `Contrasena` = p_ContrasenaNueva
        WHERE `IdUsuario` = p_IdUsuario;
    ELSE
        UPDATE `Usuarios` 
        SET `NombreCompleto` = p_NombreCompleto,
            `Correo` = p_Correo,
            `NombreUsuario` = p_NombreUsuario,
            `CodigoEstado` = p_CodigoEstado
        WHERE `IdUsuario` = p_IdUsuario;
    END IF;

    -- Retornar usuario actualizado
    SELECT 
        u.`IdUsuario`,
        u.`NombreUsuario`,
        u.`NombreCompleto`,
        u.`Correo`,
        eu.`Nombre` AS `Estado`
    FROM `Usuarios` u
    INNER JOIN `EstadosUsuario` eu ON eu.`CodigoEstado` = u.`CodigoEstado`
    WHERE u.`IdUsuario` = p_IdUsuario;
END$$

DELIMITER ;
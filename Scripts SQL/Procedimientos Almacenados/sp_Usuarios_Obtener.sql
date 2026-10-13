USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_Obtener`$$

CREATE PROCEDURE `sp_Usuarios_Obtener`(
	IN p_IdUsuario int
)
BEGIN
    SELECT 
        u.`IdUsuario`,
        u.`NombreUsuario`,
        u.`NombreCompleto`,
        u.`Correo`,
        u.`CodigoEstado`
    FROM `Usuarios` u
    WHERE u.`IdUsuario` = p_IdUsuario;
END$$

DELIMITER ;
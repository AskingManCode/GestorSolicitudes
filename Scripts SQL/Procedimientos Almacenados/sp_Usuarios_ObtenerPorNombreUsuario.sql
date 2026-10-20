USE tiusr20pl_Publica_SA;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_ObtenerPorNombreUsuario`$$

CREATE PROCEDURE `sp_Usuarios_ObtenerPorNombreUsuario`(
    IN p_NombreUsuario VARCHAR(50)
)
BEGIN
    SELECT 
        u.`IdUsuario`,
        u.`NombreUsuario`,
        u.`NombreCompleto`,
        u.`Contrasena`,
        u.`CodigoEstado`,
        u.`IntentosFallidos`
    FROM `Usuarios` u
    WHERE u.`NombreUsuario` = p_NombreUsuario;
END$$

DELIMITER ;
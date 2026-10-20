USE tiusr20pl_Publica_SA;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_ActualizarIntentosYEstado`$$

CREATE PROCEDURE `sp_Usuarios_ActualizarIntentosYEstado`(
    IN p_IdUsuario INT,
    IN p_IntentosFallidos INT,
    IN p_CodigoEstado VARCHAR(20)
)
BEGIN
    IF p_CodigoEstado IS NOT NULL AND p_CodigoEstado != '' THEN
        UPDATE `Usuarios`
        SET `IntentosFallidos` = p_IntentosFallidos,
            `CodigoEstado` = p_CodigoEstado
        WHERE `IdUsuario` = p_IdUsuario;
    ELSE
        UPDATE `Usuarios`
        SET `IntentosFallidos` = p_IntentosFallidos
        WHERE `IdUsuario` = p_IdUsuario;
    END IF;
END$$

DELIMITER ;
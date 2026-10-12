-- =======================================================
-- SP: Usuarios - Cargar Estados
-- =======================================================
USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_EstadosUsuario_Listar`$$

CREATE PROCEDURE `sp_EstadosUsuario_Listar`()
BEGIN
    SELECT 
        `CodigoEstado`,
        `Nombre`
    FROM `EstadosUsuario`
    WHERE `Activo` = TRUE
    ORDER BY 
        CASE `CodigoEstado`
            WHEN 'ACTIVO'    THEN 1
            WHEN 'INACTIVO'  THEN 2
            WHEN 'BLOQUEADO' THEN 3
            ELSE 4
        END;
END$$

DELIMITER ;
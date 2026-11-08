-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SP: sp_Solicitudes_ActualizarPublicacion
-- DESCRIPCIÓN: Actualiza el estado de visualización externa y la fecha de vencimiento de una solicitud.
-- =======================================================

USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Solicitudes_ActualizarPublicacion`$$

CREATE PROCEDURE `sp_Solicitudes_ActualizarPublicacion`(
    IN p_IdSolicitud INT,
    IN p_VisibleExterno BOOLEAN,
    IN p_FechaVencimientoPublicacion DATE
)
BEGIN
    -- Validar que la solicitud existe y está activa
    IF NOT EXISTS (SELECT 1 FROM `Solicitudes` WHERE `IdSolicitud` = p_IdSolicitud AND `Activo` = TRUE) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La solicitud no existe o se encuentra inactiva.';
    END IF;

    -- Validar que si se publica externamente, la fecha de vencimiento sea obligatoria
    IF p_VisibleExterno = TRUE AND p_FechaVencimientoPublicacion IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Debe indicar una fecha de vencimiento para visualizar la solicitud en el módulo externo.';
    END IF;

    -- Si no se visualiza externamente, limpiar la fecha de vencimiento
    IF p_VisibleExterno = FALSE THEN
        SET p_FechaVencimientoPublicacion = NULL;
    END IF;

    -- Actualizar configuración de publicación
    UPDATE `Solicitudes`
    SET `VisibleExterno` = p_VisibleExterno,
        `FechaVencimientoPublicacion` = p_FechaVencimientoPublicacion
    WHERE `IdSolicitud` = p_IdSolicitud;

    -- Retornar la solicitud actualizada con los nombres de representante y estado
    SELECT 
        s.*,
        r.`Nombre` AS `NombreRepresentante`,
        r.`Email` AS `EmailRepresentante`,
        e.`Nombre` AS `NombreEstado`
    FROM `Solicitudes` s
    INNER JOIN `Representantes` r ON s.`IdRepresentante` = r.`IdRepresentante`
    INNER JOIN `EstadosSolicitud` e ON s.`CodigoEstado` = e.`CodigoEstado`
    WHERE s.`IdSolicitud` = p_IdSolicitud;
END$$

DELIMITER ;

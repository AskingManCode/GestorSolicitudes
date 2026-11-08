-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SP: sp_Propuestas_ListarPorSolicitud
-- DESCRIPCIÓN: Lista todas las participaciones (propuestas) realizadas por proveedores para una solicitud.
-- =======================================================

USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Propuestas_ListarPorSolicitud`$$

CREATE PROCEDURE `sp_Propuestas_ListarPorSolicitud`(
    IN p_IdSolicitud INT
)
BEGIN
    -- Validar que la solicitud exista
    IF NOT EXISTS (SELECT 1 FROM `Solicitudes` WHERE `IdSolicitud` = p_IdSolicitud AND `Activo` = TRUE) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La solicitud indicada no existe.';
    END IF;

    SELECT 
        pr.`IdPropuesta`,
        pr.`IdSolicitud`,
        pr.`IdProveedor`,
        pr.`Aprobada`,
        pr.`FechaPropuesta`,
        pr.`FechaAprobacion`,
        pr.`Activo`,
        s.`ConsecutivoOficio`,
        s.`Titulo` AS `TituloSolicitud`,
        pv.`CedulaJuridica`,
        pv.`Nombre` AS `NombreProveedor`,
        pv.`NombreRepresentante`,
        pv.`Telefono`,
        pv.`Correo`,
        EXISTS(
            SELECT 1 FROM `Propuestas` p2 
            WHERE p2.`IdSolicitud` = pr.`IdSolicitud` 
              AND p2.`Aprobada` = TRUE 
              AND p2.`Activo` = TRUE
        ) AS `SolicitudTienePropuestaAprobada`
    FROM `Propuestas` pr
    INNER JOIN `Proveedores` pv ON pr.`IdProveedor` = pv.`IdProveedor`
    INNER JOIN `Solicitudes` s ON pr.`IdSolicitud` = s.`IdSolicitud`
    WHERE pr.`IdSolicitud` = p_IdSolicitud
      AND pr.`Activo` = TRUE
    ORDER BY pr.`Aprobada` DESC, pr.`FechaPropuesta` DESC;
END$$

DELIMITER ;

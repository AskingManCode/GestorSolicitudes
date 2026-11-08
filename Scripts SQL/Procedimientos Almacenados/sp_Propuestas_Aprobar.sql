-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SP: sp_Propuestas_Aprobar
-- DESCRIPCIÓN: Aprueba una propuesta de un proveedor y asocia su desglose propuesto a la solicitud en firme.
-- =======================================================

USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Propuestas_Aprobar`$$

CREATE PROCEDURE `sp_Propuestas_Aprobar`(
    IN p_IdPropuesta INT,
    IN p_IdSolicitud INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Validar que la solicitud exista y bloquear fila para evitar condiciones de carrera
    IF NOT EXISTS (SELECT 1 FROM `Solicitudes` WHERE `IdSolicitud` = p_IdSolicitud AND `Activo` = TRUE FOR UPDATE) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La solicitud indicada no existe o se encuentra inactiva.';
    END IF;

    -- Validar que la propuesta exista y pertenezca a la solicitud indicada
    IF NOT EXISTS (
        SELECT 1 FROM `Propuestas` 
        WHERE `IdPropuesta` = p_IdPropuesta 
          AND `IdSolicitud` = p_IdSolicitud 
          AND `Activo` = TRUE
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La participación seleccionada no existe para esta solicitud.';
    END IF;

    -- Validar que no se haya aprobado previamente una propuesta para esta solicitud
    IF EXISTS (
        SELECT 1 FROM `Propuestas` 
        WHERE `IdSolicitud` = p_IdSolicitud 
          AND `Aprobada` = TRUE 
          AND `Activo` = TRUE
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ya se aprobó una propuesta para esta solicitud.';
    END IF;

    -- Validar que la propuesta cuente con desgloses propuestos
    IF NOT EXISTS (
        SELECT 1 FROM `DesglosesPropuestas`
        WHERE `IdPropuesta` = p_IdPropuesta
          AND `Activo` = TRUE
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La propuesta no tiene desgloses registrados para asociar a la solicitud.';
    END IF;

    -- Marcar la propuesta como aprobada
    UPDATE `Propuestas`
    SET `Aprobada` = TRUE,
        `FechaAprobacion` = NOW()
    WHERE `IdPropuesta` = p_IdPropuesta
      AND `IdSolicitud` = p_IdSolicitud;

    -- Reemplazar desgloses previos de la solicitud (si existieran) por los de la propuesta aprobada
    DELETE FROM `DesglosesSolicitud`
    WHERE `IdSolicitud` = p_IdSolicitud;

    INSERT INTO `DesglosesSolicitud` (
        `IdSolicitud`,
        `Mes`,
        `Anio`,
        `Horas`,
        `Monto`,
        `Iva`,
        `Total`,
        `PorcentajeCobro`,
        `Observaciones`,
        `Activo`,
        `FechaCreacion`
    )
    SELECT 
        p_IdSolicitud,
        dp.`Mes`,
        dp.`Anio`,
        dp.`Horas`,
        dp.`Monto`,
        dp.`Iva`,
        dp.`Total`,
        dp.`PorcentajeCobro`,
        dp.`Observaciones`,
        TRUE,
        NOW()
    FROM `DesglosesPropuestas` dp
    WHERE dp.`IdPropuesta` = p_IdPropuesta
      AND dp.`Activo` = TRUE;

    COMMIT;

    -- Retornar la propuesta actualizada
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
        TRUE AS `SolicitudTienePropuestaAprobada`
    FROM `Propuestas` pr
    INNER JOIN `Proveedores` pv ON pr.`IdProveedor` = pv.`IdProveedor`
    INNER JOIN `Solicitudes` s ON pr.`IdSolicitud` = s.`IdSolicitud`
    WHERE pr.`IdPropuesta` = p_IdPropuesta;
END$$

DELIMITER ;

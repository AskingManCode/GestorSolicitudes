-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SP: sp_Propuestas_ObtenerDetalle
-- DESCRIPCIÓN: Obtiene el encabezado de una propuesta y el listado de sus desgloses propuestos.
-- =======================================================

USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Propuestas_ObtenerDetalle`$$

CREATE PROCEDURE `sp_Propuestas_ObtenerDetalle`(
    IN p_IdPropuesta INT
)
BEGIN
    -- Encabezado de la propuesta con datos del proveedor y de la solicitud
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
    WHERE pr.`IdPropuesta` = p_IdPropuesta
      AND pr.`Activo` = TRUE;

    -- Detalle de los desgloses propuestos
    SELECT 
        dp.`IdDesglosePropuesta`,
        dp.`IdPropuesta`,
        dp.`Mes`,
        dp.`Anio`,
        dp.`Horas`,
        dp.`Monto`,
        dp.`Iva`,
        dp.`Total`,
        dp.`PorcentajeCobro`,
        dp.`Observaciones`,
        dp.`Activo`,
        dp.`FechaCreacion`
    FROM `DesglosesPropuestas` dp
    WHERE dp.`IdPropuesta` = p_IdPropuesta
      AND dp.`Activo` = TRUE
    ORDER BY dp.`Anio` ASC, dp.`Mes` ASC, dp.`IdDesglosePropuesta` ASC;
END$$

DELIMITER ;

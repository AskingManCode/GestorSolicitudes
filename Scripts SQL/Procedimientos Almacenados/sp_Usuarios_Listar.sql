-- =======================================================
-- SP: Usuarios - Listar con paginación
-- =======================================================
USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_Listar`$$

CREATE PROCEDURE `sp_Usuarios_Listar`(
   IN p_Pagina INT,
   IN p_TamanoPagina INT
)
BEGIN
   DECLARE p_Offset INT;
   SET p_Offset = (p_Pagina - 1) * p_TamanoPagina;

   -- Total de registros activos
   SELECT COUNT(*) AS TotalRegistros
   FROM `Usuarios`
   WHERE 1=1;

   -- Página de datos
   SELECT 
      u.`IdUsuario`,
      u.`NombreUsuario`,
      u.`NombreCompleto`,
      u.`Correo`,
      e.`Nombre` AS `Estado`
   FROM `Usuarios` u
   INNER JOIN `EstadosUsuario` e 
		ON u.`CodigoEstado` = e.`CodigoEstado`
   ORDER BY u.`IdUsuario` DESC
   LIMIT p_TamanoPagina OFFSET p_Offset;
END$$

DELIMITER ;

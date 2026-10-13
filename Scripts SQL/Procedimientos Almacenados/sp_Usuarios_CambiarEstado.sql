-- =======================================================
-- SP: Usuarios - Cambiar Estado
-- =======================================================
USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_CambiarEstado`$$

CREATE PROCEDURE `sp_Usuarios_CambiarEstado`(
   IN p_IdUsuario INT,
   IN p_CodigoEstado VARCHAR(20)
)
BEGIN
   -- Validar que el usuario existe
   IF NOT EXISTS (SELECT 1 FROM `Usuarios` WHERE `IdUsuario` = p_IdUsuario) THEN
       SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El usuario no existe.';
   END IF;

   -- Validar que el estado existe y está activo
   IF NOT EXISTS (
       SELECT 1 FROM `EstadosUsuario` 
       WHERE `CodigoEstado` = p_CodigoEstado AND `Activo` = TRUE
   ) THEN
       SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El estado indicado no existe o está inactivo.';
   END IF;

   -- Actualizar estado
   UPDATE `Usuarios` 
   SET `CodigoEstado` = p_CodigoEstado 
   WHERE `IdUsuario` = p_IdUsuario;

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


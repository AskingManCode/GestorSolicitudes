USE `tiusr20pl_Publica_SA`;

DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Usuarios_Eliminar`$$

CREATE PROCEDURE `sp_Usuarios_Eliminar`(
	IN p_IdUsuario INT
)
BEGIN
    DECLARE v_Usuario VARCHAR(50);
    DECLARE v_TieneAsignaciones INT DEFAULT 0;
    
    -- Validar que el usuario existe
    IF NOT EXISTS (SELECT 1 FROM `Usuarios` WHERE `IdUsuario` = p_IdUsuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El usuario no existe.';
    END IF;
    
    -- Obtener nombre de usuario para validaciones
    SELECT `NombreUsuario` INTO v_Usuario 
    FROM `Usuarios` 
    WHERE `IdUsuario` = p_IdUsuario;
    
    -- Verificar si tiene asignaciones
    SELECT COUNT(*) INTO v_TieneAsignaciones
    FROM (
        SELECT 1 FROM `Tareas` WHERE `IdUsuario` = 16
        UNION ALL
        SELECT 1 FROM `Bitacoras` WHERE `IdUsuario` = 16
    ) AS Asignaciones;
    
    -- Si tiene asignaciones, no permitir eliminación
    IF v_TieneAsignaciones > 0 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'No se puede eliminar un registro con datos relacionados.';
    END IF;
    
    -- Eliminar usuario
    DELETE FROM `Usuarios` WHERE `IdUsuario` = p_IdUsuario;
    
    -- Retornar confirmación
    SELECT ROW_COUNT() AS FilasAfectadas;
END$$

DELIMITER ;
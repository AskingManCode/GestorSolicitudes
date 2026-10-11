-- =======================================================
-- PROYECTO: GESTOR DE SOLICITUDES - PÚBLICA S.A.
-- SCRIPT: 00. ReiniciarBD_SoloDesarrollo.sql
-- DESCRIPCIÓN: Borra TODAS las tablas para recrearlas desde cero.
-- !! DESTRUYE TODOS LOS DATOS. Usar solo en desarrollo/pruebas.
-- Flujo cuando cambias una tabla en el 01:  00 -> 01 -> 02
-- =======================================================

USE `tiusr20pl_Publica_SA`;

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `Bitacoras`;
DROP TABLE IF EXISTS `Tareas`;
DROP TABLE IF EXISTS `DesglosesSolicitud`;
DROP TABLE IF EXISTS `Solicitudes`;
DROP TABLE IF EXISTS `Representantes`;
DROP TABLE IF EXISTS `Usuarios`;
DROP TABLE IF EXISTS `EstadosSolicitud`;
DROP TABLE IF EXISTS `EstadosUsuario`;

SET FOREIGN_KEY_CHECKS = 1;

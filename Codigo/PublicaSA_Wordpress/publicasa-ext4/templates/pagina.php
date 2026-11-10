<?php
/** Plantilla institucional compartida visualmente con EXT2 y EXT3. */
defined('ABSPATH') || exit;
$url_listado = publicasa_ext4_listado_url();
?>
<!DOCTYPE html>
<html <?php language_attributes(); ?>>
<head>
    <meta charset="<?php bloginfo('charset'); ?>">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <?php wp_head(); ?>
</head>
<body <?php body_class('ps-portal'); ?>>
<?php wp_body_open(); ?>
<header class="ps-portal-barra">
    <nav class="ps-portal-contenedor ps-portal-nav" aria-label="Navegación del portal">
        <a class="ps-portal-marca" href="<?php echo esc_url($url_listado); ?>">
            <img src="<?php echo esc_url(plugins_url('assets/logo.png', dirname(__DIR__) . '/publicasa-ext4.php')); ?>" alt="" width="36" height="36">
            <span>Pública S.A.</span>
        </a>
        <span class="ps-portal-etiqueta">Módulo Externo de Proveedores</span>
        <a class="ps-portal-listado" href="<?php echo esc_url($url_listado); ?>">Solicitudes Publicadas</a>
    </nav>
</header>
<main class="ps-portal-contenedor ps-portal-principal" id="contenido">
    <?php while (have_posts()): the_post(); the_content(); endwhile; ?>
</main>
<footer class="ps-portal-pie">
    <div class="ps-portal-contenedor">
        <strong>Pública S.A. &copy; <?php echo esc_html(wp_date('Y')); ?></strong>
        <small>Sistema de Administración de Solicitudes &bull; Portal para Proveedores y Oferentes</small>
    </div>
</footer>
<?php wp_footer(); ?>
</body>
</html>

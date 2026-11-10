<?php
/**
 * Plantilla limpia para páginas del Módulo Externo de Pública S.A.
 * Estilos igualados al Sistema Interno (SB Admin 2 / Bootstrap custom palette).
 */
if (!defined('ABSPATH')) {
    exit;
}

$logo_url = plugins_url('assets/logo.png', dirname(__FILE__));
?>
<!DOCTYPE html>
<html <?php language_attributes(); ?>>
<head>
    <meta charset="<?php bloginfo('charset'); ?>">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?php wp_title('|', true, 'right'); ?>Pública S.A.</title>
    
    <!-- Bootstrap 5 y FontAwesome 6 (mismas librerías que el Sistema Interno) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" rel="stylesheet" />

    <?php wp_head(); ?>

    <style>
        :root {
            --publica-primary: #4e73df;
            --publica-hover: #2e59d9;
            --publica-success: #1cc88a;
            --publica-gray-dark: #5a5c69;
            --publica-bg: #f8f9fc;
        }

        body {
            background-color: var(--publica-bg) !important;
            font-family: system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            color: var(--publica-gray-dark);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            overflow-x: hidden;
        }

        main {
            flex: 1;
        }

        /* Barra de navegación superior idéntica a la paleta del Sistema Interno */
        .publicasa-navbar {
            background-color: var(--publica-primary) !important;
            min-height: 58px;
            box-shadow: 0 0.15rem 1.75rem 0 rgba(58, 59, 69, 0.15) !important;
        }

        .logo-img {
            height: 36px;
            width: 36px;
            object-fit: cover;
            border-radius: 50%;
            background-color: white;
            padding: 2px;
            display: block;
            border: 1px solid rgba(255, 255, 255, 0.4);
        }

        .publicasa-brand-text {
            font-size: 1.15rem;
            font-weight: 800;
            letter-spacing: 0.5px;
            color: #ffffff !important;
        }

        .publicasa-tag {
            font-size: 0.75rem;
            font-weight: 600;
            background-color: rgba(255, 255, 255, 0.2);
            color: #ffffff !important;
            border-radius: 0.35rem;
            padding: 0.3rem 0.6rem;
            letter-spacing: 0.3px;
        }

        /* Footer institucional idéntico al pie corporativo */
        .publicasa-footer {
            background-color: #ffffff;
            border-top: 1px solid #e3e6f0;
            color: #858796;
            font-size: 0.85rem;
        }

        /* Ocultar elementos sobrantes del tema de WordPress */
        .wp-block-post-title,
        header.wp-block-template-part,
        footer.wp-block-template-part,
        .entry-title {
            display: none !important;
        }
    </style>
</head>
<body <?php body_class('publicasa-theme-match'); ?>>

<!-- Topbar / Navbar Pública S.A. -->
<nav class="navbar navbar-expand-lg navbar-dark publicasa-navbar mb-4 sticky-top">
    <div class="container">
        <a class="navbar-brand d-flex align-items-center text-decoration-none py-1" href="<?php echo esc_url(remove_query_arg('id', get_permalink())); ?>">
            <img src="<?php echo esc_url($logo_url); ?>" alt="Logo de Pública S.A." class="logo-img me-2" />
            <span class="publicasa-brand-text">Pública S.A.</span>
            <span class="publicasa-tag ms-3 d-none d-sm-inline-block">
                <i class="fas fa-globe me-1"></i> Módulo Externo de Proveedores
            </span>
        </a>
        <div class="navbar-nav ms-auto">
            <a class="nav-link text-white fw-bold d-flex align-items-center" href="<?php echo esc_url(remove_query_arg('id', get_permalink())); ?>" title="Ver listado de solicitudes">
                <i class="fas fa-ticket-alt me-2"></i>
                <span>Solicitudes Publicadas</span>
            </a>
        </div>
    </div>
</nav>

<!-- Contenedor Principal -->
<main class="container mb-5">
    <?php
    while (have_posts()) :
        the_post();
        the_content();
    endwhile;
    ?>
</main>

<!-- Footer Pública S.A. -->
<footer class="publicasa-footer py-4 mt-auto">
    <div class="container text-center">
        <div class="fw-bold mb-1" style="color: #5a5c69;">
            Pública S.A. &copy; <?php echo date('Y'); ?>
        </div>
        <small>Sistema de Administración de Solicitudes &bull; Portal para Proveedores y Oferentes</small>
    </div>
</footer>

<?php wp_footer(); ?>
</body>
</html>

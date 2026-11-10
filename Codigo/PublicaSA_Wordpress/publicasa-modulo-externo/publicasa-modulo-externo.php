<?php
/**
 * Plugin Name: Pública S.A. - Módulo Externo
 * Description: Módulo externo para consulta de solicitudes, detalle y participaciones de proveedores (HU EXT2, EXT3 y EXT6), con diseño homologado al Sistema Interno.
 * Version: 1.2
 * Author: Aldo Burgos - Grupo Fuerzas Especiales Ginyu
 */

if (!defined('ABSPATH')) {
    exit;
}

// URL por defecto del servicio WCF.
if (!defined('PUBLICASA_WCF_URL')) {
    define('PUBLICASA_WCF_URL', 'http://localhost:53139/Service1.svc?wsdl');
}

// Cargar Bootstrap, FontAwesome y estilos del módulo.
add_action('wp_enqueue_scripts', 'publicasa_cargar_estilos');

function publicasa_cargar_estilos() {
    wp_enqueue_style(
        'bootstrap-css',
        'https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css',
        array(),
        '5.3.3'
    );

    wp_enqueue_style(
        'fontawesome-css',
        'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css',
        array(),
        '6.5.1'
    );

    $custom_css = "
        :root {
            --publica-primary: #4e73df;
            --publica-hover: #2e59d9;
            --publica-success: #1cc88a;
            --publica-bg: #f8f9fc;
            --publica-dark: #5a5c69;
        }

        body {
            background-color: var(--publica-bg) !important;
            color: var(--publica-dark);
            font-family: system-ui, -apple-system, 'Segoe UI',
                Roboto, 'Helvetica Neue', Arial, sans-serif;
        }

        .solicitudes-header-card {
            border-left: 4px solid var(--publica-primary) !important;
            background-color: #ffffff;
            border-radius: 0.35rem;
        }

        .solicitudes-header-card h1,
        .solicitudes-header-card h2 {
            color: var(--publica-primary);
        }

        .btn-accion {
            width: 34px;
            height: 34px;
            background-color: var(--publica-primary);
            border: none;
            color: white !important;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: background-color 0.15s ease-in-out;
            text-decoration: none;
        }

        .btn-accion:hover {
            background-color: var(--publica-hover);
            color: white !important;
        }

        .solicitudes-table thead {
            background-color: #343a40 !important;
            color: white !important;
        }

        .solicitudes-table th {
            font-weight: 600;
            font-size: 0.82rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            padding-top: 0.75rem;
            padding-bottom: 0.75rem;
            color: white !important;
        }

        .solicitudes-label {
            color: #5a5c69;
            font-size: 0.78rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            font-weight: 700;
            margin-bottom: 0.35rem;
            display: block;
        }

        .solicitudes-value-box {
            background-color: #f8f9fc;
            border: 1px solid #e3e6f0;
            border-radius: 0.35rem;
            padding: 0.55rem 0.85rem;
            min-height: 40px;
            color: #3a3b45;
        }

        .wp-block-post-title,
        .entry-title,
        header.wp-block-template-part,
        footer.wp-block-template-part {
            display: none !important;
        }
    ";

    wp_add_inline_style('bootstrap-css', $custom_css);
}

// Cargar la plantilla del módulo externo.
add_filter('template_include', 'publicasa_filtrar_plantilla', 99);

function publicasa_filtrar_plantilla($template) {
    if (is_singular() && is_page()) {
        global $post;

        if (
            $post &&
            (
                has_shortcode($post->post_content, 'publicasa_solicitudes') ||
                has_shortcode($post->post_content, 'publicasa_detalle') ||
                has_shortcode($post->post_content, 'publicasa_mis_participaciones')
            )
        ) {
            $custom_template = plugin_dir_path(__FILE__)
                . 'templates/publicasa-page.php';

            if (file_exists($custom_template)) {
                return $custom_template;
            }
        }
    }

    return $template;
}

// Mostrar un campo o indicar que está vacío.
function publicasa_mostrar_campo($valor) {
    if ($valor === null || trim((string) $valor) === '') {
        return '<em class="text-muted">&lt;No indicado&gt;</em>';
    }

    return esc_html((string) $valor);
}

// Formatear fechas.
function publicasa_mostrar_fecha($fecha) {
    if (empty($fecha) || strpos($fecha, '0001-01-01') !== false) {
        return '<em class="text-muted">&lt;No indicado&gt;</em>';
    }

    $timestamp = strtotime($fecha);

    if (!$timestamp) {
        return '<em class="text-muted">&lt;No indicado&gt;</em>';
    }

    return date('d/m/Y', $timestamp);
}

// ==========================================================================
// HU EXT2: listado de solicitudes publicadas.
// Shortcode: [publicasa_solicitudes]
// ==========================================================================

add_shortcode(
    'publicasa_solicitudes',
    'publicasa_render_solicitudes_publicadas'
);

function publicasa_render_solicitudes_publicadas() {
    if (isset($_GET['id']) && intval($_GET['id']) > 0) {
        return publicasa_render_detalle_solicitud();
    }

    ob_start();

    $wcf_url = get_option('publicasa_wcf_url', PUBLICASA_WCF_URL);
    $solicitudes = array();
    $error = null;

    try {
        if (!class_exists('SoapClient')) {
            throw new Exception(
                'La extensión SOAP de PHP no está habilitada en el servidor.'
            );
        }

        $client = new SoapClient(
            $wcf_url,
            array(
                'trace' => 1,
                'exceptions' => 1,
                'connection_timeout' => 8,
            )
        );

        $response = $client->ObtenerSolicitudesPublicadas();

        if (
            isset(
                $response->ObtenerSolicitudesPublicadasResult
                    ->SolicitudPublicadaDto
            )
        ) {
            $data = $response->ObtenerSolicitudesPublicadasResult
                ->SolicitudPublicadaDto;

            $solicitudes = is_array($data) ? $data : array($data);
        }
    } catch (Exception $e) {
        $error = 'No se pudo conectar con el servicio WCF: '
            . $e->getMessage();
    }
    ?>

    <div class="card border-0 shadow-sm mb-4 solicitudes-header-card">
        <div class="card-body py-3 d-flex justify-content-between align-items-center">
            <h1 class="h5 fw-bold mb-0" style="color: #4e73df;">
                <i class="fas fa-ticket-alt me-2"></i>
                Solicitudes Publicadas
            </h1>

            <span
                class="badge"
                style="background-color: #eaecf4; color: #4e73df; font-weight: 600; font-size: 0.85rem;"
            >
                <i class="fas fa-globe me-1"></i>
                Catálogo de Proveedores
            </span>
        </div>
    </div>

    <div class="card shadow-sm border-0 mb-4">
        <div class="card-header bg-white py-3 border-bottom d-flex justify-content-between align-items-center">
            <h6 class="m-0 fw-bold" style="color: #4e73df;">
                <i class="fas fa-table me-1"></i>
                Información de Solicitudes Disponibles
            </h6>

            <small class="text-muted">
                Mostrando solicitudes vigentes y autorizadas para consulta externa
            </small>
        </div>

        <div class="card-body p-0">
            <?php if ($error): ?>
                <div class="p-4">
                    <div
                        class="alert alert-warning border-start border-4 border-warning mb-0"
                        role="alert"
                    >
                        <strong>
                            <i class="fas fa-exclamation-triangle me-1"></i>
                            Nota de conexión:
                        </strong>

                        <?php echo esc_html($error); ?>

                        <div class="mt-2 text-muted small">
                            Asegúrate de que el proyecto
                            <strong>PublicaSA_WebService</strong>
                            esté ejecutándose en Visual Studio
                            (URL:
                            <code><?php echo esc_html($wcf_url); ?></code>).
                        </div>
                    </div>
                </div>
            <?php endif; ?>

            <?php if (empty($solicitudes) && !$error): ?>
                <div class="p-4">
                    <div class="alert alert-info border-start border-4 border-info mb-0">
                        <i class="fas fa-info-circle me-1"></i>
                        Actualmente no hay solicitudes publicadas vigentes para mostrar.
                    </div>
                </div>

            <?php elseif (!empty($solicitudes)): ?>
                <div class="table-responsive">
                    <table class="table table-striped table-hover align-middle mb-0 solicitudes-table">
                        <thead class="table-dark">
                            <tr>
                                <th scope="col" class="ps-4">
                                    Consecutivo de Oficio
                                </th>
                                <th scope="col">
                                    Título de la Solicitud
                                </th>
                                <th scope="col" class="pe-4">
                                    Representante
                                </th>
                            </tr>
                        </thead>

                        <tbody>
                            <?php foreach ($solicitudes as $sol): ?>
                                <tr>
                                    <td class="ps-4 fw-bold">
                                        <a
                                            href="<?php
                                            echo esc_url(
                                                add_query_arg(
                                                    'id',
                                                    $sol->IdSolicitud,
                                                    get_permalink()
                                                )
                                            );
                                            ?>"
                                            class="text-decoration-none"
                                            style="color: #4e73df;"
                                            title="Ver detalle de la solicitud"
                                        >
                                            <i class="fas fa-file-alt me-1 text-secondary"></i>
                                            <?php echo esc_html($sol->ConsecutivoOficio); ?>
                                        </a>
                                    </td>

                                    <td class="fw-semibold text-dark">
                                        <?php echo esc_html($sol->Titulo); ?>
                                    </td>

                                    <td class="pe-4">
                                        <?php
                                        echo !empty($sol->NombreRepresentante)
                                            ? esc_html($sol->NombreRepresentante)
                                            : '<em class="text-muted">&lt;No indicado&gt;</em>';
                                        ?>
                                    </td>
                                </tr>
                            <?php endforeach; ?>
                        </tbody>
                    </table>
                </div>
            <?php endif; ?>
        </div>
    </div>

    <?php
    return ob_get_clean();
}

// ==========================================================================
// HU EXT3: detalle de una solicitud.
// Shortcode: [publicasa_detalle]
// ==========================================================================

add_shortcode('publicasa_detalle', 'publicasa_render_detalle_solicitud');

function publicasa_render_detalle_solicitud() {
    ob_start();

    $id_solicitud = isset($_GET['id'])
        ? intval($_GET['id'])
        : 0;

    $url_regresar = remove_query_arg('id');

    if ($id_solicitud <= 0) {
        ?>
        <div class="card border-0 shadow-sm mb-4 solicitudes-header-card">
            <div class="card-body py-3">
                <div class="alert alert-secondary mb-0">
                    No se ha seleccionado ninguna solicitud para ver el detalle.

                    <a
                        href="<?php echo esc_url($url_regresar); ?>"
                        class="btn btn-sm text-white ms-2"
                        style="background-color: #4e73df;"
                    >
                        Volver al listado
                    </a>
                </div>
            </div>
        </div>
        <?php
        return ob_get_clean();
    }

    $wcf_url = get_option('publicasa_wcf_url', PUBLICASA_WCF_URL);
    $solicitud = null;
    $error = null;

    try {
        if (!class_exists('SoapClient')) {
            throw new Exception(
                'La extensión SOAP de PHP no está habilitada en el servidor.'
            );
        }

        $client = new SoapClient(
            $wcf_url,
            array(
                'trace' => 1,
                'exceptions' => 1,
                'connection_timeout' => 8,
            )
        );

        $response = $client->ObtenerDetalleSolicitud(
            array('idSolicitud' => $id_solicitud)
        );

        if (isset($response->ObtenerDetalleSolicitudResult)) {
            $solicitud = $response->ObtenerDetalleSolicitudResult;
        }
    } catch (Exception $e) {
        $error = 'Error al consultar el detalle de la solicitud: '
            . $e->getMessage();
    }

    $badge_class = 'bg-secondary';
    $estado_nombre = $solicitud ? $solicitud->NombreEstado : '';

    switch ($estado_nombre) {
        case 'Nueva':
            $badge_class = 'bg-primary';
            break;

        case 'Respondida':
            $badge_class = 'bg-success';
            break;

        case 'Iniciada':
            $badge_class = 'bg-info text-dark';
            break;

        case 'Finalizada':
            $badge_class = 'bg-secondary';
            break;

        case 'Vencida':
            $badge_class = 'bg-danger';
            break;
    }
    ?>

    <div class="card border-0 shadow-sm mb-4 solicitudes-header-card">
        <div class="card-body py-3 d-flex justify-content-between align-items-center">
            <div class="d-flex align-items-center gap-3">
                <a
                    href="<?php echo esc_url($url_regresar); ?>"
                    class="btn-accion"
                    title="Regresar al listado"
                >
                    <i class="fas fa-arrow-left"></i>
                </a>

                <h1 class="h5 fw-bold mb-0" style="color: #4e73df;">
                    Detalle de la Solicitud
                </h1>
            </div>

            <?php if ($solicitud): ?>
                <span
                    class="badge px-3 py-2"
                    style="background-color: #eaecf4; color: #4e73df; font-size: 0.88rem; font-weight: 700;"
                >
                    <i class="fas fa-file-alt me-1"></i>
                    <?php echo esc_html($solicitud->ConsecutivoOficio); ?>
                </span>
            <?php endif; ?>
        </div>
    </div>

    <div class="card shadow-sm border-0 mb-4">
        <div class="card-body p-4">
            <?php if ($error): ?>
                <div class="alert alert-danger" role="alert">
                    <i class="fas fa-times-circle me-1"></i>
                    <?php echo esc_html($error); ?>
                </div>

            <?php elseif (!$solicitud): ?>
                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle me-1"></i>
                    No se encontró información para la solicitud especificada.
                </div>

            <?php else: ?>
                <div class="mb-4 pb-3 border-bottom d-flex justify-content-between align-items-center">
                    <h5 class="fw-bold mb-0" style="color: #4e73df;">
                        <i class="fas fa-info-circle me-2"></i>
                        Información General de la Solicitud
                    </h5>

                    <span class="badge <?php echo esc_attr($badge_class); ?> px-3 py-2 fs-6">
                        Estado:
                        <?php echo publicasa_mostrar_campo($solicitud->NombreEstado); ?>
                    </span>
                </div>

                <div class="row g-3">
                    <div class="col-md-4">
                        <span class="solicitudes-label">
                            Consecutivo de Oficio
                        </span>

                        <div class="solicitudes-value-box fw-bold" style="color: #4e73df;">
                            <?php echo publicasa_mostrar_campo($solicitud->ConsecutivoOficio); ?>
                        </div>
                    </div>

                    <div class="col-md-8">
                        <span class="solicitudes-label">
                            Título de la Solicitud
                        </span>

                        <div class="solicitudes-value-box fw-semibold">
                            <?php echo publicasa_mostrar_campo($solicitud->Titulo); ?>
                        </div>
                    </div>

                    <div class="col-12">
                        <span class="solicitudes-label">
                            Descripción
                        </span>

                        <div class="solicitudes-value-box py-3" style="min-height: 70px;">
                            <?php echo publicasa_mostrar_campo($solicitud->Descripcion); ?>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <span class="solicitudes-label">
                            Documento de Inicio
                        </span>

                        <div class="solicitudes-value-box">
                            <i class="fas fa-file-contract text-secondary me-1"></i>
                            <?php echo publicasa_mostrar_campo($solicitud->DocumentoInicio); ?>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <span class="solicitudes-label">
                            Documento de Respuesta
                        </span>

                        <div class="solicitudes-value-box">
                            <i class="fas fa-reply text-secondary me-1"></i>
                            <?php echo publicasa_mostrar_campo($solicitud->DocumentoRespuesta); ?>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <span class="solicitudes-label">
                            Representante
                        </span>

                        <div class="solicitudes-value-box">
                            <i class="fas fa-user-tie text-secondary me-1"></i>
                            <?php echo publicasa_mostrar_campo($solicitud->NombreRepresentante); ?>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <span class="solicitudes-label">
                            Estado de la Solicitud
                        </span>

                        <div class="solicitudes-value-box">
                            <span class="badge <?php echo esc_attr($badge_class); ?>">
                                <?php echo publicasa_mostrar_campo($solicitud->NombreEstado); ?>
                            </span>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <span class="solicitudes-label">
                            Fecha de Ingreso
                        </span>

                        <div class="solicitudes-value-box">
                            <i class="far fa-calendar-alt text-secondary me-1"></i>
                            <?php echo publicasa_mostrar_fecha($solicitud->FechaIngreso); ?>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <span class="solicitudes-label">
                            Fecha de Inicio
                        </span>

                        <div class="solicitudes-value-box">
                            <i class="far fa-calendar-check text-secondary me-1"></i>
                            <?php echo publicasa_mostrar_fecha($solicitud->FechaInicio); ?>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <span class="solicitudes-label">
                            Fecha de Respuesta
                        </span>

                        <div class="solicitudes-value-box">
                            <i class="far fa-calendar-times text-secondary me-1"></i>
                            <?php echo publicasa_mostrar_fecha($solicitud->FechaRespuesta); ?>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <span class="solicitudes-label">
                            Vencimiento Publicación
                        </span>

                        <div class="solicitudes-value-box text-danger fw-semibold">
                            <i class="fas fa-hourglass-end me-1"></i>
                            <?php echo publicasa_mostrar_fecha($solicitud->FechaVencimientoPublicacion); ?>
                        </div>
                    </div>

                    <div class="col-12">
                        <span class="solicitudes-label">
                            Observaciones
                        </span>

                        <div class="solicitudes-value-box py-3" style="min-height: 60px;">
                            <?php echo publicasa_mostrar_campo($solicitud->Observaciones); ?>
                        </div>
                    </div>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-4 pt-3 border-top">
                    <a
                        href="<?php echo esc_url($url_regresar); ?>"
                        class="btn btn-secondary px-4 shadow-sm"
                    >
                        <i class="fas fa-arrow-left me-1"></i>
                        Regresar
                    </a>

                    <!-- Página 7: formulario de registro de propuestas EXT4. -->
                    <a
                        href="<?php
                        echo esc_url(
                            add_query_arg(
                                'idSolicitud',
                                (int) $solicitud->IdSolicitud,
                                get_permalink(7)
                            )
                        );
                        ?>"
                        class="btn text-white px-4 fw-bold shadow-sm"
                        style="background-color: #1cc88a; border-color: #1cc88a;"
                    >
                        <i class="fas fa-paper-plane me-1"></i>
                        Participar
                    </a>
                </div>
            <?php endif; ?>
        </div>
    </div>

    <?php
    return ob_get_clean();
}

// ==========================================================================
// HU EXT6: participaciones de un proveedor en solicitudes.
// Shortcode: [publicasa_mis_participaciones]
// ==========================================================================

add_shortcode(
    'publicasa_mis_participaciones',
    'publicasa_render_mis_participaciones'
);

// Busca la página del listado (EXT2) para enlazar al detalle (EXT3).
function publicasa_url_pagina_solicitudes() {
    $paginas = get_posts(array(
        'post_type' => 'page',
        'post_status' => 'publish',
        'posts_per_page' => 1,
        's' => '[publicasa_solicitudes',
        'fields' => 'ids',
    ));

    return !empty($paginas) ? get_permalink($paginas[0]) : '';
}

function publicasa_render_mis_participaciones() {
    ob_start();

    $cedula = isset($_GET['cedula'])
        ? sanitize_text_field(wp_unslash($_GET['cedula']))
        : '';
    $consultado = isset($_GET['cedula']);

    $resultado = null;
    $participaciones = array();
    $error = null;

    if ($consultado) {
        $wcf_url = get_option('publicasa_wcf_url', PUBLICASA_WCF_URL);

        try {
            if (!class_exists('SoapClient')) {
                throw new Exception(
                    'La extensión SOAP de PHP no está habilitada en el servidor.'
                );
            }

            $client = new SoapClient(
                $wcf_url,
                array(
                    'trace' => 1,
                    'exceptions' => 1,
                    'connection_timeout' => 8,
                )
            );

            $response = $client->ObtenerParticipacionesProveedor(
                array('cedulaJuridica' => $cedula)
            );

            if (isset($response->ObtenerParticipacionesProveedorResult)) {
                $resultado = $response->ObtenerParticipacionesProveedorResult;

                if (
                    isset(
                        $resultado->Participaciones
                            ->ParticipacionProveedorDto
                    )
                ) {
                    $data = $resultado->Participaciones
                        ->ParticipacionProveedorDto;

                    $participaciones = is_array($data) ? $data : array($data);
                }
            }
        } catch (Exception $e) {
            $error = 'No se pudo conectar con el servicio WCF: '
                . $e->getMessage();
        }
    }

    $url_solicitudes = publicasa_url_pagina_solicitudes();
    ?>

    <div class="card border-0 shadow-sm mb-4 solicitudes-header-card">
        <div class="card-body py-3 d-flex justify-content-between align-items-center">
            <h1 class="h5 fw-bold mb-0" style="color: #4e73df;">
                <i class="fas fa-handshake me-2"></i>
                Mis Participaciones
            </h1>

            <span
                class="badge"
                style="background-color: #eaecf4; color: #4e73df; font-weight: 600; font-size: 0.85rem;"
            >
                <i class="fas fa-globe me-1"></i>
                Portal de Proveedores
            </span>
        </div>
    </div>

    <div class="card shadow-sm border-0 mb-4">
        <div class="card-body p-4">
            <form method="get" action="<?php echo esc_url(get_permalink()); ?>" class="row g-3 align-items-end">
                <?php
                // Conserva el parámetro de página si los enlaces permanentes no están activos.
                if (isset($_GET['page_id'])):
                ?>
                    <input type="hidden" name="page_id" value="<?php echo esc_attr(intval($_GET['page_id'])); ?>" />
                <?php endif; ?>

                <div class="col-md-6">
                    <label for="publicasa-cedula" class="solicitudes-label">
                        Cédula jurídica del proveedor
                        <span class="text-danger">*</span>
                    </label>

                    <input
                        type="text"
                        id="publicasa-cedula"
                        name="cedula"
                        class="form-control"
                        value="<?php echo esc_attr($cedula); ?>"
                        placeholder="3-101-123456 o 3101123456"
                        maxlength="20"
                        required
                        pattern="(3-[0-9]{3}-[0-9]{6}|[0-9]{10})"
                        title="La cédula debe tener 10 dígitos o el formato 3-XXX-XXXXXX."
                    />
                </div>

                <div class="col-md-3">
                    <button type="submit" class="btn text-white px-4 fw-bold shadow-sm w-100" style="background-color: #4e73df;">
                        <i class="fas fa-search me-1"></i>
                        Consultar
                    </button>
                </div>
            </form>
        </div>
    </div>

    <?php
    // Mensajes al proveedor en el modal estándar (igual al del Sistema Interno).
    $modal_titulo = null;
    $modal_mensaje = null;

    if ($consultado && $error) {
        $modal_titulo = 'Advertencia';
        $modal_mensaje = $error;
    } elseif ($consultado && empty($participaciones)) {
        $modal_titulo = 'Información';
        $modal_mensaje = ($resultado && !empty($resultado->Mensaje))
            ? $resultado->Mensaje
            : 'Datos no encontrados.';
    }
    ?>

    <?php if ($consultado && !$modal_mensaje): ?>
        <div class="card shadow-sm border-0 mb-4">
            <div class="card-header bg-white py-3 border-bottom d-flex justify-content-between align-items-center">
                <h6 class="m-0 fw-bold" style="color: #4e73df;">
                    <i class="fas fa-table me-1"></i>
                    Solicitudes en las que ha participado
                </h6>

                <?php if ($resultado && !empty($resultado->NombreProveedor)): ?>
                    <small class="text-muted">
                        <i class="fas fa-building me-1"></i>
                        <?php echo esc_html($resultado->NombreProveedor); ?>
                    </small>
                <?php endif; ?>
            </div>

            <div class="card-body p-0">
                    <div class="table-responsive">
                        <table class="table table-striped table-hover align-middle mb-0 solicitudes-table">
                            <thead class="table-dark">
                                <tr>
                                    <th scope="col" class="ps-4">
                                        Consecutivo de Oficio
                                    </th>
                                    <th scope="col" class="pe-4">
                                        Título de la Solicitud
                                    </th>
                                </tr>
                            </thead>

                            <tbody>
                                <?php foreach ($participaciones as $part): ?>
                                    <tr>
                                        <td class="ps-4 fw-bold">
                                            <?php if ($url_solicitudes): ?>
                                                <a
                                                    href="<?php
                                                    echo esc_url(
                                                        add_query_arg(
                                                            'id',
                                                            (int) $part->IdSolicitud,
                                                            $url_solicitudes
                                                        )
                                                    );
                                                    ?>"
                                                    class="text-decoration-none"
                                                    style="color: #4e73df;"
                                                    title="Ver detalle de la solicitud"
                                                >
                                                    <i class="fas fa-file-alt me-1 text-secondary"></i>
                                                    <?php echo esc_html($part->ConsecutivoOficio); ?>
                                                </a>
                                            <?php else: ?>
                                                <?php echo esc_html($part->ConsecutivoOficio); ?>
                                            <?php endif; ?>
                                        </td>

                                        <td class="pe-4 fw-semibold text-dark">
                                            <?php echo esc_html($part->Titulo); ?>
                                        </td>
                                    </tr>
                                <?php endforeach; ?>
                            </tbody>
                        </table>
                    </div>
            </div>
        </div>
    <?php endif; ?>

    <?php if ($modal_mensaje): ?>
        <!-- Modal estándar de mensajes (mismo diseño que el Sistema Interno) -->
        <div class="modal fade" id="modalMensaje" tabindex="-1" aria-labelledby="modalMensajeLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="modalMensajeLabel"><?php echo esc_html($modal_titulo); ?></h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                    </div>
                    <div class="modal-body"><?php echo esc_html($modal_mensaje); ?></div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>
        <?php
        wp_enqueue_script(
            'publicasa-bootstrap-js',
            'https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js',
            array(),
            '5.3.3',
            true
        );
        wp_add_inline_script(
            'publicasa-bootstrap-js',
            "new bootstrap.Modal(document.getElementById('modalMensaje')).show();"
        );
        ?>
    <?php endif; ?>

    <?php
    return ob_get_clean();
}

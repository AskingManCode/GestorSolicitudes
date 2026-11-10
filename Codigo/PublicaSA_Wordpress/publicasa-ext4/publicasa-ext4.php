<?php
/**
 * Plugin Name: Pública S.A. — Registro de propuestas EXT4
 * Description: Formulario de participación conectado al servicio WCF.
 * Version: 0.3.0
 * Requires PHP: 7.4
 */

defined('ABSPATH') || exit;

// Carga los estilos en la cabecera de la página que contiene el formulario.
add_action('wp_enqueue_scripts', function () {
    global $post;
    if (is_page('registrar-propuesta') || ($post &&
        has_shortcode($post->post_content, 'publicasa_registrar_propuesta'))) {
        wp_enqueue_style('publicasa-ext4', plugins_url('assets/formulario.css', __FILE__), array(), '0.3.0');
        wp_enqueue_script('publicasa-ext4', plugins_url('assets/formulario.js', __FILE__), array(), '0.3.0', true);
    }
});

// Usa la misma estructura institucional en EXT4 y en la carga Excel EXT5.
add_filter('template_include', function ($template) {
    global $post;
    if (is_page() && $post &&
        has_shortcode($post->post_content, 'publicasa_registrar_propuesta')) {
        return __DIR__ . '/templates/pagina.php';
    }
    return $template;
}, 100);

// Resuelve el listado desde la ruta configurada o su shortcode, sin fijar un ID local.
function publicasa_ext4_listado_url() {
    $ruta = get_option('publicasa_ext4_detalle', '');
    $query = wp_parse_url($ruta, PHP_URL_QUERY);
    $args = array();
    if (is_string($query)) { parse_str($query, $args); }
    if (!empty($args['page_id']) && is_scalar($args['page_id'])) {
        $pagina = get_post((int) $args['page_id']);
        if ($pagina && $pagina->post_status === 'publish' &&
            has_shortcode($pagina->post_content, 'publicasa_solicitudes')) {
            return get_permalink($pagina->ID);
        }
    }
    foreach (get_pages(array('post_status' => 'publish')) as $pagina) {
        if (has_shortcode($pagina->post_content, 'publicasa_solicitudes')) {
            return get_permalink($pagina->ID);
        }
    }
    return home_url('/');
}

// EXT3 puede usar esta función para construir el enlace de Participar.
function publicasa_ext4_url($id_solicitud) {
    return add_query_arg('idSolicitud', (int) $id_solicitud,
        home_url('/registrar-propuesta/'));
}

function publicasa_ext4_detalle_url($id) {
    $ruta = get_option('publicasa_ext4_detalle', '/detalle-solicitud/?idSolicitud={id}');
    return home_url(str_replace('{id}', (string) $id, $ruta));
}

add_action('admin_menu', function () {
    add_options_page('Propuestas EXT4', 'Propuestas EXT4', 'manage_options',
        'publicasa-ext4', 'publicasa_ext4_ajustes');
});

add_action('admin_init', function () {
    register_setting('publicasa_ext4', 'publicasa_ext4_wsdl', array(
        'sanitize_callback' => 'esc_url_raw', 'default' => ''));
    register_setting('publicasa_ext4', 'publicasa_ext4_detalle', array(
        'sanitize_callback' => function ($valor) {
            // El regreso siempre apunta a una ruta del mismo WordPress.
            $valor = sanitize_text_field($valor);
            return preg_match('~^/(?!/)[^\\\\]*$~', $valor)
                ? $valor : '/detalle-solicitud/?idSolicitud={id}';
        }, 'default' => '/detalle-solicitud/?idSolicitud={id}'));
});

function publicasa_ext4_ajustes() {
    if (!current_user_can('manage_options')) { return; }
    ?>
    <div class="wrap"><h1>Propuestas EXT4</h1>
    <form method="post" action="options.php">
        <?php settings_fields('publicasa_ext4'); ?>
        <p><label for="publicasa-wsdl">Dirección WSDL del servicio</label><br>
        <input id="publicasa-wsdl" type="url" class="large-text" name="publicasa_ext4_wsdl"
            value="<?php echo esc_attr(get_option('publicasa_ext4_wsdl', '')); ?>"
            placeholder="http://localhost:53139/Service1.svc?wsdl" required></p>
        <p>Debe ser accesible desde el servidor donde funciona WordPress.</p>
        <p><label for="publicasa-detalle">Ruta del detalle EXT3</label><br>
        <input id="publicasa-detalle" class="large-text" name="publicasa_ext4_detalle"
            value="<?php echo esc_attr(get_option('publicasa_ext4_detalle', '/detalle-solicitud/?idSolicitud={id}')); ?>"></p>
        <p>Usá <code>{id}</code> donde corresponda el identificador de la solicitud.</p>
        <?php submit_button(); ?>
    </form></div>
    <?php
}

// Acepta decimales escritos con punto o coma, sin separadores de miles.
function publicasa_ext4_decimal($valor, $maximo) {
    if (!is_scalar($valor)) { return null; }
    $valor = str_replace(',', '.', trim((string) $valor));
    if (!preg_match('/\A[0-9]+(?:\.[0-9]{1,2})?\z/', $valor)) { return null; }
    $numero = (float) $valor;
    return is_finite($numero) && $numero <= $maximo ? $numero : null;
}

function publicasa_ext4_validar($datos) {
    $proveedor = isset($datos['Proveedor']) && is_array($datos['Proveedor'])
        ? $datos['Proveedor'] : array();
    if (!preg_match('/\A(?:3-[0-9]{3}-[0-9]{6}|[0-9]{10})\z/',
        $proveedor['CedulaJuridica'] ?? '')) {
        return 'Indicá una cédula jurídica de 10 dígitos o con formato 3-101-999999.';
    }
    if (trim($proveedor['Nombre'] ?? '') === '') { return 'Indicá el nombre del proveedor.'; }
    $filas = $datos['Desgloses'] ?? array();
    if (!is_array($filas) || count($filas) === 0) { return 'Agregá al menos un desglose.'; }
    $periodos = array();
    foreach ($filas as $indice => $fila) {
        $prefijo = 'Fila ' . ($indice + 1) . ': ';
        if (!is_array($fila)) { return $prefijo . 'El desglose es inválido.'; }
        $mes = filter_var($fila['Mes'] ?? '', FILTER_VALIDATE_INT);
        $anio = filter_var($fila['Anio'] ?? '', FILTER_VALIDATE_INT);
        if (!$mes || $mes < 1 || $mes > 12) { return $prefijo . 'El mes debe estar entre 1 y 12.'; }
        if (!$anio || $anio < 2000 || $anio > 2150) { return $prefijo . 'El año debe estar entre 2000 y 2150.'; }
        $periodo = $anio . '-' . $mes;
        if (isset($periodos[$periodo])) { return $prefijo . 'El mes y año están repetidos.'; }
        $periodos[$periodo] = true;
        foreach (array('Horas' => 99999999.99, 'Monto' => 999999999999.99,
            'Iva' => 999999999999.99, 'Total' => 999999999999.99, 'PorcentajeCobro' => 100) as $campo => $maximo) {
            if (publicasa_ext4_decimal($fila[$campo] ?? '', $maximo) === null) {
                return $prefijo . $campo . ' debe ser un número válido, positivo o cero, con hasta dos decimales.';
            }
        }
        $monto = (int) round(publicasa_ext4_decimal($fila['Monto'], 999999999999.99) * 100);
        $iva = (int) round($monto * 13 / 100, 0, PHP_ROUND_HALF_UP);
        if ((int) round(publicasa_ext4_decimal($fila['Iva'], 999999999999.99) * 100) !== $iva ||
            (int) round(publicasa_ext4_decimal($fila['Total'], 999999999999.99) * 100) !== $monto + $iva) {
            return $prefijo . 'Revisá el IVA y el total calculados.';
        }
        if (trim($fila['Observaciones'] ?? '') === '') { return $prefijo . 'Indicá las observaciones.'; }
    }
    return null;
}

function publicasa_ext4_enviar($datos, $id) {
    if (!class_exists('SoapClient') || !get_option('publicasa_ext4_wsdl')) {
        return array('Exitoso' => false, 'Tipo' => 'error', 'Mensaje' => 'El servicio no está disponible en este momento.');
    }
    try {
        // PHP llama a WCF; las credenciales de la BD permanecen en el servicio.
        $cliente = new SoapClient(get_option('publicasa_ext4_wsdl'), array(
            'exceptions' => true, 'trace' => false, 'connection_timeout' => 15,
            'stream_context' => stream_context_create(array('http' => array('timeout' => 30)))));
        $filas = array();
        foreach ($datos['Desgloses'] as $fila) {
            $detalle = array('Mes' => (int) $fila['Mes'], 'Anio' => (int) $fila['Anio'],
                'Observaciones' => $fila['Observaciones']);
            foreach (array('Horas', 'Monto', 'Iva', 'Total', 'PorcentajeCobro') as $campo) {
                $detalle[$campo] = str_replace(',', '.', $fila[$campo]);
            }
            $filas[] = $detalle;
        }
        $respuesta = $cliente->__soapCall('RegistrarPropuesta', array(array('registro' => array(
            'IdSolicitud' => $id, 'Proveedor' => $datos['Proveedor'],
            'Desgloses' => array('DesglosePropuesta' => $filas)))));
        $resultado = $respuesta->RegistrarPropuestaResult ?? null;
        if (!$resultado || !isset($resultado->Exitoso, $resultado->Mensaje)) {
            throw new RuntimeException('Respuesta de RegistrarPropuesta sin resultado.');
        }
        return (array) $resultado;
    } catch (Throwable $ex) {
        // Evita escribir la solicitud y los datos personales en el log de WordPress.
        error_log('PublicaSA EXT4: no fue posible completar la llamada al servicio.');
        return array('Exitoso' => false, 'Tipo' => 'error', 'Mensaje' => 'No se pudo confirmar el registro. Revisá si la propuesta quedó registrada antes de reenviarla.');
    }
}

add_shortcode('publicasa_registrar_propuesta', 'publicasa_ext4_pantalla');
function publicasa_ext4_pantalla() {
    $id_crudo = $_GET['idSolicitud'] ?? '';
    $id = is_scalar($id_crudo) ? filter_var($id_crudo, FILTER_VALIDATE_INT,
        array('options' => array('min_range' => 1, 'max_range' => 2147483647))) : false;
    if (!$id) {
        return '<p role="alert">Para registrar una propuesta, ingresá desde el botón Participar de una solicitud.</p>';
    }
    $datos = array('Proveedor' => array(), 'Desgloses' => array(array()));
    $resultado = null;
    if (($_SERVER['REQUEST_METHOD'] ?? '') === 'POST' && isset($_POST['publicasa_ext4_enviar'])) {
        $nonce = $_POST['publicasa_ext4_nonce'] ?? '';
        if (!is_string($nonce) || !wp_verify_nonce($nonce, 'publicasa_ext4_' . $id)) {
            $resultado = array('Exitoso' => false, 'Mensaje' => 'La sesión del formulario venció. Recargá la página e intentá nuevamente.');
        } else {
            // Solo se aceptan los campos que necesita el registro.
            $entrada = wp_unslash($_POST);
            foreach (array('CedulaJuridica', 'Nombre', 'NombreRepresentante', 'Telefono', 'Correo') as $campo) {
                $valor = $entrada['Proveedor'][$campo] ?? '';
                $datos['Proveedor'][$campo] = is_string($valor) ? sanitize_text_field($valor) : '';
            }
            $datos['Desgloses'] = array();
            if (isset($entrada['Desgloses']) && is_array($entrada['Desgloses'])) {
                foreach ($entrada['Desgloses'] as $fila) {
                    if (!is_array($fila)) { continue; }
                    $detalle = array();
                    foreach (array('Mes', 'Anio', 'Horas', 'Monto', 'Iva', 'Total', 'PorcentajeCobro', 'Observaciones') as $campo) {
                        $valor = $fila[$campo] ?? '';
                        $detalle[$campo] = is_string($valor) ? sanitize_textarea_field($valor) : '';
                    }
                    $datos['Desgloses'][] = $detalle;
                }
            }
            $error = publicasa_ext4_validar($datos);
            $resultado = $error ? array('Exitoso' => false, 'Mensaje' => $error)
                : publicasa_ext4_enviar($datos, $id);
        }
    }
    if (!$datos['Desgloses']) { $datos['Desgloses'] = array(array()); }
    $url_cancelar = publicasa_ext4_detalle_url($id);
    ob_start();
    include __DIR__ . '/templates/formulario.php';
    return ob_get_clean();
}

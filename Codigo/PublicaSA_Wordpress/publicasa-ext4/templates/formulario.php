<?php defined('ABSPATH') || exit; ?>
<section class="ps-ext4" aria-labelledby="ps-titulo">
    <p class="ps-eyebrow">PÚBLICA S.A. / PARTICIPACIÓN</p>
    <h1 id="ps-titulo">Registrar propuesta</h1>
    <p class="ps-intro">Solicitud n.º <?php echo esc_html($id); ?> · Completá los datos del proveedor y el desglose mensual de tu propuesta.</p>
    <?php
        $tipo_aviso = !empty($resultado['Exitoso']) ? 'exito'
            : (($resultado['Tipo'] ?? '') === 'error' ? 'error' : 'aviso');
        $titulo_aviso = array('exito' => 'Éxito', 'aviso' => 'Aviso', 'error' => 'Error');
    ?>
    <dialog class="ps-modal ps-modal-<?php echo esc_attr($tipo_aviso); ?>" aria-labelledby="ps-modal-titulo" aria-describedby="ps-modal-mensaje" data-modal <?php echo $resultado ? 'open data-inicial' : ''; ?>>
        <div class="ps-modal-cabecera">
            <h2 id="ps-modal-titulo" data-modal-titulo><?php echo esc_html($titulo_aviso[$tipo_aviso]); ?></h2>
            <button type="button" class="ps-modal-cerrar" data-cerrar-modal aria-label="Cerrar mensaje">×</button>
        </div>
        <div class="ps-modal-contenido">
            <p id="ps-modal-mensaje" data-modal-mensaje><?php echo esc_html($resultado['Mensaje'] ?? ''); ?></p>
            <?php if (!empty($resultado['Exitoso']) && !empty($resultado['IdPropuesta'])): ?>
                <p><strong>Propuesta n.º <?php echo esc_html($resultado['IdPropuesta']); ?>.</strong></p>
            <?php endif; ?>
        </div>
        <div class="ps-modal-pie"><button type="button" data-cerrar-modal autofocus>Aceptar</button></div>
    </dialog>
    <?php if (!empty($resultado['Exitoso'])): ?>
        <a class="ps-boton ps-secundario" href="<?php echo esc_url($url_cancelar); ?>">Volver a la solicitud</a>
    <?php else: ?>
    <form method="post" class="ps-formulario">
        <?php wp_nonce_field('publicasa_ext4_' . $id, 'publicasa_ext4_nonce'); ?>
        <section data-vista-excel hidden aria-labelledby="ps-excel-titulo" data-worker-url="<?php echo esc_url(plugins_url('assets/excel-worker.js', dirname(__DIR__) . '/publicasa-ext4.php')); ?>">
            <h2 id="ps-excel-titulo" tabindex="-1">Cargar propuesta desde Excel</h2>
            <p>Seleccioná un archivo .xlsx o .xls. La primera fila debe contener estos ocho encabezados:</p>
            <p class="ps-excel-columnas">Mes · Año · Horas · Monto · IVA · Total · Observaciones · Porcentaje de cobro</p>
            <p><a href="<?php echo esc_url(plugins_url('assets/plantilla-propuesta.xlsx', dirname(__DIR__) . '/publicasa-ext4.php')); ?>" download>Descargar plantilla con dos meses de ejemplo</a></p>
            <p class="ps-ayuda">Mes: número del 1 al 12. Montos en colones. IVA: 13% del monto. Total: monto más IVA. Todos los datos son requeridos. Hasta 100 desgloses y 5 MB por archivo.</p>
            <label>Archivo Excel<input type="file" data-archivo-excel accept=".xlsx,.xls"></label>
            <label class="ps-excel-hoja" data-selector-hoja hidden>Hoja del archivo<select data-hoja-excel></select></label>
            <p class="ps-ayuda" data-estado-excel role="status"></p>
            <p>Al cargar, se reemplazarán los desgloses actuales. Los datos del proveedor se conservarán. Podés revisar y modificar los desgloses antes de confirmar la propuesta.</p>
            <div class="ps-acciones">
                <button type="button" class="ps-secundario" data-volver-excel>Cancelar carga</button>
                <button type="button" data-importar-excel disabled>Cargar desgloses</button>
            </div>
        </section>
        <div data-vista-propuesta>
        <fieldset><legend>1. Datos del proveedor</legend>
            <div class="ps-grid">
            <?php foreach (array('CedulaJuridica' => 'Cédula jurídica', 'Nombre' => 'Nombre del proveedor') as $campo => $etiqueta): ?>
                <label><?php echo esc_html($etiqueta); ?> <span aria-hidden="true">*</span>
                <input name="Proveedor[<?php echo esc_attr($campo); ?>]" value="<?php echo esc_attr($datos['Proveedor'][$campo] ?? ''); ?>" required maxlength="<?php echo $campo === 'Nombre' ? '150' : '20'; ?>"
                    <?php if ($campo === 'CedulaJuridica'): ?>pattern="(3-[0-9]{3}-[0-9]{6}|[0-9]{10})" placeholder="3-101-999999" title="10 dígitos o formato 3-101-999999"<?php endif; ?>>
                </label>
            <?php endforeach; ?>
            </div>
            <p class="ps-ayuda">Si el proveedor aún no está registrado, completá también estos tres datos de contacto. El servicio verificará la cédula al confirmar.</p>
            <div class="ps-grid ps-tres">
            <?php foreach (array('NombreRepresentante' => array('Representante', 'text', 150), 'Telefono' => array('Teléfono', 'tel', 30), 'Correo' => array('Correo electrónico', 'email', 150)) as $campo => $opciones): ?>
                <label><?php echo esc_html($opciones[0]); ?>
                <input type="<?php echo esc_attr($opciones[1]); ?>" name="Proveedor[<?php echo esc_attr($campo); ?>]" maxlength="<?php echo esc_attr($opciones[2]); ?>" value="<?php echo esc_attr($datos['Proveedor'][$campo] ?? ''); ?>"></label>
            <?php endforeach; ?>
            </div>
        </fieldset>
        <fieldset><legend>2. Desglose de la propuesta</legend>
            <p class="ps-ayuda">Agregá una fila por mes. El IVA y el total se calculan con el monto indicado. Todos los datos del desglose son obligatorios.</p>
            <div class="ps-filas">
            <?php foreach ($datos['Desgloses'] as $indice => $fila): ?>
                <?php include __DIR__ . '/fila.php'; ?>
            <?php endforeach; ?>
            </div>
            <div class="ps-herramientas">
                <button type="button" class="ps-secundario" data-agregar>+ Agregar mes</button>
                <button type="button" class="ps-secundario" data-abrir-excel>Cargar Excel</button>
            </div>
            <p class="ps-ayuda">También podés completar los desgloses desde un archivo Excel y revisarlos antes de confirmar.</p>
        </fieldset>
        <div class="ps-acciones">
            <a class="ps-boton ps-secundario" href="<?php echo esc_url($url_cancelar); ?>">Cancelar</a>
            <button type="submit" name="publicasa_ext4_enviar" value="1">Confirmar propuesta</button>
        </div>
        </div>
    </form>
    <?php endif; ?>
</section>

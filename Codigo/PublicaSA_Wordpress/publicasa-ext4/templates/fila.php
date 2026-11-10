<?php defined('ABSPATH') || exit; ?>
<div class="ps-fila">
    <div class="ps-fila-cabecera"><h2>Desglose <span data-numero><?php echo esc_html($indice + 1); ?></span></h2>
        <button type="button" class="ps-quitar" data-quitar>Quitar mes</button>
    </div>
    <div class="ps-grid ps-cuatro">
        <label>Mes *<select name="Desgloses[<?php echo esc_attr($indice); ?>][Mes]" data-campo="Mes" required>
            <option value="">Seleccioná</option>
            <?php foreach (array(1 => 'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre') as $mes => $nombre): ?>
                <option value="<?php echo esc_attr($mes); ?>" <?php selected($fila['Mes'] ?? '', $mes); ?>><?php echo esc_html($nombre); ?></option>
            <?php endforeach; ?>
        </select></label>
        <?php foreach (array('Anio' => array('Año', 2000, 2150, '1'), 'Horas' => array('Horas', 0, 99999999.99, '0.01'), 'Monto' => array('Monto (₡)', 0, 999999999999.99, '0.01'), 'PorcentajeCobro' => array('Porcentaje de cobro (%)', 0, 100, '0.01'), 'Iva' => array('IVA · 13% (₡)', 0, 999999999999.99, '0.01'), 'Total' => array('Total (₡)', 0, 999999999999.99, '0.01')) as $campo => $opciones): ?>
            <label><?php echo esc_html($opciones[0]); ?> *
            <input type="number" name="Desgloses[<?php echo esc_attr($indice); ?>][<?php echo esc_attr($campo); ?>]" data-campo="<?php echo esc_attr($campo); ?>" value="<?php echo esc_attr(str_replace(',', '.', $fila[$campo] ?? '')); ?>" min="<?php echo esc_attr($opciones[1]); ?>" max="<?php echo esc_attr($opciones[2]); ?>" step="<?php echo esc_attr($opciones[3]); ?>" required <?php echo in_array($campo, array('Iva', 'Total'), true) ? 'readonly' : ''; ?>>
            </label>
        <?php endforeach; ?>
    </div>
    <label>Observaciones *<textarea name="Desgloses[<?php echo esc_attr($indice); ?>][Observaciones]" data-campo="Observaciones" rows="2" required><?php echo esc_textarea($fila['Observaciones'] ?? ''); ?></textarea></label>
</div>

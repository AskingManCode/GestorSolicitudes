/* Valida el archivo completo antes de cambiar cualquier dato del formulario. */
(() => {
    'use strict';
    const campos = ['Mes', 'Anio', 'Horas', 'Monto', 'Iva', 'Total', 'Observaciones', 'PorcentajeCobro'];
    const normalizar = valor => String(valor ?? '').normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '').toLowerCase().replace(/[^a-z0-9]/g, '');
    const nombres = ['mes', 'ano', 'horas', 'monto', 'iva', 'total', 'observaciones', 'porcentajedecobro'];
    const vacia = celda => !celda || (!celda.f && (celda.v == null || String(celda.v).trim() === ''));
    function leerHoja(hoja, XLSX) {
        if (!hoja || !hoja['!ref']) throw Error('La hoja seleccionada está vacía.');
        const rango = XLSX.utils.decode_range(hoja['!fullref'] || hoja['!ref']);
        if (rango.e.r > 1000 || rango.e.c > 63) {
            throw Error('La hoja es demasiado grande. Usá la plantilla y quitá las filas y columnas sobrantes.');
        }
        const columnas = {};
        for (let c = 0; c <= rango.e.c; c++) {
            const celda = hoja[XLSX.utils.encode_cell({r: 0, c})];
            if (vacia(celda)) continue;
            const nombre = normalizar(celda.v);
            let indice = nombres.indexOf(nombre);
            if (nombre === 'anio') indice = 1;
            if (nombre === 'porcentajecobro') indice = 7;
            if (indice < 0 || columnas[campos[indice]] !== undefined) {
                throw Error('La primera fila debe contener una sola vez: Mes, Año, Horas, Monto, IVA, Total, Observaciones y Porcentaje de cobro.');
            }
            columnas[campos[indice]] = c;
        }
        if (Object.keys(columnas).length !== 8) {
            throw Error('Faltan columnas. Usá los ocho encabezados de la plantilla en la primera fila.');
        }
        const filas = [], periodos = new Set();
        for (let r = 1; r <= rango.e.r; r++) {
            const celdas = Array.from({length: rango.e.c + 1}, (_, c) => hoja[XLSX.utils.encode_cell({r, c})]);
            if (celdas.every(vacia)) continue;
            const prefijo = `Fila ${r + 1} del Excel: `;
            const fallo = mensaje => { throw Error(prefijo + mensaje); };
            if (filas.length >= 100) throw Error('Podés cargar hasta 100 desgloses por archivo.');
            if (celdas.some((celda, c) => !vacia(celda) && !Object.values(columnas).includes(c))) {
                fallo('hay datos en una columna sin encabezado.');
            }
            const fila = {};
            for (const campo of campos) {
                const celda = celdas[columnas[campo]];
                if (vacia(celda)) fallo(`${campo === 'Anio' ? 'Año' : campo} es requerido.`);
                if (celda.t === 'e' || celda.t === 'b' || celda.t === 'd') fallo(`${campo} contiene un valor inválido.`);
                if (campo === 'Observaciones') {
                    if (celda.f) fallo('las observaciones deben ser texto, sin fórmulas.');
                    fila[campo] = String(celda.v).trim();
                    if (!fila[campo]) fallo('Observaciones es requerido.');
                    continue;
                }
                if (celda.f && (celda.t !== 'n' || typeof celda.v !== 'number')) {
                    fallo(`${campo}: abrí el Excel, recalculá las fórmulas y guardalo antes de cargarlo.`);
                }
                let valor = String(celda.v ?? '').trim().replace(',', '.');
                const esPorcentaje = campo === 'PorcentajeCobro' && String(celda.z || '').includes('%');
                if (esPorcentaje && typeof celda.v === 'number') {
                    valor = String(celda.v * 100);
                    // Evita residuos binarios propios de Excel, sin aceptar más de dos decimales.
                    const redondeado = Math.round(Number(valor) * 100) / 100;
                    if (Math.abs(Number(valor) - redondeado) < 1e-8) valor = String(redondeado);
                }
                if (!/^\d+(?:\.\d{1,2})?$/.test(valor) || !Number.isFinite(Number(valor))) {
                    fallo(`${campo} debe ser un número positivo o cero, con hasta dos decimales y sin separadores de miles.`);
                }
                const numero = Number(valor);
                const maximo = {Mes: 12, Anio: 2150, Horas: 99999999.99, Monto: 999999999999.99,
                    Iva: 999999999999.99, Total: 999999999999.99, PorcentajeCobro: 100}[campo];
                if (numero > maximo) fallo(`${campo} supera el valor máximo de ${maximo}.`);
                if (campo === 'Mes' && (!Number.isInteger(numero) || numero < 1)) fallo('el mes debe ser un entero entre 1 y 12.');
                if (campo === 'Anio' && (!Number.isInteger(numero) || numero < 2000)) fallo('el año debe ser un entero entre 2000 y 2150.');
                // Excel guarda 50% como 0.5. El formulario usa 50 sobre 100.
                fila[campo] = valor;
            }
            const centimos = valor => Math.round(Number(valor) * 100);
            const monto = centimos(fila.Monto), iva = Math.floor((monto * 13 + 50) / 100);
            if (centimos(fila.Iva) !== iva) fallo('el IVA debe corresponder al 13% del monto, redondeado a dos decimales.');
            if (centimos(fila.Total) !== monto + iva) fallo('el total debe ser igual al monto más el IVA.');
            const periodo = `${Number(fila.Anio)}-${Number(fila.Mes)}`;
            if (periodos.has(periodo)) fallo('el mes y año están repetidos.');
            periodos.add(periodo);
            filas.push(fila);
        }
        if (!filas.length) throw Error('Agregá al menos un desglose debajo de los encabezados.');
        return filas;
    }
    globalThis.PublicasaExcel = {leerHoja};
})();

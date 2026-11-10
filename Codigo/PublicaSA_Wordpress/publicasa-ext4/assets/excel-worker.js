/* El archivo se lee localmente sin bloquear el formulario. No se ejecutan macros. */
'use strict';
importScripts('vendor/xlsx.full.min.js', 'excel-validacion.js');
let libro;
self.onmessage = event => {
    try {
        if (event.data.accion === 'abrir') {
            const bytes = new Uint8Array(event.data.buffer);
            const zip = bytes[0] === 0x50 && bytes[1] === 0x4b;
            const xls = bytes[0] === 0xd0 && bytes[1] === 0xcf && bytes[2] === 0x11 && bytes[3] === 0xe0;
            if (!zip && !xls) throw Error('Formato inválido.');
            libro = XLSX.read(event.data.buffer, {type: 'array', cellFormula: true,
                cellNF: true, cellDates: false, sheetRows: 1002});
            const hojas = libro.SheetNames.filter((nombre, indice) =>
                !(libro.Workbook?.Sheets?.[indice]?.Hidden));
            if (!hojas.length) throw Error('El archivo no tiene hojas visibles.');
            self.postMessage({hojas});
        } else {
            self.postMessage({filas: PublicasaExcel.leerHoja(libro?.Sheets[event.data.hoja], XLSX)});
        }
    } catch (error) {
        const validacion = event.data.accion === 'cargar';
        self.postMessage({error: validacion ? error.message
            : 'No se pudo leer el Excel. Usá un archivo .xlsx o .xls válido, sin contraseña.'});
    }
};

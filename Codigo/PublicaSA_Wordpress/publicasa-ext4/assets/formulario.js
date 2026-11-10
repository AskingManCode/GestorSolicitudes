/* La pantalla mantiene las filas en memoria hasta confirmar la propuesta. */
(() => {
    'use strict';
    document.querySelectorAll('.ps-ext4').forEach(pantalla => {
        const modal = pantalla.querySelector('[data-modal]');
        let devolverFoco = null;

        function abrirModal(mensaje, tipo = 'aviso', destino = null) {
            devolverFoco = destino || document.activeElement;
            if (mensaje !== null) modal.querySelector('[data-modal-mensaje]').textContent = mensaje;
            modal.querySelector('[data-modal-titulo]').textContent = {
                exito: 'Éxito', aviso: 'Aviso', error: 'Error'
            }[tipo];
            modal.className = `ps-modal ps-modal-${tipo}`;
            // showModal mantiene el foco dentro del aviso y bloquea el fondo.
            if (!modal.open) modal.showModal();
        }

        modal.querySelectorAll('[data-cerrar-modal]').forEach(boton => {
            boton.addEventListener('click', () => modal.close());
        });
        modal.addEventListener('close', () => {
            if (devolverFoco && devolverFoco.isConnected) devolverFoco.focus();
        });
        if (modal.hasAttribute('data-inicial')) {
            const tipo = modal.classList.contains('ps-modal-exito') ? 'exito'
                : (modal.classList.contains('ps-modal-error') ? 'error' : 'aviso');
            devolverFoco = pantalla.querySelector('.ps-boton, [type="submit"]');
            modal.removeAttribute('open');
            abrirModal(null, tipo, devolverFoco);
        }

        const form = pantalla.querySelector('.ps-formulario');
        if (!form) return;
        // Presenta las validaciones en español dentro del mismo aviso.
        form.noValidate = true;
        const filas = form.querySelector('.ps-filas');
        const plantilla = filas.firstElementChild.cloneNode(true);
        const campo = (fila, nombre) => fila.querySelector(`[data-campo="${nombre}"]`);

        function renumerar() {
            [...filas.children].forEach((fila, indice) => {
                fila.querySelector('[data-numero]').textContent = indice + 1;
                fila.querySelector('[data-quitar]').disabled = filas.children.length === 1;
                fila.querySelectorAll('[data-campo]').forEach(input => {
                    input.name = `Desgloses[${indice}][${input.dataset.campo}]`;
                });
            });
        }

        function calcular(fila) {
            const entrada = campo(fila, 'Monto').value;
            if (!/^\d+(?:\.\d{1,2})?$/.test(entrada)) {
                campo(fila, 'Iva').value = '';
                campo(fila, 'Total').value = '';
                return;
            }
            // Calcula en céntimos y redondea el IVA a dos decimales.
            const [entero, decimal = ''] = entrada.split('.');
            const monto = Number(entero) * 100 + Number(decimal.padEnd(2, '0'));
            const iva = Math.floor((monto * 13 + 50) / 100);
            const formato = centimos => `${Math.floor(centimos / 100)}.${String(centimos % 100).padStart(2, '0')}`;
            campo(fila, 'Iva').value = formato(iva);
            campo(fila, 'Total').value = formato(monto + iva);
        }

        const vistaExcel = form.querySelector('[data-vista-excel]');
        const vistaPropuesta = form.querySelector('[data-vista-propuesta]');
        const archivoExcel = form.querySelector('[data-archivo-excel]');
        const hojaExcel = form.querySelector('[data-hoja-excel]');
        const importarExcel = form.querySelector('[data-importar-excel]');
        const estadoExcel = form.querySelector('[data-estado-excel]');
        let lector = null, cancelarLectura = null, generacion = 0;
        function detenerLectura() {
            generacion++;
            if (cancelarLectura) cancelarLectura();
            if (lector) lector.terminate();
            lector = null;
        }
        function pedirLectura(datos, transferibles = []) {
            return new Promise((resolve, reject) => {
                const finalizar = () => { clearTimeout(limite); cancelarLectura = null; };
                const limite = setTimeout(() => {
                    finalizar();
                    lector.terminate();
                    lector = null;
                    reject(Error('El archivo tardó demasiado en leerse. Usá un Excel más pequeño y volvé a seleccionarlo.'));
                }, 15000);
                cancelarLectura = () => { finalizar(); reject(Error('Carga cancelada.')); };
                lector.onmessage = event => {
                    finalizar();
                    if (event.data.error) reject(Error(event.data.error));
                    else resolve(event.data);
                };
                lector.onerror = () => {
                    finalizar();
                    reject(Error('No se pudo leer el Excel. Volvé a seleccionarlo o usá la plantilla.'));
                };
                lector.postMessage(datos, transferibles);
            });
        }
        form.querySelector('[data-abrir-excel]').addEventListener('click', () => {
            vistaPropuesta.hidden = true;
            vistaExcel.hidden = false;
            vistaExcel.querySelector('h2').focus();
        });
        function volverDeExcel() {
            detenerLectura();
            archivoExcel.value = '';
            importarExcel.disabled = true;
            form.querySelector('[data-selector-hoja]').hidden = true;
            estadoExcel.textContent = '';
            vistaExcel.hidden = true;
            vistaPropuesta.hidden = false;
            form.querySelector('[data-abrir-excel]').focus();
        }
        form.querySelector('[data-volver-excel]').addEventListener('click', volverDeExcel);
        archivoExcel.addEventListener('change', async () => {
            detenerLectura();
            const intento = generacion;
            importarExcel.disabled = true;
            hojaExcel.replaceChildren();
            form.querySelector('[data-selector-hoja]').hidden = true;
            estadoExcel.textContent = '';
            const archivo = archivoExcel.files[0];
            if (!archivo) return;
            if (!/\.(xlsx|xls)$/i.test(archivo.name) || !archivo.size || archivo.size > 5 * 1024 * 1024) {
                abrirModal('Seleccioná un archivo .xlsx o .xls de hasta 5 MB.', 'aviso', archivoExcel);
                return;
            }
            estadoExcel.textContent = 'Leyendo archivo…';
            try {
                const buffer = await archivo.arrayBuffer();
                if (intento !== generacion) return;
                lector = new Worker(vistaExcel.dataset.workerUrl);
                const respuesta = await pedirLectura({accion: 'abrir', buffer}, [buffer]);
                respuesta.hojas.forEach(nombre => {
                    const opcion = document.createElement('option');
                    opcion.value = nombre;
                    opcion.textContent = nombre;
                    hojaExcel.append(opcion);
                });
                form.querySelector('[data-selector-hoja]').hidden = false;
                importarExcel.disabled = false;
                estadoExcel.textContent = 'Archivo listo. Elegí la hoja y presioná Cargar desgloses.';
            } catch (error) {
                if (intento !== generacion) return;
                estadoExcel.textContent = '';
                abrirModal(error.message, 'aviso', archivoExcel);
            }
        });
        importarExcel.addEventListener('click', async () => {
            if (!lector) return;
            const intento = generacion;
            importarExcel.disabled = true;
            archivoExcel.disabled = true;
            hojaExcel.disabled = true;
            estadoExcel.textContent = 'Validando desgloses…';
            try {
                const respuesta = await pedirLectura({accion: 'cargar', hoja: hojaExcel.value});
                // Prepara todas las filas antes de reemplazar las existentes.
                const nuevas = respuesta.filas.map(datos => {
                    const fila = plantilla.cloneNode(true);
                    fila.querySelectorAll('[data-campo]').forEach(input => {
                        input.value = datos[input.dataset.campo];
                    });
                    calcular(fila);
                    return fila;
                });
                filas.replaceChildren(...nuevas);
                renumerar();
                volverDeExcel();
                abrirModal(`Se cargaron ${nuevas.length} desgloses del Excel. Revisalos y presioná Confirmar propuesta para enviarlos.`,
                    'exito', campo(filas.firstElementChild, 'Mes'));
            } catch (error) {
                if (intento !== generacion) return;
                estadoExcel.textContent = '';
                abrirModal(error.message, 'aviso', archivoExcel);
                importarExcel.disabled = !lector;
            } finally {
                archivoExcel.disabled = false;
                hojaExcel.disabled = false;
            }
        });

        filas.addEventListener('input', event => {
            if (event.target.dataset.campo === 'Monto') calcular(event.target.closest('.ps-fila'));
        });
        filas.addEventListener('click', event => {
            const quitar = event.target.closest('[data-quitar]');
            if (quitar && filas.children.length > 1) {
                quitar.closest('.ps-fila').remove();
                renumerar();
            }
        });
        form.querySelector('[data-agregar]').addEventListener('click', () => {
            const nueva = plantilla.cloneNode(true);
            nueva.querySelectorAll('[data-campo]').forEach(input => { input.value = ''; });
            filas.append(nueva);
            renumerar();
            campo(nueva, 'Mes').focus();
        });
        form.addEventListener('submit', event => {
            if (!vistaExcel.hidden) { event.preventDefault(); return; }
            const invalido = [...form.elements].find(input =>
                input.willValidate && !input.validity.valid);
            if (invalido) {
                event.preventDefault();
                const etiqueta = invalido.closest('label');
                const nombre = etiqueta ? etiqueta.firstChild.textContent.replace('*', '').trim() : 'Este campo';
                const fila = invalido.closest('.ps-fila');
                const prefijo = fila ? `Desglose ${fila.querySelector('[data-numero]').textContent}: ` : '';
                let mensaje = `Revisá el valor de ${nombre.toLowerCase()}.`;
                if (invalido.validity.valueMissing) mensaje = `Completá ${nombre.toLowerCase()}.`;
                else if (invalido.validity.badInput) mensaje = `${nombre} debe ser un número válido.`;
                else if (invalido.validity.patternMismatch) mensaje = 'Indicá una cédula jurídica de 10 dígitos o con formato 3-101-999999.';
                else if (invalido.validity.typeMismatch) mensaje = 'Indicá un correo electrónico válido.';
                else if (invalido.validity.rangeUnderflow || invalido.validity.rangeOverflow) mensaje = `${nombre} debe estar entre ${invalido.min} y ${invalido.max}.`;
                else if (invalido.validity.stepMismatch) mensaje = invalido.step === '1'
                    ? `${nombre} debe ser un número entero.` : `${nombre} admite hasta dos decimales.`;
                abrirModal(prefijo + mensaje, 'aviso', invalido);
                return;
            }
            const periodos = new Set();
            let mensaje = '';
            let destino = null;
            [...filas.children].forEach((fila, indice) => {
                const periodo = `${campo(fila, 'Anio').value}-${campo(fila, 'Mes').value}`;
                if (periodos.has(periodo)) {
                    mensaje = `Desglose ${indice + 1}: el mes y año están repetidos.`;
                    destino = campo(fila, 'Mes');
                }
                periodos.add(periodo);
                if (!campo(fila, 'Observaciones').value.trim()) {
                    mensaje = `Desglose ${indice + 1}: indicá las observaciones.`;
                    destino = campo(fila, 'Observaciones');
                }
            });
            if (mensaje) {
                event.preventDefault();
                abrirModal(mensaje, 'aviso', destino);
                return;
            }
            // Mantiene el nombre del botón en el POST y evita un segundo clic.
            if (form.dataset.enviando) { event.preventDefault(); return; }
            form.dataset.enviando = '1';
            const confirmar = form.querySelector('[type="submit"]');
            confirmar.textContent = 'Enviando propuesta…';
            confirmar.setAttribute('aria-disabled', 'true');
        });
        // Tanto las filas manuales como las importadas se confirman juntas.
        [...filas.children].forEach(calcular);
        renumerar();
    });
})();

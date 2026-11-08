// Configurar modales de Solicitudes
document.addEventListener('DOMContentLoaded', function () {
    // 1. Modal de Eliminación
    const modalEliminar = document.getElementById('modalEliminar');
    if (modalEliminar) {
        modalEliminar.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            const id = button.getAttribute('data-id');
            const consecutivo = button.getAttribute('data-consecutivo');

            const inputId = document.getElementById('inputEliminarId');
            if (inputId) inputId.value = id;

            const lbl = document.getElementById('lblConsecutivoEliminar');
            if (lbl) lbl.textContent = consecutivo;
        });
    }

    // 2. Modal de Publicación a Proveedores (EXT1)
    const modalPublicar = document.getElementById('modalPublicar');
    if (modalPublicar) {
        const chkVisible = document.getElementById('chkVisibleExterno');
        const inputFecha = document.getElementById('inputFechaVencimiento');
        const inputId = document.getElementById('inputPublicarId');
        const lblConsecutivo = document.getElementById('lblPublicarConsecutivo');
        const lblTitulo = document.getElementById('lblPublicarTitulo');
        const reqAsterisco = document.getElementById('reqAsterisco');

        function actualizarEstadoFecha() {
            if (!chkVisible || !inputFecha) return;

            if (chkVisible.checked) {
                inputFecha.disabled = false;
                inputFecha.required = true;
                if (reqAsterisco) reqAsterisco.style.display = 'inline';

                // Si está vacío, sugerir por defecto una fecha futura (+7 días)
                if (!inputFecha.value) {
                    const hoy = new Date();
                    hoy.setDate(hoy.getDate() + 7);
                    inputFecha.value = hoy.toISOString().split('T')[0];
                }
            } else {
                inputFecha.disabled = true;
                inputFecha.required = false;
                if (reqAsterisco) reqAsterisco.style.display = 'none';
            }
        }

        if (chkVisible) {
            chkVisible.addEventListener('change', actualizarEstadoFecha);
        }

        modalPublicar.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            const id = button.getAttribute('data-id');
            const consecutivo = button.getAttribute('data-consecutivo');
            const titulo = button.getAttribute('data-titulo');
            const visible = button.getAttribute('data-visible') === 'true';
            const vencimiento = button.getAttribute('data-vencimiento') || '';

            if (inputId) inputId.value = id;
            if (lblConsecutivo) lblConsecutivo.textContent = consecutivo;
            if (lblTitulo) lblTitulo.textContent = titulo;
            if (chkVisible) chkVisible.checked = visible;
            if (inputFecha) {
                inputFecha.value = vencimiento;
                // La fecha mínima permitida es hoy
                const hoyStr = new Date().toISOString().split('T')[0];
                inputFecha.min = hoyStr;
            }

            actualizarEstadoFecha();
        });
    }
});

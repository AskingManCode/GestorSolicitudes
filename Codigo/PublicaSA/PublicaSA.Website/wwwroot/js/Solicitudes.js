// Configurar modal de eliminación
document.addEventListener('DOMContentLoaded', function () {
    const modalEliminar = document.getElementById('modalEliminar');
    if (!modalEliminar) return;

    modalEliminar.addEventListener('show.bs.modal', function (event) {
        const button = event.relatedTarget;
        const id = button.getAttribute('data-id');
        const consecutivo = button.getAttribute('data-consecutivo');

        const inputId = document.getElementById('inputEliminarId');
        if (inputId) inputId.value = id;

        const lbl = document.getElementById('lblConsecutivoEliminar');
        if (lbl) lbl.textContent = consecutivo;
    });
});

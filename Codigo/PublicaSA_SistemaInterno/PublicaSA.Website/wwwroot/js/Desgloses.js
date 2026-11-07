// Configurar modal de eliminación
document.addEventListener('DOMContentLoaded', function () {
    const modalEliminar = document.getElementById('modalEliminar');
    if (!modalEliminar) return;

    modalEliminar.addEventListener('show.bs.modal', function (event) {
        const button = event.relatedTarget;
        const id = button.getAttribute('data-id');
        const detalle = button.getAttribute('data-detalle');

        const inputId = document.getElementById('inputEliminarDesgloseId');
        if (inputId) inputId.value = id;

        const lbl = document.getElementById('lblDesgloseDetalle');
        if (lbl) lbl.textContent = detalle;
    });
});

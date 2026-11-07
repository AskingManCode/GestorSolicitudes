document.addEventListener('DOMContentLoaded', function () {
    const modalEliminar = document.getElementById('modalConfirmarEliminar');
    if (modalEliminar) {
        modalEliminar.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            document.getElementById('inputEliminarId').value = button.getAttribute('data-id');
            document.getElementById('lblNombreEliminar').textContent = button.getAttribute('data-nombre');
        });
    }

    const modalError = document.getElementById('modalMensajeError');
    if (modalError) {
        new bootstrap.Modal(modalError).show();
    }
});
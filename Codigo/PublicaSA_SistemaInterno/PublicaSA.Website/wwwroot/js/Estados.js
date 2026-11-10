// HU SOL4: pasa el estado seleccionado al diálogo de confirmación de eliminación.
document.addEventListener('DOMContentLoaded', function () {
    const modalEliminar = document.getElementById('modalConfirmarEliminar');
    if (modalEliminar) {
        modalEliminar.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            document.getElementById('inputEliminarCodigo').value = button.getAttribute('data-codigo');
            document.getElementById('lblNombreEliminar').textContent = button.getAttribute('data-nombre');
        });
    }
});

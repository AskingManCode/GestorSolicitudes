// Mostrar / ocultar contraseña
document.getElementById('btnTogglePassword')?.addEventListener('click', function () {
    const input = document.getElementById('Contrasena');
    const icon = document.getElementById('iconTogglePassword');

    if (!input || !icon) return;

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.replace('fa-eye', 'fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.replace('fa-eye-slash', 'fa-eye');
    }
});

// Función reutilizable para mostrar mensajes en modal
function mostrarMensaje(titulo, mensaje, tipo = 'info') {
    const modalEl = document.getElementById('modalMensaje');
    if (!modalEl) return;

    const header = document.getElementById('modalMensajeHeader');
    const title = document.getElementById('modalMensajeLabel');
    const body = document.getElementById('modalMensajeBody');

    title.textContent = titulo;
    // usamos innerHTML para permitir saltos de línea (<br/>) en mensajes de validación
    body.innerHTML = mensaje;

    header.classList.remove('bg-success', 'bg-danger', 'bg-warning', 'text-white');
    
    if (tipo === 'success') header.classList.add('bg-success', 'text-white');
    else if (tipo === 'error') header.classList.add('bg-danger', 'text-white');
    else if (tipo === 'warning') header.classList.add('bg-warning');

    new bootstrap.Modal(modalEl).show();
}

// Mensajes de validación en español
document.addEventListener('DOMContentLoaded', function () {
    if (typeof $ !== 'undefined' && $.validator) {
        $.extend($.validator.messages, {
            required: "Este campo es requerido.",
            email: "Ingrese un correo válido.",
            minlength: $.validator.format("Ingrese al menos {0} caracteres."),
            maxlength: $.validator.format("No puede superar los {0} caracteres.")
        });
    }
});
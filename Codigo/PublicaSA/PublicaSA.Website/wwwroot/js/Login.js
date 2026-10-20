// Toggle mostrar/ocultar contraseña (mismo patrón que Usuarios)
document.getElementById('btnTogglePassword')?.addEventListener('click', function() {
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
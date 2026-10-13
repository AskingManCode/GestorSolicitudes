using FluentValidation;
using PublicaSA.Entities;

namespace PublicaSA.Entities
{
    public class UsuarioRequestValidator : AbstractValidator<UsuarioRequest>
    {
        public UsuarioRequestValidator()
        {
            RuleFor(x => x.NombreCompleto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre completo es requerido.")
                .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ ]+$").WithMessage("El nombre completo solo puede contener letras y espacios.")
                .Must(x => x == x.Trim()).WithMessage("El nombre completo no puede iniciar ni terminar con espacios.");

            RuleFor(x => x.Correo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El correo electrónico es requerido.")
                .MaximumLength(150).WithMessage("El correo electrónico no puede superar los 150 caracteres.")
                .Matches(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$").WithMessage("El formato del correo electrónico no es válido.");

            RuleFor(x => x.NombreUsuario)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.")
                .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("El nombre de usuario solo puede contener letras, números, puntos, guiones y guiones bajos.");

            RuleFor(x => x.Contrasena)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede superar los 100 caracteres.")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\W_]").WithMessage("La contraseña debe contener al menos un carácter especial.");

            RuleFor(x => x.CodigoEstado)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El estado es requerido.")
                .Must(e => e == "ACTIVO" || e == "INACTIVO" || e == "BLOQUEADO")
                .WithMessage("El estado debe ser Activo, Inactivo o Bloqueado.");
        }
    }
}
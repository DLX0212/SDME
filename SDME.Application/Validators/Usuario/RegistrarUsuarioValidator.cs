using FluentValidation;
using SDME.Application.DTOs.Usuario;
using System.Text.RegularExpressions;

namespace SDME.Application.Validators.Usuario
{
    public class RegistrarUsuarioValidator : AbstractValidator<RegistrarUsuarioDto>
    {
        public RegistrarUsuarioValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es requerido")
                .MaximumLength(100).WithMessage("El apellido no puede tener más de 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El formato del email no es válido")
                .MaximumLength(200).WithMessage("El email no puede tener más de 200 caracteres");

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Must(ValidarTelefonoRD).WithMessage("El teléfono debe ser válido (809/829/849 + 7 dígitos)");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");

            RuleFor(x => x.ConfirmarPassword)
                .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");
        }

        private bool ValidarTelefonoRD(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return false;

            var limpio = Regex.Replace(telefono, @"[\s\-\(\)]", "");
            var patron = @"^(809|829|849)\d{7}$";
            return Regex.IsMatch(limpio, patron);
        }
    }
}

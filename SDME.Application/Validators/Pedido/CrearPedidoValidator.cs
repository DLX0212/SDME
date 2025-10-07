using FluentValidation;
using SDME.Application.DTOs.Pedido;

namespace SDME.Application.Validators.Pedido
{
    public class CrearPedidoValidator : AbstractValidator<DTOs.Pedido.CrearPedidoDto>
    {
        public CrearPedidoValidator()
        {
            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("El usuario es requerido");

            RuleFor(x => x.TipoEntrega)
                .NotEmpty().WithMessage("El tipo de entrega es requerido")
                .Must(tipo => tipo == "Domicilio" || tipo == "Recoger")
                .WithMessage("El tipo de entrega debe ser 'Domicilio' o 'Recoger'");

            RuleFor(x => x.DireccionEntregaId)
                .GreaterThan(0).WithMessage("Debe seleccionar una dirección de entrega")
                .When(x => x.TipoEntrega == "Domicilio");

            RuleFor(x => x.Detalles)
                .NotEmpty().WithMessage("El pedido debe tener al menos un producto");

            RuleForEach(x => x.Detalles).ChildRules(detalle =>
            {
                detalle.RuleFor(x => x.ProductoId)
                    .GreaterThan(0).WithMessage("El producto es requerido");

                detalle.RuleFor(x => x.Cantidad)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");
            });
        }
    }
}
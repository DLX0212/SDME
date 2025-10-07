using FluentValidation;
using SDME.Application.DTOs.Producto;

namespace SDME.Application.Validators.Producto
{
    public class CrearProductoValidator : AbstractValidator<DTOs.Producto.CrearProductoDto>
    {
        public CrearProductoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del producto es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede tener más de 200 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede tener más de 1000 caracteres");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");
        }
    }
}
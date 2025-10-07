using Microsoft.Extensions.Logging;
using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Common;
using SDME.Application.Interfaces;
using SDME.Domain.Entities.Core;
using SDME.Domain.Interfaces;

namespace SDME.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(IUnitOfWork unitOfWork, ILogger<CategoriaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<CategoriaDto>>> ObtenerTodasAsync()
        {
            try
            {
                var categorias = await _unitOfWork.Categorias.GetConProductosAsync();
                var categoriasDto = categorias.Select(ConvertirADto).ToList();

                return ResponseDto<List<CategoriaDto>>.Success(categoriasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return ResponseDto<List<CategoriaDto>>.Failure("Error al obtener las categorías");
            }
        }

        public async Task<ResponseDto<CategoriaDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);

                if (categoria == null)
                {
                    return ResponseDto<CategoriaDto>.Failure("Categoría no encontrada");
                }

                return ResponseDto<CategoriaDto>.Success(ConvertirADto(categoria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría {Id}", id);
                return ResponseDto<CategoriaDto>.Failure("Error al obtener la categoría");
            }
        }

        public async Task<ResponseDto<CategoriaDto>> CrearAsync(CrearCategoriaDto dto)
        {
            try
            {
                _logger.LogInformation("Creando categoría: {Nombre}", dto.Nombre);

                var categoria = new Categoria
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    ImagenUrl = dto.ImagenUrl,
                    Orden = dto.Orden,
                    CreadoPor = "Admin"
                };

                await _unitOfWork.Categorias.AddAsync(categoria);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Categoría creada exitosamente con ID: {Id}", categoria.Id);

                return ResponseDto<CategoriaDto>.Success(
                    ConvertirADto(categoria),
                    "Categoría creada exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return ResponseDto<CategoriaDto>.Failure("Error al crear la categoría");
            }
        }

        private CategoriaDto ConvertirADto(Categoria categoria)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                ImagenUrl = categoria.ImagenUrl,
                Orden = categoria.Orden,
                CantidadProductos = categoria.Productos?.Count ?? 0,
                FechaCreacion = categoria.FechaCreacion,
                EstaActivo = categoria.EstaActivo
            };
        }
    }
}

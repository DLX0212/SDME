using Microsoft.Extensions.Logging;
using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Producto;
using SDME.Application.Interfaces;
using SDME.Domain.Entities.Core;
using SDME.Domain.Interfaces;

namespace SDME.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(IUnitOfWork unitOfWork, ILogger<ProductoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerTodosAsync()
        {
            try
            {
                var productos = await _unitOfWork.Productos.GetAllAsync();
                var productosDto = productos.Select(ConvertirADto).ToList();

                return ResponseDto<List<ProductoDto>>.Success(productosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return ResponseDto<List<ProductoDto>>.Failure("Error al obtener los productos");
            }
        }

        public async Task<ResponseDto<ProductoDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var producto = await _unitOfWork.Productos.GetByIdAsync(id);

                if (producto == null)
                {
                    return ResponseDto<ProductoDto>.Failure("Producto no encontrado");
                }

                return ResponseDto<ProductoDto>.Success(ConvertirADto(producto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return ResponseDto<ProductoDto>.Failure("Error al obtener el producto");
            }
        }

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            try
            {
                var productos = await _unitOfWork.Productos.GetByCategoriaAsync(categoriaId);
                var productosDto = productos.Select(ConvertirADto).ToList();

                return ResponseDto<List<ProductoDto>>.Success(productosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por categoría {Id}", categoriaId);
                return ResponseDto<List<ProductoDto>>.Failure("Error al obtener los productos");
            }
        }

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerDisponiblesAsync()
        {
            try
            {
                var productos = await _unitOfWork.Productos.GetDisponiblesAsync();
                var productosDto = productos.Select(ConvertirADto).ToList();

                return ResponseDto<List<ProductoDto>>.Success(productosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos disponibles");
                return ResponseDto<List<ProductoDto>>.Failure("Error al obtener los productos disponibles");
            }
        }

        public async Task<ResponseDto<ProductoDto>> CrearAsync(CrearProductoDto dto)
        {
            try
            {
                _logger.LogInformation("Creando producto: {Nombre}", dto.Nombre);

                // Validación de negocio: verificar que la categoría existe
                var categoriaExiste = await _unitOfWork.Categorias.ExistsAsync(dto.CategoriaId);
                if (!categoriaExiste)
                {
                    return ResponseDto<ProductoDto>.Failure("La categoría seleccionada no existe");
                }

                var producto = new Producto
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Precio = dto.Precio,
                    ImagenUrl = dto.ImagenUrl,
                    CategoriaId = dto.CategoriaId,
                    Stock = dto.Stock,
                    Disponible = true,
                    CreadoPor = "Admin" // En producción obtener del usuario autenticado
                };

                await _unitOfWork.Productos.AddAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Producto creado exitosamente con ID: {Id}", producto.Id);

                return ResponseDto<ProductoDto>.Success(ConvertirADto(producto), "Producto creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return ResponseDto<ProductoDto>.Failure("Error al crear el producto");
            }
        }

        public async Task<ResponseDto<ProductoDto>> ActualizarAsync(int id, ActualizarProductoDto dto)
        {
            try
            {
                var producto = await _unitOfWork.Productos.GetByIdAsync(id);

                if (producto == null)
                {
                    return ResponseDto<ProductoDto>.Failure("Producto no encontrado");
                }

                producto.Nombre = dto.Nombre;
                producto.Descripcion = dto.Descripcion;
                producto.Precio = dto.Precio;
                producto.ImagenUrl = dto.ImagenUrl;
                producto.Stock = dto.Stock;
                producto.Disponible = dto.Disponible;
                producto.ModificadoPor = "Admin";

                await _unitOfWork.Productos.UpdateAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Producto actualizado: {Id}", id);

                return ResponseDto<ProductoDto>.Success(ConvertirADto(producto), "Producto actualizado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                return ResponseDto<ProductoDto>.Failure("Error al actualizar el producto");
            }
        }

        public async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            try
            {
                var producto = await _unitOfWork.Productos.GetByIdAsync(id);

                if (producto == null)
                {
                    return ResponseDto<bool>.Failure("Producto no encontrado");
                }

                // Eliminación lógica
                producto.EstaActivo = false;
                await _unitOfWork.Productos.UpdateAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Producto eliminado (lógicamente): {Id}", id);

                return ResponseDto<bool>.Success(true, "Producto eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return ResponseDto<bool>.Failure("Error al eliminar el producto");
            }
        }

        public async Task<ResponseDto<List<ProductoDto>>> BuscarAsync(string termino)
        {
            try
            {
                var productos = await _unitOfWork.Productos.BuscarAsync(termino);
                var productosDto = productos.Select(ConvertirADto).ToList();

                return ResponseDto<List<ProductoDto>>.Success(productosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con término: {Termino}", termino);
                return ResponseDto<List<ProductoDto>>.Failure("Error al buscar productos");
            }
        }

        private ProductoDto ConvertirADto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                ImagenUrl = producto.ImagenUrl,
                Stock = producto.Stock,
                Disponible = producto.Disponible,
                CategoriaNombre = producto.Categoria?.Nombre ?? "Sin categoría",
                FechaCreacion = producto.FechaCreacion,
                EstaActivo = producto.EstaActivo
            };
        }
    }
}

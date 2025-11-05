using Microsoft.Extensions.Logging;
using Moq;
using SDME.Application.DTOs.Categoria;
using SDME.Application.Services;
using SDME.Domain.Entities.Core;
using SDME.Domain.Interfaces;
using SDME.Domain.Interfaces.Core;
using Xunit;

namespace SDME.Application.Test.Services
{
    public class CategoriaServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
        private readonly Mock<ILogger<CategoriaService>> _loggerMock;
        private readonly CategoriaService _service;

        public CategoriaServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoriaRepoMock = new Mock<ICategoriaRepository>();
            _loggerMock = new Mock<ILogger<CategoriaService>>();

            _unitOfWorkMock.Setup(u => u.Categorias).Returns(_categoriaRepoMock.Object);
            _service = new CategoriaService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_CreatesCategoria_Successfully()
        {
            // Arrange
            var dto = new CrearCategoriaDto
            {
                Nombre = "Empanadas",
                Descripcion = "Empanadas variadas",
                Orden = 1
            };

            _categoriaRepoMock.Setup(r => r.AddAsync(It.IsAny<Categoria>()))
                .ReturnsAsync((Categoria c) => c);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _service.CrearAsync(dto);

            // Assert
            Assert.True(resultado.Exito);
            Assert.NotNull(resultado.Data);
            Assert.Equal("Empanadas", resultado.Data.Nombre);
        }

        [Fact]
        public async Task ObtenerTodasAsync_ReturnsAllCategorias()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria
                {
                    
                    Nombre = "Empanadas",
                    Orden = 1,
                    CreadoPor = "system",
                    Productos = new List<Producto>()
                },
                new Categoria
                {
                    
                    Nombre = "Bebidas",
                    Orden = 2,
                    CreadoPor = "system",
                    Productos = new List<Producto>()
                }
            };

            _categoriaRepoMock.Setup(r => r.GetConProductosAsync())
                .ReturnsAsync(categorias);

            // Act
            var resultado = await _service.ObtenerTodasAsync();

            // Assert
            Assert.True(resultado.Exito);
            Assert.Equal(2, resultado.Data.Count);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_ReturnsCategoria_WhenExists()
        {
            // Arrange
            var categoriaId = 1;
            var categoria = new Categoria
            {
                
                Nombre = "Empanadas",
                Descripcion = "Empanadas variadas",
                Orden = 1,
                CreadoPor = "system"
            };

            _categoriaRepoMock.Setup(r => r.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _service.ObtenerPorIdAsync(categoriaId);

            // Assert
            Assert.True(resultado.Exito);
            Assert.NotNull(resultado.Data);
            Assert.Equal("Empanadas", resultado.Data.Nombre);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_Fails_WhenCategoriaNotFound()
        {
            // Arrange
            var categoriaId = 999;

            _categoriaRepoMock.Setup(r => r.GetByIdAsync(categoriaId))
                .ReturnsAsync((Categoria?)null);

            // Act
            var resultado = await _service.ObtenerPorIdAsync(categoriaId);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Equal("Categoría no encontrada", resultado.Mensaje);
        }
    }
}

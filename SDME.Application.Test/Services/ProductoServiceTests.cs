using Microsoft.Extensions.Logging;
using Moq;
using SDME.Application.DTOs.Producto;
using SDME.Application.Services;
using SDME.Domain.Entities.Core;
using SDME.Domain.Interfaces;
using SDME.Domain.Interfaces.Core;
using Xunit;

namespace SDME.Application.Test.Services
{ 
 public class ProductoServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductoRepository> _productoRepoMock;
    private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
    private readonly Mock<ILogger<ProductoService>> _loggerMock;
    private readonly ProductoService _service;

    public ProductoServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productoRepoMock = new Mock<IProductoRepository>();
        _categoriaRepoMock = new Mock<ICategoriaRepository>();
        _loggerMock = new Mock<ILogger<ProductoService>>();

        _unitOfWorkMock.Setup(u => u.Productos).Returns(_productoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Categorias).Returns(_categoriaRepoMock.Object);
        _service = new ProductoService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CrearAsync_CreatesProduct_WhenCategoriaExists()
    {
        // Arrange
        var dto = new CrearProductoDto
        {
            Nombre = "Empanada de Pollo",
            Descripcion = "Deliciosa empanada",
            Precio = 50.00m,
            CategoriaId = 1,
            Stock = 100
        };

        _categoriaRepoMock.Setup(r => r.ExistsAsync(dto.CategoriaId))
            .ReturnsAsync(true);

        _productoRepoMock.Setup(r => r.AddAsync(It.IsAny<Producto>()))
            .ReturnsAsync((Producto p) => p);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CrearAsync(dto);

        // Assert
        Assert.True(resultado.Exito);
        Assert.NotNull(resultado.Data);
        Assert.Equal("Empanada de Pollo", resultado.Data.Nombre);
        Assert.Equal(50.00m, resultado.Data.Precio);
    }

    [Fact]
    public async Task CrearAsync_Fails_WhenCategoriaDoesNotExist()
    {
        // Arrange
        var dto = new CrearProductoDto
        {
            Nombre = "Empanada de Pollo",
            Precio = 50.00m,
            CategoriaId = 999,
            Stock = 100
        };

        _categoriaRepoMock.Setup(r => r.ExistsAsync(dto.CategoriaId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.CrearAsync(dto);

        // Assert
        Assert.False(resultado.Exito);
        Assert.Equal("La categoría seleccionada no existe", resultado.Mensaje);
    }

    [Fact]
    public async Task ObtenerDisponiblesAsync_ReturnsOnlyAvailableProducts()
    {
        // Arrange
        var productos = new List<Producto>
            {
                new Producto
                {
                    
                    Nombre = "Producto 1",
                    Precio = 50m,
                    Stock = 10,
                    Disponible = true,
                    CategoriaId = 1,
                    CreadoPor = "system",
                    Categoria = new Categoria { Nombre = "Test" }
                }
            };

        _productoRepoMock.Setup(r => r.GetDisponiblesAsync())
            .ReturnsAsync(productos);

        // Act
        var resultado = await _service.ObtenerDisponiblesAsync();

        // Assert
        Assert.True(resultado.Exito);
        Assert.Single(resultado.Data);
        Assert.True(resultado.Data.First().Disponible);
    }

    [Fact]
    public async Task ActualizarAsync_UpdatesProduct_WhenProductExists()
    {
        // Arrange
        var productoId = 1;
        var dto = new ActualizarProductoDto
        {
            Nombre = "Empanada Actualizada",
            Descripcion = "Nueva descripción",
            Precio = 60.00m,
            Stock = 150,
            Disponible = true
        };

        var producto = new Producto
        {
            
            Nombre = "Empanada Original",
            Precio = 50m,
            CategoriaId = 1,
            CreadoPor = "system",
            Categoria = new Categoria { Nombre = "Test" }
        };

        _productoRepoMock.Setup(r => r.GetByIdAsync(productoId))
            .ReturnsAsync(producto);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.ActualizarAsync(productoId, dto);

        // Assert
        Assert.True(resultado.Exito);
        Assert.NotNull(resultado.Data);
        Assert.Equal("Empanada Actualizada", resultado.Data.Nombre);
        Assert.Equal(60.00m, resultado.Data.Precio);
    }

    [Fact]
    public async Task BuscarAsync_FindsProductsBySearchTerm()
    {
        // Arrange
        var termino = "pollo";
        var productos = new List<Producto>
            {
                new Producto
                {
                    
                    Nombre = "Empanada de Pollo",
                    Precio = 50m,
                    CategoriaId = 1,
                    CreadoPor = "system",
                    Categoria = new Categoria { Nombre = "Empanadas" }
                }
            };

        _productoRepoMock.Setup(r => r.BuscarAsync(termino))
            .ReturnsAsync(productos);

        // Act
        var resultado = await _service.BuscarAsync(termino);

        // Assert
        Assert.True(resultado.Exito);
        Assert.Single(resultado.Data);
        Assert.Contains("Pollo", resultado.Data.First().Nombre);
    }

    [Fact]
    public async Task EliminarAsync_SoftDeletesProduct_WhenProductExists()
    {
        // Arrange
        var productoId = 1;
        var producto = new Producto
        {
            
            Nombre = "Test",
            Precio = 50m,
            CategoriaId = 1,
            CreadoPor = "system",
            EstaActivo = true
        };

        _productoRepoMock.Setup(r => r.GetByIdAsync(productoId))
            .ReturnsAsync(producto);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.EliminarAsync(productoId);

        // Assert
        Assert.True(resultado.Exito);
        Assert.True(resultado.Data);
        Assert.False(producto.EstaActivo);
    }
}
}



using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Persistence.Context;
using SDME.Persistence.Repositories.Core;
using Xunit;

namespace SDME.Persistence.Test.Repositories
{
    public class ProductoRepositoryTests
    {
        private readonly SDMEDbContext _context;
        private readonly ProductoRepository _repository;
        private readonly CategoriaRepository _categoriaRepository;

        public ProductoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SDMEDbContext>()
                .UseInMemoryDatabase($"ProductoDB_{Guid.NewGuid()}")
                .Options;
            _context = new SDMEDbContext(options);
            _repository = new ProductoRepository(_context);
            _categoriaRepository = new CategoriaRepository(_context);
        }

        private async Task<Categoria> CrearCategoriaAsync()
        {
            var categoria = new Categoria
            {
                Nombre = "Empanadas",
                Descripcion = "Empanadas variadas",
                Orden = 1,
                CreadoPor = "system"
            };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        [Fact]
        public async Task GetByCategoriaAsync_ReturnsCorrectProducts()
        {
            // Arrange
            var categoria = await CrearCategoriaAsync();
            var producto = new Producto
            {
                Nombre = "Empanada de Pollo",
                Descripcion = "Deliciosa empanada",
                Precio = 50.00m,
                CategoriaId = categoria.Id,
                Stock = 100,
                Disponible = true,
                CreadoPor = "system"
            };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            var productos = await _repository.GetByCategoriaAsync(categoria.Id);

            // Assert
            Assert.Single(productos);
            Assert.Equal("Empanada de Pollo", productos.First().Nombre);
        }

        [Fact]
        public async Task GetDisponiblesAsync_ReturnsOnlyAvailableProducts()
        {
            // Arrange
            var categoria = await CrearCategoriaAsync();
            var disponible = new Producto
            {
                Nombre = "Disponible",
                Precio = 50m,
                CategoriaId = categoria.Id,
                Stock = 10,
                Disponible = true,
                CreadoPor = "system"
            };
            var noDisponible = new Producto
            {
                Nombre = "No Disponible",
                Precio = 50m,
                CategoriaId = categoria.Id,
                Stock = 0,
                Disponible = false,
                CreadoPor = "system"
            };
            await _context.Productos.AddRangeAsync(disponible, noDisponible);
            await _context.SaveChangesAsync();

            // Act
            var productos = await _repository.GetDisponiblesAsync();

            // Assert
            Assert.Single(productos);
            Assert.True(productos.First().Disponible);
            Assert.True(productos.First().Stock > 0);
        }

        [Fact]
        public async Task BuscarAsync_FindsProductsByName()
        {
            // Arrange
            var categoria = await CrearCategoriaAsync();
            var producto = new Producto
            {
                Nombre = "Empanada Especial",
                Descripcion = "Muy rica",
                Precio = 75m,
                CategoriaId = categoria.Id,
                Stock = 50,
                Disponible = true,
                CreadoPor = "system"
            };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            var productos = await _repository.BuscarAsync("Especial");

            // Assert
            Assert.Single(productos);
            Assert.Contains("Especial", productos.First().Nombre);
        }

        [Fact]
        public async Task TieneStockAsync_ReturnsTrue_WhenStockIsAvailable()
        {
            // Arrange
            var categoria = await CrearCategoriaAsync();
            var producto = new Producto
            {
                Nombre = "Producto Test",
                Precio = 50m,
                CategoriaId = categoria.Id,
                Stock = 100,
                Disponible = true,
                CreadoPor = "system"
            };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            var tieneStock = await _repository.TieneStockAsync(producto.Id, 50);

            // Assert
            Assert.True(tieneStock);
        }

        [Fact]
        public async Task TieneStockAsync_ReturnsFalse_WhenStockIsInsufficient()
        {
            // Arrange
            var categoria = await CrearCategoriaAsync();
            var producto = new Producto
            {
                Nombre = "Producto Test",
                Precio = 50m,
                CategoriaId = categoria.Id,
                Stock = 10,
                Disponible = true,
                CreadoPor = "system"
            };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            var tieneStock = await _repository.TieneStockAsync(producto.Id, 50);

            // Assert
            Assert.False(tieneStock);
        }
    }
}

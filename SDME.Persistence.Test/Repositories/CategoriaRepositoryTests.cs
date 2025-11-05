using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Persistence.Context;
using SDME.Persistence.Repositories.Core;
using Xunit;

namespace SDME.Persistence.Test.Repositories
{
    public class CategoriaRepositoryTests
    {
        private readonly SDMEDbContext _context;
        private readonly CategoriaRepository _repository;

        public CategoriaRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SDMEDbContext>()
                .UseInMemoryDatabase($"CategoriaDB_{Guid.NewGuid()}")
                .Options;
            _context = new SDMEDbContext(options);
            _repository = new CategoriaRepository(_context);
        }

        [Fact]
        public async Task GetConProductosAsync_ReturnsCategoriasWithProducts()
        {
            // Arrange
            var categoria = new Categoria
            {
                Nombre = "Empanadas",
                Descripcion = "Empanadas variadas",
                Orden = 1,
                CreadoPor = "system"
            };
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var producto = new Producto
            {
                Nombre = "Empanada de Pollo",
                Precio = 50m,
                CategoriaId = categoria.Id,
                Stock = 10,
                Disponible = true,
                CreadoPor = "system"
            };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            var categorias = await _repository.GetConProductosAsync();

            // Assert
            Assert.Single(categorias);
            Assert.Single(categorias.First().Productos);
        }

        [Fact]
        public async Task GetByNombreAsync_ReturnsCorrectCategoria()
        {
            // Arrange
            var categoria = new Categoria
            {
                Nombre = "Bebidas",
                Descripcion = "Bebidas refrescantes",
                Orden = 2,
                CreadoPor = "system"
            };
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNombreAsync("Bebidas");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bebidas", result.Nombre);
        }

        [Fact]
        public async Task GetConProductosAsync_OrdersByOrden()
        {
            // Arrange
            var categoria1 = new Categoria
            {
                Nombre = "Tercera",
                Orden = 3,
                CreadoPor = "system"
            };
            var categoria2 = new Categoria
            {
                Nombre = "Primera",
                Orden = 1,
                CreadoPor = "system"
            };
            var categoria3 = new Categoria
            {
                Nombre = "Segunda",
                Orden = 2,
                CreadoPor = "system"
            };
            await _context.Categorias.AddRangeAsync(categoria1, categoria2, categoria3);
            await _context.SaveChangesAsync();

            // Act
            var categorias = await _repository.GetConProductosAsync();

            // Assert
            Assert.Equal(3, categorias.Count());
            Assert.Equal("Primera", categorias.First().Nombre);
            Assert.Equal("Tercera", categorias.Last().Nombre);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Persistence.Context;
using SDME.Persistence.Repositories.Core;
using Xunit;

namespace SDME.Persistence.Test.Repositories
{
    public class UsuarioRepositoryTests
    {
        private readonly SDMEDbContext _context;
        private readonly UsuarioRepository _repository;

        public UsuarioRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SDMEDbContext>()
                .UseInMemoryDatabase($"UsuarioDB_{Guid.NewGuid()}")
                .Options;
            _context = new SDMEDbContext(options);
            _repository = new UsuarioRepository(_context);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsCorrectUser()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash123",
                TipoUsuario = TipoUsuario.Cliente,
                CreadoPor = "system"
            };
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("juan@test.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("juan@test.com", result.Email);
            Assert.Equal("Juan", result.Nombre);
        }

        [Fact]
        public async Task ExisteEmailAsync_ReturnsTrue_WhenEmailExists()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "María",
                Apellido = "González",
                Email = "maria@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash456",
                TipoUsuario = TipoUsuario.Cliente,
                CreadoPor = "system"
            };
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            // Act
            var existe = await _repository.ExisteEmailAsync("maria@test.com");

            // Assert
            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteEmailAsync_ReturnsFalse_WhenEmailDoesNotExist()
        {
            // Act
            var existe = await _repository.ExisteEmailAsync("noexiste@test.com");

            // Assert
            Assert.False(existe);
        }

        [Fact]
        public async Task GetClientesAsync_ReturnsOnlyClientes()
        {
            // Arrange
            var cliente = new Usuario
            {
                Nombre = "Cliente",
                Apellido = "Test",
                Email = "cliente@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash",
                TipoUsuario = TipoUsuario.Cliente,
                CreadoPor = "system"
            };
            var admin = new Usuario
            {
                Nombre = "Admin",
                Apellido = "Test",
                Email = "admin@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash",
                TipoUsuario = TipoUsuario.Administrador,
                CreadoPor = "system"
            };
            await _context.Usuarios.AddRangeAsync(cliente, admin);
            await _context.SaveChangesAsync();

            // Act
            var clientes = await _repository.GetClientesAsync();

            // Assert
            Assert.Single(clientes);
            Assert.All(clientes, c => Assert.Equal(TipoUsuario.Cliente, c.TipoUsuario));
        }

        [Fact]
        public async Task AddAsync_AddsNewUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Nuevo",
                Apellido = "Usuario",
                Email = "nuevo@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash",
                TipoUsuario = TipoUsuario.Cliente,
                CreadoPor = "system"
            };

            // Act
            await _repository.AddAsync(usuario);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.Usuarios.CountAsync();
            Assert.Equal(1, count);
        }
    }
}

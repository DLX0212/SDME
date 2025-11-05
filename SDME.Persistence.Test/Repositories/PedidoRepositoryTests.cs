using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Persistence.Context;
using SDME.Persistence.Repositories.Core;
using Xunit;

namespace SDME.Persistence.Test.Repositories
{
    public class PedidoRepositoryTests
    {
        private readonly SDMEDbContext _context;
        private readonly PedidoRepository _repository;

        public PedidoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SDMEDbContext>()
                .UseInMemoryDatabase($"PedidoDB_{Guid.NewGuid()}")
                .Options;
            _context = new SDMEDbContext(options);
            _repository = new PedidoRepository(_context);
        }

        private async Task<Usuario> CrearUsuarioAsync()
        {
            var usuario = new Usuario
            {
                Nombre = "Test",
                Apellido = "User",
                Email = "test@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash",
                TipoUsuario = TipoUsuario.Cliente,
                CreadoPor = "system"
            };
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        [Fact]
        public async Task GetByUsuarioAsync_ReturnsUserOrders()
        {
            // Arrange
            var usuario = await CrearUsuarioAsync();
            var pedido = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-001",
                Estado = EstadoPedido.Recibido,
                TipoEntrega = TipoEntrega.Domicilio,
                Subtotal = 100m,
                Impuesto = 18m,
                Total = 118m,
                CreadoPor = "system"
            };
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();

            // Act
            var pedidos = await _repository.GetByUsuarioAsync(usuario.Id);

            // Assert
            Assert.Single(pedidos);
            Assert.Equal(usuario.Id, pedidos.First().UsuarioId);
        }

        [Fact]
        public async Task GetByEstadoAsync_ReturnsOrdersWithSpecificStatus()
        {
            // Arrange
            var usuario = await CrearUsuarioAsync();
            var pedidoRecibido = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-001",
                Estado = EstadoPedido.Recibido,
                TipoEntrega = TipoEntrega.Domicilio,
                Subtotal = 100m,
                Impuesto = 18m,
                Total = 118m,
                CreadoPor = "system"
            };
            var pedidoEntregado = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-002",
                Estado = EstadoPedido.Entregado,
                TipoEntrega = TipoEntrega.Domicilio,
                Subtotal = 100m,
                Impuesto = 18m,
                Total = 118m,
                CreadoPor = "system"
            };
            await _context.Pedidos.AddRangeAsync(pedidoRecibido, pedidoEntregado);
            await _context.SaveChangesAsync();

            // Act
            var pedidos = await _repository.GetByEstadoAsync(EstadoPedido.Recibido);

            // Assert
            Assert.Single(pedidos);
            Assert.All(pedidos, p => Assert.Equal(EstadoPedido.Recibido, p.Estado));
        }

        [Fact]
        public async Task GetPedidosDelDiaAsync_ReturnsTodayOrders()
        {
            // Arrange
            var usuario = await CrearUsuarioAsync();
            var pedidoHoy = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-HOY",
                FechaPedido = DateTime.UtcNow,
                Estado = EstadoPedido.Recibido,
                TipoEntrega = TipoEntrega.Domicilio,
                Subtotal = 100m,
                Impuesto = 18m,
                Total = 118m,
                CreadoPor = "system"
            };
            var pedidoAyer = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-AYER",
                FechaPedido = DateTime.UtcNow.AddDays(-1),
                Estado = EstadoPedido.Recibido,
                TipoEntrega = TipoEntrega.Domicilio,
                Subtotal = 100m,
                Impuesto = 18m,
                Total = 118m,
                CreadoPor = "system"
            };
            await _context.Pedidos.AddRangeAsync(pedidoHoy, pedidoAyer);
            await _context.SaveChangesAsync();

            // Act
            var pedidos = await _repository.GetPedidosDelDiaAsync();

            // Assert
            Assert.Single(pedidos);
            Assert.Equal(DateTime.UtcNow.Date, pedidos.First().FechaPedido.Date);
        }

        [Fact]
        public async Task GetVentasTotalesAsync_CalculatesTotalSales()
        {
            // Arrange
            var usuario = await CrearUsuarioAsync();
            var pedido1 = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-001",
                FechaPedido = DateTime.UtcNow,
                Estado = EstadoPedido.Entregado,
                TipoEntrega = TipoEntrega.Domicilio,
                Total = 100m,
                CreadoPor = "system"
            };
            var pedido2 = new Pedido
            {
                UsuarioId = usuario.Id,
                NumeroPedido = "PED-002",
                FechaPedido = DateTime.UtcNow,
                Estado = EstadoPedido.Entregado,
                TipoEntrega = TipoEntrega.Domicilio,
                Total = 150m,
                CreadoPor = "system"
            };
            await _context.Pedidos.AddRangeAsync(pedido1, pedido2);
            await _context.SaveChangesAsync();

            // Act
            var ventasTotales = await _repository.GetVentasTotalesAsync(
                DateTime.UtcNow.Date,
                DateTime.UtcNow.Date.AddDays(1)
            );

            // Assert
            Assert.Equal(250m, ventasTotales);
        }
    }
}

using Moq;
using SDME.Application.DTOs.Pedido;
using SDME.Application.Logging;
using SDME.Application.Services;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces;
using SDME.Domain.Interfaces.Core;
using Xunit;

namespace SDME.Application.Test.Services
{
    public class PedidoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPedidoRepository> _pedidoRepoMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IProductoRepository> _productoRepoMock;
        private readonly Mock<IAppLogger<PedidoService>> _loggerMock;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _pedidoRepoMock = new Mock<IPedidoRepository>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _productoRepoMock = new Mock<IProductoRepository>();
            _loggerMock = new Mock<IAppLogger<PedidoService>>();

            _unitOfWorkMock.Setup(u => u.Pedidos).Returns(_pedidoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Productos).Returns(_productoRepoMock.Object);

            _service = new PedidoService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        #region CrearPedidoAsync Tests

        [Fact]
        public async Task CrearPedidoAsync_CreatesPedido_WhenDataIsValid()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Domicilio",
                DireccionEntregaId = 1,
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto
                    {
                        ProductoId = 1,
                        Cantidad = 2
                    }
                }
            };

            var usuario = new Usuario
            {
                
                Nombre = "Test",
                Apellido = "User",
                Email = "test@test.com",
                Telefono = "8091234567",
                PasswordHash = "hash",
                TipoUsuario = TipoUsuario.Cliente,
                EstaActivo = true,
                CreadoPor = "system"
            };

            var producto = new Producto
            {
                
                Nombre = "Empanada",
                Precio = 50m,
                Stock = 100,
                Disponible = true,
                EstaActivo = true,
                CategoriaId = 1,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(producto);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            _pedidoRepoMock.Setup(r => r.GetConDetallesAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Pedido
                {
                    
                    UsuarioId = usuario.Id,
                    Usuario = usuario,
                    NumeroPedido = $"PED-2025-{id:D6}",
                    Estado = EstadoPedido.Recibido,
                    TipoEntrega = TipoEntrega.Domicilio,
                    Subtotal = 100m,
                    Impuesto = 18m,
                    Total = 118m,
                    DetallesPedido = new List<DetallePedido>
                    {
                        new DetallePedido
                        {
                            ProductoId = 1,
                            Producto = producto,
                            Cantidad = 2,
                            PrecioUnitario = 50m,
                            Subtotal = 100m
                        }
                    }
                });

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.True(resultado.Exito);
            Assert.NotNull(resultado.Data);
            Assert.Equal(118m, resultado.Data.Total);
            Assert.Equal("Domicilio", resultado.Data.TipoEntrega);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenUsuarioNotFound()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 999,
                TipoEntrega = "Domicilio",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Equal("Usuario no encontrado", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenUsuarioIsNotActive()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Domicilio",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
               
                Email = "test@test.com",
                EstaActivo = false,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Equal("El usuario no está activo", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenPedidoIsEmpty()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Domicilio",
                Detalles = new List<CrearDetallePedidoDto>()
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Equal("El pedido debe contener al menos un producto", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenTipoEntregaIsInvalid()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "TipoInvalido",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
               
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("Tipo de entrega inválido", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenDomicilioWithoutDireccion()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Domicilio",
                DireccionEntregaId = null,
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("dirección", resultado.Mensaje.ToLower());
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenProductNotFound()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Recoger",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 999, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
               
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Producto?)null);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("no encontrado", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenProductIsNotActive()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Recoger",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            var producto = new Producto
            {
                
                Nombre = "Empanada",
                Precio = 50m,
                Stock = 100,
                Disponible = true,
                EstaActivo = false,
                CategoriaId = 1,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(producto);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("no está disponible", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenProductIsNotAvailable()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Recoger",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            var producto = new Producto
            {
                
                Nombre = "Empanada",
                Precio = 50m,
                Stock = 100,
                Disponible = false,
                EstaActivo = true,
                CategoriaId = 1,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(producto);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("no está disponible", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenCantidadIsZeroOrNegative()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Recoger",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 0 }
                }
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            var producto = new Producto
            {
                
                Nombre = "Empanada",
                Precio = 50m,
                Stock = 100,
                Disponible = true,
                EstaActivo = true,
                CategoriaId = 1,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(producto);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("mayor a cero", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_Fails_WhenProductHasInsufficientStock()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Domicilio",
                DireccionEntregaId = 1,
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 1000 }
                }
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            var producto = new Producto
            {
               
                Nombre = "Empanada",
                Precio = 50m,
                Stock = 10,
                Disponible = true,
                EstaActivo = true,
                CategoriaId = 1,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(producto);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Contains("Stock insuficiente", resultado.Mensaje);
        }

        [Fact]
        public async Task CrearPedidoAsync_CalculatesCorrectTotalsWithITBIS()
        {
            // Arrange
            var dto = new CrearPedidoDto
            {
                UsuarioId = 1,
                TipoEntrega = "Recoger",
                Detalles = new List<CrearDetallePedidoDto>
                {
                    new CrearDetallePedidoDto { ProductoId = 1, Cantidad = 2 }
                }
            };

            var usuario = new Usuario
            {
                
                Email = "test@test.com",
                EstaActivo = true,
                CreadoPor = "system"
            };

            var producto = new Producto
            {
                
                Nombre = "Empanada",
                Precio = 100m,
                Stock = 100,
                Disponible = true,
                EstaActivo = true,
                CategoriaId = 1,
                CreadoPor = "system"
            };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _productoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(producto);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            _pedidoRepoMock.Setup(r => r.GetConDetallesAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Pedido
                {
                    
                    UsuarioId = usuario.Id,
                    Usuario = usuario,
                    NumeroPedido = $"PED-2025-{id:D6}",
                    Estado = EstadoPedido.Recibido,
                    TipoEntrega = TipoEntrega.Recoger,
                    Subtotal = 200m,
                    Impuesto = 36m, // 18% de 200
                    Total = 236m,
                    DetallesPedido = new List<DetallePedido>()
                });

            // Act
            var resultado = await _service.CrearPedidoAsync(dto);

            // Assert
            Assert.True(resultado.Exito);
            Assert.Equal(200m, resultado.Data.Subtotal);
            Assert.Equal(36m, resultado.Data.Impuesto);
            Assert.Equal(236m, resultado.Data.Total);
        }

        #endregion

        #region ActualizarEstadoAsync Tests

        [Fact]
        public async Task ActualizarEstadoAsync_UpdatesEstado_WhenPedidoExists()
        {
            // Arrange
            var pedidoId = 1;
            var dto = new ActualizarEstadoPedidoDto
            {
                NuevoEstado = "EnPreparacion"
            };

            var pedido = new Pedido
            {
                
                UsuarioId = 1,
                NumeroPedido = "PED-001",
                Estado = EstadoPedido.Recibido,
                TipoEntrega = TipoEntrega.Domicilio,
                Subtotal = 100m,
                Impuesto = 18m,
                Total = 118m,
                CreadoPor = "system",
                DetallesPedido = new List<DetallePedido>()
            };

            _pedidoRepoMock.Setup(r => r.GetConDetallesAsync(pedidoId))
                .ReturnsAsync(pedido);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _service.ActualizarEstadoAsync(pedidoId, dto);

            // Assert
            Assert.True(resultado.Exito);
            Assert.NotNull(resultado.Data);
            Assert.Equal("EnPreparacion", resultado.Data.Estado);
        }

        [Fact]
        public async Task ActualizarEstadoAsync_Fails_WhenPedidoNotFound()
        {
            // Arrange
            var pedidoId = 999;
            var dto = new ActualizarEstadoPedidoDto
            {
                NuevoEstado = "EnPreparacion"
            };

            _pedidoRepoMock.Setup(r => r.GetConDetallesAsync(pedidoId))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await _service.ActualizarEstadoAsync(pedidoId, dto);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Equal("Pedido no encontrado", resultado.Mensaje);
        }

        #endregion

        #region ObtenerPorIdAsync Tests

        [Fact]
        public async Task ObtenerPorIdAsync_ReturnsPedido_WhenExists()
        {
            // Arrange
            var pedidoId = 1;
            var pedido = new Pedido
            {
                
                UsuarioId = 1,
                NumeroPedido = "PED-001",
                Estado = EstadoPedido.Recibido,
                TipoEntrega = TipoEntrega.Domicilio,
                Total = 118m,
                DetallesPedido = new List<DetallePedido>()
            };

            _pedidoRepoMock.Setup(r => r.GetConDetallesAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Act
            var resultado = await _service.ObtenerPorIdAsync(pedidoId);

            // Assert
            Assert.True(resultado.Exito);
            Assert.NotNull(resultado.Data);
            Assert.Equal(pedidoId, resultado.Data.Id);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_Fails_WhenPedidoNotFound()
        {
            // Arrange
            var pedidoId = 999;

            _pedidoRepoMock.Setup(r => r.GetConDetallesAsync(pedidoId))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await _service.ObtenerPorIdAsync(pedidoId);

            // Assert
            Assert.False(resultado.Exito);
            Assert.Equal("Pedido no encontrado", resultado.Mensaje);
        }

        #endregion

        #region ObtenerPorUsuarioAsync Tests

        [Fact]
        public async Task ObtenerPorUsuarioAsync_ReturnsUserPedidos()
        {
            // Arrange
            var usuarioId = 1;
            var pedidos = new List<Pedido>
            {
                new Pedido
                {
                    UsuarioId = usuarioId,
                    NumeroPedido = "PED-001",
                    Estado = EstadoPedido.Recibido,
                    Total = 118m,
                    DetallesPedido = new List<DetallePedido>(),
                    CreadoPor = "system"
                },
                new Pedido
                {
                    UsuarioId = usuarioId,
                    NumeroPedido = "PED-002",
                    Estado = EstadoPedido.Entregado,
                    Total = 236m,
                    DetallesPedido = new List<DetallePedido>(),
                    CreadoPor = "system"
                }
            };

            _pedidoRepoMock.Setup(r => r.GetByUsuarioAsync(usuarioId))
                .ReturnsAsync(pedidos);

            // Act
            var resultado = await _service.ObtenerPorUsuarioAsync(usuarioId);

            // Assert
            Assert.True(resultado.Exito);
            Assert.Equal(2, resultado.Data.Count);
        }

        #endregion

        #region ObtenerPorEstadoAsync Tests

        [Fact]
        public async Task ObtenerPorEstadoAsync_ReturnsPedidosWithSpecificEstado()
        {
            // Arrange
            var estado = "Recibido";
            var pedidos = new List<Pedido>
            {
                new Pedido
                {
                    Estado = EstadoPedido.Recibido,
                    DetallesPedido = new List<DetallePedido>(),
                    CreadoPor = "system"
                },
                new Pedido
                {
                    Estado = EstadoPedido.Recibido,
                    DetallesPedido = new List<DetallePedido>(),
                    CreadoPor = "system"
                }
            };

            _pedidoRepoMock.Setup(r => r.GetByEstadoAsync(EstadoPedido.Recibido))
                .ReturnsAsync(pedidos);

            // Act
            var resultado = await _service.ObtenerPorEstadoAsync(estado);

            // Assert
            Assert.True(resultado.Exito);
            Assert.Equal(2, resultado.Data.Count);
            Assert.All(resultado.Data, p => Assert.Equal("Recibido", p.Estado));
        }

        #endregion

        #region ObtenerPedidosDelDiaAsync Tests

        [Fact]
        public async Task ObtenerPedidosDelDiaAsync_ReturnsTodayPedidos()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                new Pedido
                {
                    FechaPedido = DateTime.UtcNow,
                    Estado = EstadoPedido.Recibido,
                    DetallesPedido = new List<DetallePedido>(),
                    CreadoPor = "system"
                }
            };

            _pedidoRepoMock.Setup(r => r.GetPedidosDelDiaAsync())
                .ReturnsAsync(pedidos);

            // Act
            var resultado = await _service.ObtenerPedidosDelDiaAsync();

            // Assert
            Assert.True(resultado.Exito);
            Assert.Single(resultado.Data);
        }

        #endregion
    }
} 
    

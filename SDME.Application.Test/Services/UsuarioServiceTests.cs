using global::SDME.Application.DTOs.Usuario;
using global::SDME.Application.Services;
using global::SDME.Domain.Entities.Core;
using global::SDME.Domain.Enums;
using global::SDME.Domain.Interfaces;
using global::SDME.Domain.Interfaces.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace SDME.Application.Test.Services
{
   
        public class UsuarioServiceTests
        {
            private readonly Mock<IUnitOfWork> _unitOfWorkMock;
            private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
            private readonly Mock<ILogger<UsuarioService>> _loggerMock;
            private readonly UsuarioService _service;

            public UsuarioServiceTests()
            {
                _unitOfWorkMock = new Mock<IUnitOfWork>();
                _usuarioRepoMock = new Mock<IUsuarioRepository>();
                _loggerMock = new Mock<ILogger<UsuarioService>>();

                _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
                _service = new UsuarioService(_unitOfWorkMock.Object, _loggerMock.Object);
            }

            [Fact]
            public async Task RegistrarAsync_CreatesNewUser_WhenEmailDoesNotExist()
            {
                // Arrange
                var dto = new RegistrarUsuarioDto
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    Telefono = "8091234567",
                    Password = "password123",
                    ConfirmarPassword = "password123"
                };

                _usuarioRepoMock.Setup(r => r.ExisteEmailAsync(dto.Email))
                    .ReturnsAsync(false);

                _usuarioRepoMock.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
                    .ReturnsAsync((Usuario u) => u);

                _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                    .ReturnsAsync(1);

                // Act
                var resultado = await _service.RegistrarAsync(dto);

                // Assert
                Assert.True(resultado.Exito);
                Assert.NotNull(resultado.Data);
                Assert.Equal("Juan", resultado.Data.Nombre);
                Assert.Equal("juan@test.com", resultado.Data.Email);
            }

            [Fact]
            public async Task RegistrarAsync_FailsWhenEmailAlreadyExists()
            {
                // Arrange
                var dto = new RegistrarUsuarioDto
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "existente@test.com",
                    Telefono = "8091234567",
                    Password = "password123",
                    ConfirmarPassword = "password123"
                };

                _usuarioRepoMock.Setup(r => r.ExisteEmailAsync(dto.Email))
                    .ReturnsAsync(true);

                // Act
                var resultado = await _service.RegistrarAsync(dto);

                // Assert
                Assert.False(resultado.Exito);
                Assert.Equal("El email ya está registrado", resultado.Mensaje);
            }

            [Fact]
            public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
            {
                // Arrange
                var dto = new LoginDto
                {
                    Email = "test@test.com",
                    Password = "password123"
                };

                var usuario = new Usuario
                {
                    
                    Nombre = "Test",
                    Apellido = "User",
                    Email = "test@test.com",
                    Telefono = "8091234567",
                    PasswordHash = Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes("password123")
                    ),
                    TipoUsuario = TipoUsuario.Cliente,
                    CreadoPor = "system"
                };

                _usuarioRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                    .ReturnsAsync(usuario);

                _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                    .ReturnsAsync(1);

                // Act
                var resultado = await _service.LoginAsync(dto);

                // Assert
                Assert.True(resultado.Exito);
                Assert.NotNull(resultado.Data);
                Assert.NotEmpty(resultado.Data.Token);
                Assert.Equal("Test", resultado.Data.Usuario.Nombre);
            }

            [Fact]
            public async Task LoginAsync_Fails_WhenUserNotFound()
            {
                // Arrange
                var dto = new LoginDto
                {
                    Email = "noexiste@test.com",
                    Password = "password123"
                };

                _usuarioRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                    .ReturnsAsync((Usuario?)null);

                // Act
                var resultado = await _service.LoginAsync(dto);

                // Assert
                Assert.False(resultado.Exito);
                Assert.Equal("Credenciales inválidas", resultado.Mensaje);
            }

            [Fact]
            public async Task ActualizarAsync_UpdatesUser_WhenUserExists()
            {
                // Arrange
                var usuarioId = 1;
                var dto = new ActualizarUsuarioDto
                {
                    Nombre = "Juan Actualizado",
                    Apellido = "Pérez Actualizado",
                    Telefono = "8099876543"
                };

                var usuario = new Usuario
                {
                
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    Telefono = "8091234567",
                    PasswordHash = "hash",
                    TipoUsuario = TipoUsuario.Cliente,
                    CreadoPor = "system"
                };

                _usuarioRepoMock.Setup(r => r.GetByIdAsync(usuarioId))
                    .ReturnsAsync(usuario);

                _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                    .ReturnsAsync(1);

                // Act
                var resultado = await _service.ActualizarAsync(usuarioId, dto);

                // Assert
                Assert.True(resultado.Exito);
                Assert.NotNull(resultado.Data);
                Assert.Equal("Juan Actualizado", resultado.Data.Nombre);
            }

            [Fact]
            public async Task ExisteEmailAsync_ReturnsTrue_WhenEmailExists()
            {
                // Arrange
                var email = "existe@test.com";
                _usuarioRepoMock.Setup(r => r.ExisteEmailAsync(email))
                    .ReturnsAsync(true);

                // Act
                var resultado = await _service.ExisteEmailAsync(email);

                // Assert
                Assert.True(resultado.Exito);
                Assert.True(resultado.Data);
            }
        }
    }



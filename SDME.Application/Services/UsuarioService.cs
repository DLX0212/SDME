using Microsoft.Extensions.Logging;
using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Usuario;
using SDME.Application.Interfaces;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces;

namespace SDME.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUnitOfWork unitOfWork, ILogger<UsuarioService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<UsuarioDto>> RegistrarAsync(RegistrarUsuarioDto dto)
        {
            try
            {
                _logger.LogInformation("Intentando registrar usuario con email: {Email}", dto.Email);

                // Verificar si el email ya existe
                var existeEmail = await _unitOfWork.Usuarios.ExisteEmailAsync(dto.Email);
                if (existeEmail)
                {
                    _logger.LogWarning("El email {Email} ya esta registrado", dto.Email);
                    return ResponseDto<UsuarioDto>.Failure("El email ya está registrado");
                }

                // Crear la entidad Usuario
                var usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email.ToLower(),
                    Telefono = dto.Telefono,
                    PasswordHash = HashPassword(dto.Password),
                    TipoUsuario = TipoUsuario.Cliente,
                    CreadoPor = dto.Email
                };

                // Guardar en la base de datos
                await _unitOfWork.Usuarios.AddAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Usuario registrado exitosamente con ID: {Id}", usuario.Id);

                // Convertir a DTO para devolver
                var usuarioDto = ConvertirADto(usuario);

                return ResponseDto<UsuarioDto>.Success(usuarioDto, "Usuario registrado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return ResponseDto<UsuarioDto>.Failure("Error al registrar el usuario");
            }
        }

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                _logger.LogInformation("Intento de login para email: {Email}", dto.Email);

                var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(dto.Email);

                if (usuario == null)
                {
                    _logger.LogWarning("Usuario no encontrado: {Email}", dto.Email);
                    return ResponseDto<LoginResponseDto>.Failure("Credenciales inválidas");
                }

                // Verificar password
                if (!VerificarPassword(dto.Password, usuario.PasswordHash))
                {
                    _logger.LogWarning("Password incorrecto para: {Email}", dto.Email);
                    return ResponseDto<LoginResponseDto>.Failure("Credenciales inválidas");
                }

                // Actualizar último acceso
                usuario.ActualizarUltimoAcceso();
                await _unitOfWork.SaveChangesAsync();

                var response = new LoginResponseDto
                {
                    Usuario = ConvertirADto(usuario),
                    Token = GenerarToken(usuario)
                };

                _logger.LogInformation("Login exitoso para usuario: {Id}", usuario.Id);

                return ResponseDto<LoginResponseDto>.Success(response, "Login exitoso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el login");
                return ResponseDto<LoginResponseDto>.Failure("Error al iniciar sesión");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);

                if (usuario == null)
                {
                    return ResponseDto<UsuarioDto>.Failure("Usuario no encontrado");
                }

                return ResponseDto<UsuarioDto>.Success(ConvertirADto(usuario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                return ResponseDto<UsuarioDto>.Failure("Error al obtener el usuario");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> ActualizarAsync(int id, ActualizarUsuarioDto dto)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);

                if (usuario == null)
                {
                    return ResponseDto<UsuarioDto>.Failure("Usuario no encontrado");
                }

                usuario.Nombre = dto.Nombre;
                usuario.Apellido = dto.Apellido;
                usuario.Telefono = dto.Telefono;
                usuario.ModificadoPor = usuario.Email;

                await _unitOfWork.Usuarios.UpdateAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<UsuarioDto>.Success(ConvertirADto(usuario), "Usuario actualizado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
                return ResponseDto<UsuarioDto>.Failure("Error al actualizar el usuario");
            }
        }

        public async Task<ResponseDto<bool>> ExisteEmailAsync(string email)
        {
            try
            {
                var existe = await _unitOfWork.Usuarios.ExisteEmailAsync(email);
                return ResponseDto<bool>.Success(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar email");
                return ResponseDto<bool>.Failure("Error al verificar el email");
            }
        }

        // Métodos auxiliares privados
        private UsuarioDto ConvertirADto(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                NombreCompleto = usuario.NombreCompleto,
                FechaUltimoAcceso = usuario.FechaUltimoAcceso,
                FechaCreacion = usuario.FechaCreacion,
                EstaActivo = usuario.EstaActivo
            };
        }

        
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerificarPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hash;
        }

        private string GenerarToken(Usuario usuario)
        {
            
            return $"token_{usuario.Id}_{DateTime.UtcNow.Ticks}";
        }
    }
}

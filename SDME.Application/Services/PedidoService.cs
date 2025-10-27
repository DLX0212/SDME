using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Pedido;
using SDME.Application.Interfaces;
using SDME.Application.Logging;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces;

namespace SDME.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger<PedidoService> _logger;

        public PedidoService(
            IUnitOfWork unitOfWork,
            IAppLogger<PedidoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto)
        {
            try
            {
                _logger.LogInformation($"Iniciando creación de pedido para usuario {dto.UsuarioId}");

                // ===== VALIDACIONES DE NEGOCIO (Requisitos SRS/SAD) =====

                // 1. Validar que el usuario existe y está activo
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(dto.UsuarioId);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuario {dto.UsuarioId} no encontrado");
                    return ResponseDto<PedidoDto>.Failure("Usuario no encontrado");
                }

                if (!usuario.EstaActivo)
                {
                    return ResponseDto<PedidoDto>.Failure("El usuario no está activo");
                }

                // 2. Validar que el pedido no esté vacío (SRS 3.3)
                if (dto.Detalles == null || !dto.Detalles.Any())
                {
                    return ResponseDto<PedidoDto>.Failure("El pedido debe contener al menos un producto");
                }

                // 3. Validar tipo de entrega válido
                if (dto.TipoEntrega != "Domicilio" && dto.TipoEntrega != "Recoger")
                {
                    return ResponseDto<PedidoDto>.Failure("Tipo de entrega inválido. Use 'Domicilio' o 'Recoger'");
                }

                // 4. Si es domicilio, validar que tenga dirección
                if (dto.TipoEntrega == "Domicilio" && !dto.DireccionEntregaId.HasValue)
                {
                    return ResponseDto<PedidoDto>.Failure("Debe especificar una dirección para entrega a domicilio");
                }

                // Iniciar transacción para consistencia de datos
                await _unitOfWork.BeginTransactionAsync();

                // Crear el pedido
                var pedido = new Pedido
                {
                    UsuarioId = dto.UsuarioId,
                    TipoEntrega = ConvertirTipoEntrega(dto.TipoEntrega),
                    DireccionEntregaId = dto.DireccionEntregaId,
                    NotasEspeciales = dto.NotasEspeciales,
                    Estado = EstadoPedido.Recibido,
                    CreadoPor = usuario.Email
                };

                // 5. Validar y agregar cada producto del pedido
                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalleDto.ProductoId);

                    // Validar que el producto existe
                    if (producto == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure(
                            $"Producto con ID {detalleDto.ProductoId} no encontrado"
                        );
                    }

                    // Validar que el producto está activo
                    if (!producto.EstaActivo)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure(
                            $"El producto '{producto.Nombre}' no está disponible"
                        );
                    }

                    // Validar que el producto está disponible
                    if (!producto.Disponible)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure(
                            $"El producto '{producto.Nombre}' no está disponible temporalmente"
                        );
                    }

                    // Validar cantidad positiva
                    if (detalleDto.Cantidad <= 0)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure(
                            "La cantidad debe ser mayor a cero"
                        );
                    }

                    // Validar stock disponible (Regla de negocio crítica)
                    if (!producto.TieneStock(detalleDto.Cantidad))
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure(
                            $"Stock insuficiente para '{producto.Nombre}'. " +
                            $"Disponible: {producto.Stock}, Solicitado: {detalleDto.Cantidad}"
                        );
                    }

                    // Crear detalle del pedido
                    var detalle = new DetallePedido
                    {
                        ProductoId = producto.Id,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = producto.Precio,
                        NotasEspeciales = detalleDto.NotasEspeciales
                    };

                    detalle.CalcularSubtotal();
                    pedido.AgregarDetalle(detalle);

                    // Reducir stock del producto
                    producto.ReducirStock(detalleDto.Cantidad);
                    await _unitOfWork.Productos.UpdateAsync(producto);
                }

                // 6. Calcular totales (incluye ITBIS 18% RD según SAD)
                pedido.CalcularTotal();

                // 7. Guardar el pedido
                await _unitOfWork.Pedidos.AddAsync(pedido);
                await _unitOfWork.SaveChangesAsync();

                // 8. Generar número de pedido único
                pedido.GenerarNumeroPedido();
                await _unitOfWork.Pedidos.UpdateAsync(pedido);
                await _unitOfWork.SaveChangesAsync();

                // Confirmar transacción
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Pedido creado exitosamente: {pedido.NumeroPedido}");

                // Obtener pedido completo con relaciones
                var pedidoCompleto = await _unitOfWork.Pedidos.GetConDetallesAsync(pedido.Id);

                return ResponseDto<PedidoDto>.Success(
                    ConvertirADto(pedidoCompleto!),
                    "Pedido creado exitosamente"
                );
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error de regla de negocio al crear pedido");
                return ResponseDto<PedidoDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error inesperado al crear pedido");
                return ResponseDto<PedidoDto>.Failure(
                    "Ocurrió un error al crear el pedido. Por favor intente nuevamente."
                );
            }
        }

        public async Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(
            int id,
            ActualizarEstadoPedidoDto dto)
        {
            try
            {
                // Validar que el pedido existe
                var pedido = await _unitOfWork.Pedidos.GetConDetallesAsync(id);
                if (pedido == null)
                {
                    return ResponseDto<PedidoDto>.Failure("Pedido no encontrado");
                }

                var nuevoEstado = ConvertirEstado(dto.NuevoEstado);

                // Aplicar reglas de transición de estado (lógica de negocio)
                try
                {
                    pedido.CambiarEstado(nuevoEstado);
                }
                catch (InvalidOperationException ex)
                {
                    return ResponseDto<PedidoDto>.Failure(ex.Message);
                }

                await _unitOfWork.Pedidos.UpdateAsync(pedido);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Estado del pedido {id} cambiado a {dto.NuevoEstado}");

                return ResponseDto<PedidoDto>.Success(
                    ConvertirADto(pedido),
                    "Estado actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar estado del pedido {id}");
                return ResponseDto<PedidoDto>.Failure("Error al actualizar el estado");
            }
        }

        public async Task<ResponseDto<PedidoDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var pedido = await _unitOfWork.Pedidos.GetConDetallesAsync(id);

                if (pedido == null)
                {
                    return ResponseDto<PedidoDto>.Failure("Pedido no encontrado");
                }

                return ResponseDto<PedidoDto>.Success(ConvertirADto(pedido));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener pedido {id}");
                return ResponseDto<PedidoDto>.Failure("Error al obtener el pedido");
            }
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var pedidos = await _unitOfWork.Pedidos.GetByUsuarioAsync(usuarioId);
                var pedidosDto = pedidos.Select(ConvertirADto).ToList();

                return ResponseDto<List<PedidoDto>>.Success(pedidosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener pedidos del usuario {usuarioId}");
                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener los pedidos");
            }
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorEstadoAsync(string estado)
        {
            try
            {
                var estadoEnum = ConvertirEstado(estado);
                var pedidos = await _unitOfWork.Pedidos.GetByEstadoAsync(estadoEnum);
                var pedidosDto = pedidos.Select(ConvertirADto).ToList();

                return ResponseDto<List<PedidoDto>>.Success(pedidosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener pedidos por estado {estado}");
                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener los pedidos");
            }
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPedidosDelDiaAsync()
        {
            try
            {
                var pedidos = await _unitOfWork.Pedidos.GetPedidosDelDiaAsync();
                var pedidosDto = pedidos.Select(ConvertirADto).ToList();

                return ResponseDto<List<PedidoDto>>.Success(pedidosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del día");
                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener los pedidos");
            }
        }

        // ===== MÉTODOS PRIVADOS AUXILIARES =====

        private PedidoDto ConvertirADto(Pedido pedido)
        {
            return new PedidoDto
            {
                Id = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                FechaPedido = pedido.FechaPedido,
                Estado = pedido.Estado.ToString(),
                Subtotal = pedido.Subtotal,
                Impuesto = pedido.Impuesto,
                Total = pedido.Total,
                TipoEntrega = pedido.TipoEntrega.ToString(),
                NotasEspeciales = pedido.NotasEspeciales,
                NombreCliente = pedido.Usuario?.NombreCompleto ?? "Cliente",
                Detalles = pedido.DetallesPedido?.Select(d => new DetallePedidoDto
                {
                    NombreProducto = d.Producto?.Nombre ?? "Producto",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    NotasEspeciales = d.NotasEspeciales
                }).ToList() ?? new List<DetallePedidoDto>(),
                FechaCreacion = pedido.FechaCreacion,
                EstaActivo = pedido.EstaActivo
            };
        }

        private TipoEntrega ConvertirTipoEntrega(string tipo)
        {
            return tipo.ToLower() switch
            {
                "domicilio" => TipoEntrega.Domicilio,
                "recoger" => TipoEntrega.Recoger,
                _ => TipoEntrega.Domicilio
            };
        }

        private EstadoPedido ConvertirEstado(string estado)
        {
            return estado.ToLower() switch
            {
                "recibido" => EstadoPedido.Recibido,
                "enpreparacion" => EstadoPedido.EnPreparacion,
                "encamino" => EstadoPedido.EnCamino,
                "entregado" => EstadoPedido.Entregado,
                "cancelado" => EstadoPedido.Cancelado,
                _ => EstadoPedido.Recibido
            };
        }
    }
}
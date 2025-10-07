using Microsoft.Extensions.Logging;
using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Pedido;
using SDME.Application.Interfaces;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces;

namespace SDME.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(IUnitOfWork unitOfWork, ILogger<PedidoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto)
        {
            try
            {
                _logger.LogInformation("Creando pedido para usuario {UsuarioId}", dto.UsuarioId);

                // Iniciar transacción
                await _unitOfWork.BeginTransactionAsync();

                // Validación de negocio: verificar que el usuario existe
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(dto.UsuarioId);
                if (usuario == null)
                {
                    return ResponseDto<PedidoDto>.Failure("Usuario no encontrado");
                }

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

                // Validar y agregar los detalles del pedido
                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalleDto.ProductoId);

                    if (producto == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure($"Producto con ID {detalleDto.ProductoId} no encontrado");
                    }

                    // Validación de negocio: verificar stock disponible
                    if (!producto.TieneStock(detalleDto.Cantidad))
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ResponseDto<PedidoDto>.Failure($"Stock insuficiente para {producto.Nombre}");
                    }

                    var detalle = new DetallePedido
                    {
                        ProductoId = producto.Id,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = producto.Precio,
                        NotasEspeciales = detalleDto.NotasEspeciales
                    };

                    detalle.CalcularSubtotal();
                    pedido.AgregarDetalle(detalle);

                    // Reducir el stock del producto
                    producto.ReducirStock(detalleDto.Cantidad);
                    await _unitOfWork.Productos.UpdateAsync(producto);
                }

                // Calcular totales del pedido
                pedido.CalcularTotal();

                // Guardar el pedido
                await _unitOfWork.Pedidos.AddAsync(pedido);
                await _unitOfWork.SaveChangesAsync();

                // Generar número de pedido
                pedido.GenerarNumeroPedido();
                await _unitOfWork.Pedidos.UpdateAsync(pedido);
                await _unitOfWork.SaveChangesAsync();

                // Confirmar transacción
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Pedido creado exitosamente: {NumeroPedido}", pedido.NumeroPedido);

                // Obtener el pedido completo con sus relaciones
                var pedidoCompleto = await _unitOfWork.Pedidos.GetConDetallesAsync(pedido.Id);

                return ResponseDto<PedidoDto>.Success(
                    ConvertirADto(pedidoCompleto!),
                    "Pedido creado exitosamente"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error al crear pedido");
                return ResponseDto<PedidoDto>.Failure("Error al crear el pedido");
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
                _logger.LogError(ex, "Error al obtener pedido {Id}", id);
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
                _logger.LogError(ex, "Error al obtener pedidos del usuario {UsuarioId}", usuarioId);
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
                _logger.LogError(ex, "Error al obtener pedidos por estado {Estado}", estado);
                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener los pedidos");
            }
        }

        public async Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto)
        {
            try
            {
                var pedido = await _unitOfWork.Pedidos.GetByIdAsync(id);

                if (pedido == null)
                {
                    return ResponseDto<PedidoDto>.Failure("Pedido no encontrado");
                }

                var nuevoEstado = ConvertirEstado(dto.NuevoEstado);

                // Validación de negocio: cambiar estado del pedido
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

                _logger.LogInformation("Estado del pedido {Id} actualizado a {Estado}", id, dto.NuevoEstado);

                var pedidoActualizado = await _unitOfWork.Pedidos.GetConDetallesAsync(id);
                return ResponseDto<PedidoDto>.Success(
                    ConvertirADto(pedidoActualizado!),
                    "Estado actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido {Id}", id);
                return ResponseDto<PedidoDto>.Failure("Error al actualizar el estado del pedido");
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
                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener los pedidos del día");
            }
        }

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

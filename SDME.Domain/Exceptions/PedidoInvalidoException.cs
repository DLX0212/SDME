namespace SDME.Domain.Exceptions
{
    /// <summary>
    /// Excepción cuando un pedido no cumple con las reglas de negocio
    /// </summary>
    public class PedidoInvalidoException : DomainException
    {
        public int? PedidoId { get; }

        public PedidoInvalidoException(string message) : base(message)
        {
        }

        public PedidoInvalidoException(int pedidoId, string message) : base(message)
        {
            PedidoId = pedidoId;
        }

        public static PedidoInvalidoException PedidoVacio()
        {
            return new PedidoInvalidoException("El pedido debe contener al menos un producto");
        }

        public static PedidoInvalidoException EstadoInvalido(int pedidoId, string estadoActual)
        {
            return new PedidoInvalidoException(
                pedidoId,
                $"No se puede realizar esta operación. El pedido está en estado: {estadoActual}"
            );
        }
    }
}

namespace SDME.Domain.Exceptions
{
    /// <summary>
    /// Excepción cuando un pago es rechazado
    /// </summary>
    public class PagoRechazadoException : DomainException
    {
        public int PedidoId { get; }
        public string Motivo { get; }

        public PagoRechazadoException(int pedidoId, string motivo)
            : base($"El pago del pedido #{pedidoId} fue rechazado. Motivo: {motivo}")
        {
            PedidoId = pedidoId;
            Motivo = motivo;
        }
    }
}
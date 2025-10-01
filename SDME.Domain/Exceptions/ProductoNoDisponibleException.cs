namespace SDME.Domain.Exceptions
{
    /// <summary>
    /// Excepción cuando un producto no está disponible o sin stock
    /// </summary>
    public class ProductoNoDisponibleException : DomainException
    {
        public int ProductoId { get; }
        public string ProductoNombre { get; }

        public ProductoNoDisponibleException(int productoId, string productoNombre)
            : base($"El producto '{productoNombre}' no está disponible o no tiene stock suficiente")
        {
            ProductoId = productoId;
            ProductoNombre = productoNombre;
        }

        public ProductoNoDisponibleException(int productoId, string productoNombre, int cantidadSolicitada, int stockDisponible)
            : base($"El producto '{productoNombre}' no tiene stock suficiente. Solicitado: {cantidadSolicitada}, Disponible: {stockDisponible}")
        {
            ProductoId = productoId;
            ProductoNombre = productoNombre;
        }
    }
}

namespace SDME.Application.DTOs.Common
{
    public class PaginacionDto
    {
        public int PaginaActual { get; set; } = 1;
        public int TamanioPagina { get; set; } = 10;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }

    public class PaginacionResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public PaginacionDto Paginacion { get; set; } = new();
    }
}

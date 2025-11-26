using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Common;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{

    /// Servicio de categorías - USA la abstracción IHttpClientService
    public class CategoriaApiService : BaseApiService<CategoriaDto>, ICategoriaService
    {
        public CategoriaApiService(IHttpClientService httpClient)
            : base(httpClient, "Categorias")
        {
        }

    }
}
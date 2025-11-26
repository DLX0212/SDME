using SDME.Application.DTOs.Common;

namespace SDME.Web.Services.Base
{

    /// Interfaz base para cliente HTTP de consumo de API, define operaciones CRUD 

    public interface IApiClient
    {
        /// Get a Endpoint
        Task<ResponseDto<T>> GetAsync<T>(string endpoint) where T : class;
        /// GET 
        Task<ResponseDto<List<T>>> GetListAsync<T>(string endpoint) where T : class;

        ///POST
        Task<ResponseDto<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
            where TResponse : class;

        /// PUT
        Task<ResponseDto<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload)
            where TResponse : class;

        /// PATCH
        Task<ResponseDto<TResponse>> PatchAsync<TRequest, TResponse>(string endpoint, TRequest payload)
            where TResponse : class;
     ///DELETE

        Task<ResponseDto<bool>> DeleteAsync(string endpoint);
    }
}
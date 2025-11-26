using SDME.Application.DTOs.Common;

namespace SDME.Web.Services.Base
{
    /// Interfaz para cliente HTTP genérico
 
    public interface IHttpClientService
    {

        ///  GET que retorna un objeto
        Task<ResponseDto<T>> GetAsync<T>(string endpoint) where T : class;

        ///  GET que retorna una lista
        Task<ResponseDto<List<T>>> GetListAsync<T>(string endpoint) where T : class;

        Task<ResponseDto<TPrimitive>> GetPrimitiveAsync<TPrimitive>(string endpoint);

        ///  POST
        Task<ResponseDto<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload) where TResponse : class;

        /// PUT
        Task<ResponseDto<TResponse>> PutAsync<TRequest, TResponse>(string endpoint,TRequest payload) where TResponse : class;

        ///  PATCH
        Task<ResponseDto<TResponse>> PatchAsync<TRequest, TResponse>(string endpoint,TRequest payload) where TResponse : class;

    
        ///  DELETE
        Task<ResponseDto<bool>> DeleteAsync(string endpoint);
    }
}

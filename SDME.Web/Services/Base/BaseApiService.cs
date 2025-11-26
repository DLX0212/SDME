using SDME.Application.DTOs.Common;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services.Base
{
    // Esta clase implementa la lógica CRUD una sola vez para TODOS los servicios
    public abstract class BaseApiService<T> : IApiService<T> where T : class
    {
        protected readonly IHttpClientService _httpClient;
        protected readonly string _endpoint; // Ejemplo: "Productos", "Usuarios"

        public BaseApiService(IHttpClientService httpClient, string endpoint)
        {
            _httpClient = httpClient;
            _endpoint = endpoint;
        }

        // Implementación genérica: Sirve para Producto, Categoria, Pedido, etc.
        public virtual async Task<ResponseDto<List<T>>> ObtenerTodosAsync()
        {
            return await _httpClient.GetListAsync<T>(_endpoint);
        }

        public virtual async Task<ResponseDto<T>> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetAsync<T>($"{_endpoint}/{id}");
        }

        public virtual async Task<ResponseDto<T>> CrearAsync(object dto)
        {
            return await _httpClient.PostAsync<object, T>(_endpoint, dto);
        }

        public virtual async Task<ResponseDto<T>> ActualizarAsync(int id, object dto)
        {
            return await _httpClient.PutAsync<object, T>($"{_endpoint}/{id}", dto);
        }

        public virtual async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{_endpoint}/{id}");
        }
    }
}
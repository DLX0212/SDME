using SDME.Application.DTOs.Common;

namespace SDME.Web.Services.Interfaces
{

 // Interfaz para servicios de consumo de api (CRUD)

    public interface IApiService<T> where T : class
    {

        /// Obtiene todos los registros

        Task<ResponseDto<List<T>>> ObtenerTodosAsync();

        /// Obtiene un registro por ID

        Task<ResponseDto<T>> ObtenerPorIdAsync(int id);

        /// Crea un nuevo registro

        Task<ResponseDto<T>> CrearAsync(object dto);


        /// Actualiza un registro existente

        Task<ResponseDto<T>> ActualizarAsync(int id, object dto);


        /// Elimina un registro

        Task<ResponseDto<bool>> EliminarAsync(int id);
    }
}

namespace SDME.Application.DTOs.Common
{

 // DTO genérico para respuestas del API

    public class ResponseDto<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errores { get; set; } = new();

        public static ResponseDto<T> Success(T data, string mensaje = "Operación exitosa")
        {
            return new ResponseDto<T>
            {
                Exito = true,
                Mensaje = mensaje,
                Data = data
            };
        }

        public static ResponseDto<T> Failure(string mensaje, List<string>? errores = null)
        {
            return new ResponseDto<T>
            {
                Exito = false,
                Mensaje = mensaje,
                Errores = errores ?? new List<string>()
            };
        }
    }
}

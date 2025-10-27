namespace SDME.Application.Logging
{
    /// Abstracción del logger para no depender directamente de ILogger de Microsoft
    public interface IAppLogger<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception exception, string message);
    }
}

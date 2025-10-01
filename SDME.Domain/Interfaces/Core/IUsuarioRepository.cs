using SDME.Domain.Entities.Core;

namespace SDME.Domain.Interfaces.Core
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> ExisteEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetClientesAsync();
        Task<IEnumerable<Usuario>> GetAdministradoresAsync();
    }
}
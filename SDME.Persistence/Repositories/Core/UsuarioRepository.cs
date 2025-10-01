using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Base;
using SDME.Persistence.Context;

namespace SDME.Persistence.Repositories.Core
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SDMEDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Usuario>> GetClientesAsync()
        {
            return await _dbSet
                .Where(u => u.TipoUsuario == TipoUsuario.Cliente)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAdministradoresAsync()
        {
            return await _dbSet
                .Where(u => u.TipoUsuario == TipoUsuario.Administrador)
                .ToListAsync();
        }
    }
}

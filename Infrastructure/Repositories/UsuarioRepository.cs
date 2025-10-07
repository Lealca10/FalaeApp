using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DatabaseContext _context;

        public UsuarioRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<UsuarioDomain> GetByIdAsync(string id)
        {
            return await _context.Usuarios
                .Include(u => u.Preferencias)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UsuarioDomain> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<UsuarioDomain>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Preferencias)
                .Where(u => u.Ativo)
                .ToListAsync();
        }

        public async Task AddAsync(UsuarioDomain usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UsuarioDomain usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var usuario = await GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Ativo = false;
                await UpdateAsync(usuario);
            }
        }
    }
}
}

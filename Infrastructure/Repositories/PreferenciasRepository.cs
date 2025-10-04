using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.BaseDados;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PreferenciasRepository : IPreferenciasRepository
    {
        private readonly DatabaseContext _context;

        public PreferenciasRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<PreferenciasUsuario> GetByUsuarioIdAsync(string usuarioId)
        {
            return await _context.PreferenciasUsuarios
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task AddAsync(PreferenciasUsuario preferencias)
        {
            await _context.PreferenciasUsuarios.AddAsync(preferencias);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PreferenciasUsuario preferencias)
        {
            _context.PreferenciasUsuarios.Update(preferencias);
            await _context.SaveChangesAsync();
        }
    }
}

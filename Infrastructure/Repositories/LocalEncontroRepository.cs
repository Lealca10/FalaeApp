using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LocalEncontroRepository : ILocalEncontroRepository
    {
        private readonly DatabaseContext _context;

        public LocalEncontroRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<LocalEncontroDomain> GetByIdAsync(string id)
        {
            return await _context.LocaisEncontro
                .FirstOrDefaultAsync(l => l.Id == id && l.Ativo);
        }

        public async Task<LocalEncontroDomain> GetByIdWithEncontrosAsync(string id)
        {
            return await _context.LocaisEncontro
                .Include(l => l.Encontros)
                .FirstOrDefaultAsync(l => l.Id == id && l.Ativo);
        }

        public async Task<IEnumerable<LocalEncontroDomain>> GetAllAsync()
        {
            return await _context.LocaisEncontro
                .Where(l => l.Ativo)
                .OrderBy(l => l.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<LocalEncontroDomain>> GetByCapacidadeAsync(int capacidadeMinima)
        {
            return await _context.LocaisEncontro
                .Where(l => l.Capacidade >= capacidadeMinima && l.Ativo)
                .OrderBy(l => l.Capacidade)
                .ToListAsync();
        }

        public async Task AddAsync(LocalEncontroDomain local)
        {
            await _context.LocaisEncontro.AddAsync(local);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LocalEncontroDomain local)
        {
            _context.LocaisEncontro.Update(local);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var local = await GetByIdAsync(id);
            if (local != null)
            {
                local.Ativo = false;
                await UpdateAsync(local);
            }
        }

        public async Task<bool> ExisteLocalComNomeAsync(string nome)
        {
            return await _context.LocaisEncontro
                .AnyAsync(l => l.Nome.ToLower() == nome.ToLower() && l.Ativo);
        }
    }
}

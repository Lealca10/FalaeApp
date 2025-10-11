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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DatabaseContext _context;

        public FeedbackRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<FeedbackEncontroDomain> GetByIdAsync(string id)
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Encontro)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<FeedbackEncontroDomain>> GetByEncontroIdAsync(string encontroId)
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Usuario)
                .Where(f => f.EncontroId == encontroId)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeedbackEncontroDomain>> GetByUsuarioIdAsync(string usuarioId)
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Encontro)
                .Where(f => f.UsuarioId == usuarioId)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeedbackEncontroDomain>> GetByNotaAsync(int notaMinima, int notaMaxima)
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Encontro)
                .Include(f => f.Usuario)
                .Where(f => f.Nota >= notaMinima && f.Nota <= notaMaxima)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();
        }

        public async Task<double> GetMediaNotaByEncontroIdAsync(string encontroId)
        {
            var media = await _context.FeedbacksEncontro
                .Where(f => f.EncontroId == encontroId)
                .AverageAsync(f => (double?)f.Nota) ?? 0.0;

            return media;
        }

        public async Task<double> GetMediaNotaByLocalIdAsync(string localId)
        {
            var media = await _context.FeedbacksEncontro
                .Include(f => f.Encontro)
                .Where(f => f.Encontro.LocalId == localId)
                .AverageAsync(f => (double?)f.Nota) ?? 0.0;

            return media;
        }

        public async Task AddAsync(FeedbackEncontroDomain feedback)
        {
            await _context.FeedbacksEncontro.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FeedbackEncontroDomain feedback)
        {
            _context.FeedbacksEncontro.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var feedback = await GetByIdAsync(id);
            if (feedback != null)
            {
                _context.FeedbacksEncontro.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsuarioJaDeuFeedbackAsync(string encontroId, string usuarioId)
        {
            return await _context.FeedbacksEncontro
                .AnyAsync(f => f.EncontroId == encontroId && f.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<FeedbackEncontroDomain>> GetAllAsync()
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Encontro)
                .Include(f => f.Usuario)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();
        }
    }
}

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.BaseDados;
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
                .Include(f => f.Usuario)
                .Include(f => f.Encontro)
                    .ThenInclude(e => e.Local)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<FeedbackEncontroDomain>> GetByEncontroIdAsync(string encontroId)
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Usuario)
                .Include(f => f.Encontro)
                    .ThenInclude(e => e.Local)
                .Where(f => f.EncontroId == encontroId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeedbackEncontroDomain>> GetByUsuarioIdAsync(string usuarioId)
        {
            return await _context.FeedbacksEncontro
                .Include(f => f.Usuario)
                .Include(f => f.Encontro)
                    .ThenInclude(e => e.Local)
                .Where(f => f.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<FeedbackEncontroDomain> CreateAsync(FeedbackEncontroDomain feedback)
        {
            _context.FeedbacksEncontro.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task UpdateAsync(FeedbackEncontroDomain feedback)
        {
            _context.FeedbacksEncontro.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var feedback = await _context.FeedbacksEncontro.FindAsync(id);
            if (feedback != null)
            {
                _context.FeedbacksEncontro.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string encontroId, string usuarioId)
        {
            return await _context.FeedbacksEncontro
                .AnyAsync(f => f.EncontroId == encontroId && f.UsuarioId == usuarioId);
        }

        public Task<IEnumerable<FeedbackEncontroDomain>> GetByNotaAsync(int notaMinima, int notaMaxima)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetMediaNotaByEncontroIdAsync(string encontroId)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetMediaNotaByLocalIdAsync(string localId)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(FeedbackEncontroDomain feedback)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UsuarioJaDeuFeedbackAsync(string encontroId, string usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FeedbackEncontroDomain>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
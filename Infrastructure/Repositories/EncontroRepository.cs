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
    public class EncontroRepository : IEncontroRepository
    {
        private readonly DatabaseContext _context;

        public EncontroRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Encontro> GetByIdAsync(string id)
        {
            return await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Include(e => e.Feedbacks)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EncontroDomain> GetByIdWithDetailsAsync(string id)
        {
            return await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Include(e => e.Feedbacks)
                    .ThenInclude(f => f.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EncontroDomain>> GetAllAsync()
        {
            return await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .OrderByDescending(e => e.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<EncontroDomain>> GetByUsuarioIdAsync(string usuarioId)
        {
            return await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Where(e => e.Participantes.Any(p => p.Id == usuarioId))
                .OrderByDescending(e => e.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<EncontroDomain>> GetByLocalIdAsync(string localId)
        {
            return await _context.Encontros
                .Include(e => e.Participantes)
                .Where(e => e.LocalId == localId)
                .OrderByDescending(e => e.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<EncontroDomain>> GetByDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Where(e => e.DataHora >= dataInicio && e.DataHora <= dataFim)
                .OrderBy(e => e.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<EncontroDomain>> GetByStatusAsync(string status)
        {
            return await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.DataHora)
                .ToListAsync();
        }

        public async Task AddAsync(EncontroDomain encontro)
        {
            await _context.Encontros.AddAsync(encontro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EncontroDomain encontro)
        {
            _context.Encontros.Update(encontro);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var encontro = await GetByIdAsync(id);
            if (encontro != null)
            {
                _context.Encontros.Remove(encontro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AdicionarParticipanteAsync(string encontroId, UsuarioDomain participante)
        {
            var encontro = await _context.Encontros
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == encontroId);

            if (encontro == null) return false;

            encontro.Participantes.Add(participante);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverParticipanteAsync(string encontroId, string usuarioId)
        {
            var encontro = await _context.Encontros
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == encontroId);

            if (encontro == null) return false;

            var participante = encontro.Participantes.FirstOrDefault(p => p.Id == usuarioId);
            if (participante != null)
            {
                encontro.Participantes.Remove(participante);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExisteConflitoHorarioAsync(string localId, DateTime dataHora)
        {
            return await _context.Encontros
                .AnyAsync(e => e.LocalId == localId &&
                              e.DataHora.Date == dataHora.Date &&
                              e.Status != "cancelado");
        }
    }
}

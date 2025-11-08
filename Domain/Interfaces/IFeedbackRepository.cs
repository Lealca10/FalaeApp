using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<FeedbackEncontroDomain> GetByIdAsync(string id);
        Task<IEnumerable<FeedbackEncontroDomain>> GetByEncontroIdAsync(string encontroId);
        Task<IEnumerable<FeedbackEncontroDomain>> GetByUsuarioIdAsync(string usuarioId);
        Task<IEnumerable<FeedbackEncontroDomain>> GetByNotaAsync(int notaMinima, int notaMaxima);
        Task<double> GetMediaNotaByEncontroIdAsync(string encontroId);
        Task<double> GetMediaNotaByLocalIdAsync(string localId);
        Task AddAsync(FeedbackEncontroDomain feedback);
        Task UpdateAsync(FeedbackEncontroDomain feedback);
        Task DeleteAsync(string id);
        Task<bool> UsuarioJaDeuFeedbackAsync(string encontroId, string usuarioId);
        Task<IEnumerable<FeedbackEncontroDomain>> GetAllAsync();
        Task<bool> ExistsAsync(string encontroId, string usuarioId);
        Task<FeedbackEncontroDomain> CreateAsync(FeedbackEncontroDomain feedback);
    }
}

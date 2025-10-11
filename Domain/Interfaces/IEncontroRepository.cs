using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEncontroRepository
    {
        Task<EncontroDomain> GetByIdAsync(string id);
        Task<EncontroDomain> GetByIdWithDetailsAsync(string id);
        Task<IEnumerable<EncontroDomain>> GetAllAsync();
        Task<IEnumerable<EncontroDomain>> GetByUsuarioIdAsync(string usuarioId);
        Task<IEnumerable<EncontroDomain>> GetByLocalIdAsync(string localId);
        Task<IEnumerable<EncontroDomain>> GetByDataAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<EncontroDomain>> GetByStatusAsync(string status);
        Task AddAsync(EncontroDomain encontro);
        Task UpdateAsync(EncontroDomain encontro);
        Task DeleteAsync(string id);
        Task<bool> AdicionarParticipanteAsync(string encontroId, UsuarioDomain participante);
        Task<bool> RemoverParticipanteAsync(string encontroId, string usuarioId);
        Task<bool> ExisteConflitoHorarioAsync(string localId, DateTime dataHora);
    }
}
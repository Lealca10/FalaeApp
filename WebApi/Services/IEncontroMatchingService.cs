using Application.response;
using Domain.Entities;
using WebApi.Models.Request;

namespace WebApi.Services
{
    public interface IEncontroMatchingService
    {
        // Método original (mantido para compatibilidade)
        Task<MatchingResult> EncontrarParticipantesCompatíveis(MatchingRequest request);

        // Nova sobrecarga com usuário logado
        Task<MatchingResult> EncontrarParticipantesCompatíveis(MatchingRequest request, UsuarioDomain usuarioLogado);
    }
}
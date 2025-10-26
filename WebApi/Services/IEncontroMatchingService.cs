using Application.response;
using WebApi.Models.Request; // CORRIGIDO: usar WebApi.Models.Request

namespace WebApi.Services
{
    public interface IEncontroMatchingService
    {
        Task<MatchingResult> EncontrarParticipantesCompatíveis(MatchingRequest request);
        int CalcularPreferenciasCompatíveis(Domain.Entities.PreferenciasUsuarioDomain pref1, Domain.Entities.PreferenciasUsuarioDomain pref2);
    }
}
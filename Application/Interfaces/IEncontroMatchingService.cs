using Application.Request;
using Application.response;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEncontroMatchingService
    {
        Task<MatchingResult> EncontrarParticipantesCompatíveis(MatchingRequest request);

        int CalcularPreferenciasCompatíveis(
            PreferenciasUsuarioDomain pref1,
            PreferenciasUsuarioDomain pref2);
    }
}

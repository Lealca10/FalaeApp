using Application.Interfaces;
using Application.Request;
using Domain.Entities;
using Domain.Interfaces;


namespace Application.UseCases.Encontro
{
    public interface ICriarEncontroComUsuarioService
    {
        Task<EncontroDomain> CriarAsync(CriarEncontroComUsuarioRequest request, string usuarioId);
    }

    public class CriarEncontroComUsuarioService : ICriarEncontroComUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IEncontroRepository _encontroRepo;
        private readonly ILocalEncontroRepository _localRepo;
        private readonly IEncontroMatchingService _matchingService;

        public CriarEncontroComUsuarioService(
            IUsuarioRepository usuarioRepo,
            IEncontroRepository encontroRepo,
            ILocalEncontroRepository localRepo,
            IEncontroMatchingService matchingService)
        {
            _usuarioRepo = usuarioRepo;
            _encontroRepo = encontroRepo;
            _localRepo = localRepo;
            _matchingService = matchingService;
        }

        public async Task<EncontroDomain> CriarAsync(CriarEncontroComUsuarioRequest request, string usuarioId)
        {
            var usuarioLogado = await _usuarioRepo.GetByIdAsync(usuarioId);
            if (usuarioLogado == null)
                throw new Exception("Usuário logado não encontrado.");

            var local = await _localRepo.GetByIdAsync(request.LocalId);
            if (local == null || !local.Ativo)
                throw new Exception("Local inválido ou inativo.");

            var matchingRequest = new MatchingRequest
            {
                LocalId = request.LocalId,
                DataHora = request.DataHora,
                MinimoPreferenciasIguais = request.MinimoPreferenciasIguais,
                NumeroParticipantes = request.NumeroParticipantes
            };

            var matching = await _matchingService.EncontrarParticipantesCompatíveis(matchingRequest);

            if (!matching.Sucesso)
                throw new Exception(matching.Mensagem);

            var encontro = new EncontroDomain
            {
                LocalId = request.LocalId,
                DataHora = request.DataHora,
                Status = "agendado",
                DataCriacao = DateTime.UtcNow,
                Participantes = new List<UsuarioDomain>()
            };

            encontro.Participantes.Add(usuarioLogado);

            foreach (var p in matching.ParticipantesSugeridos)
            {
                if (p.Id != usuarioLogado.Id)
                {
                    var user = await _usuarioRepo.GetByIdAsync(p.Id);
                    if (user != null)
                        encontro.Participantes.Add(user);
                }
            }

            await _encontroRepo.AddAsync(encontro);

            return encontro;
        }
    }
}

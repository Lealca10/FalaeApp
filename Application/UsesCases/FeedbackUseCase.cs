using Application.Interfaces;
using Application.Request;
using Application.response;
using Application.Response;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases
{
    public class FeedbackUseCase : IFeedbackUseCase
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackUseCase(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<FeedbackResponse> CriarFeedbackAsync(FeedbackRequest request)
        {
            // Validar se já existe feedback para este encontro e usuário
            var feedbackExistente = await _feedbackRepository.ExistsAsync(request.EncontroId, request.UsuarioId);
            if (feedbackExistente)
            {
                throw new InvalidOperationException("Já existe um feedback para este encontro e usuário");
            }

            // Validar nota (1-5)
            if (request.Nota < 1 || request.Nota > 5)
            {
                throw new ArgumentException("A nota deve estar entre 1 e 5");
            }

            var feedback = new FeedbackEncontroDomain
            {
                Id = Guid.NewGuid().ToString(),
                EncontroId = request.EncontroId,
                UsuarioId = request.UsuarioId,
                Nota = request.Nota,
                Comentario = request.Comentario,
                DataCriacao = DateTime.UtcNow
            };

            var feedbackCriado = await _feedbackRepository.CreateAsync(feedback);
            return MapToResponse(feedbackCriado);
        }

        public async Task<FeedbackResponse> ObterFeedbackPorIdAsync(string id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
                throw new KeyNotFoundException("Feedback não encontrado");

            return MapToResponse(feedback);
        }

        public async Task<IEnumerable<FeedbackResponse>> ObterFeedbacksPorEncontroAsync(string encontroId)
        {
            var feedbacks = await _feedbackRepository.GetByEncontroIdAsync(encontroId);
            return feedbacks.Select(MapToResponse);
        }

        public async Task<IEnumerable<FeedbackResponse>> ObterFeedbacksPorUsuarioAsync(string usuarioId)
        {
            var feedbacks = await _feedbackRepository.GetByUsuarioIdAsync(usuarioId);
            return feedbacks.Select(MapToResponse);
        }

        public async Task<FeedbackResponse> AtualizarFeedbackAsync(string id, FeedbackRequest request)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
                throw new KeyNotFoundException("Feedback não encontrado");

            // Validar nota (1-5)
            if (request.Nota < 1 || request.Nota > 5)
            {
                throw new ArgumentException("A nota deve estar entre 1 e 5");
            }

            feedback.Nota = request.Nota;
            feedback.Comentario = request.Comentario;

            await _feedbackRepository.UpdateAsync(feedback);
            return MapToResponse(feedback);
        }

        public async Task DeletarFeedbackAsync(string id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }

        public async Task<bool> VerificarFeedbackExistenteAsync(string encontroId, string usuarioId)
        {
            return await _feedbackRepository.ExistsAsync(encontroId, usuarioId);
        }

        private FeedbackResponse MapToResponse(FeedbackEncontroDomain feedback)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                EncontroId = feedback.EncontroId,
                UsuarioId = feedback.UsuarioId,
                Nota = feedback.Nota,
                Comentario = feedback.Comentario,
                DataCriacao = feedback.DataCriacao,
                Usuario = new UsuarioInfo
                {
                    Id = feedback.Usuario?.Id ?? string.Empty,
                    Nome = feedback.Usuario?.Nome ?? string.Empty,
                    Email = feedback.Usuario?.Email ?? string.Empty,
                    Cidade = feedback.Usuario?.Cidade ?? string.Empty
                },
                Encontro = new EncontroInfo
                {
                    Id = feedback.Encontro?.Id ?? string.Empty,
                    DataHora = feedback.Encontro?.DataHora ?? DateTime.MinValue,
                    Status = feedback.Encontro?.Status ?? string.Empty,
                    Local = new LocalEncontroInfo
                    {
                        Id = feedback.Encontro?.Local?.Id ?? string.Empty,
                        Nome = feedback.Encontro?.Local?.Nome ?? string.Empty,
                        Endereco = feedback.Encontro?.Local?.Endereco ?? string.Empty,
                        Capacidade = feedback.Encontro?.Local?.Capacidade ?? 0,
                        ImagemUrl = feedback.Encontro?.Local?.ImagemUrl ?? string.Empty
                    }
                }
            };
        }
    }
}
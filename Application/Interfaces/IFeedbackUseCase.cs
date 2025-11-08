using Application.Request;
using Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFeedbackUseCase
    {
        Task<FeedbackResponse> CriarFeedbackAsync(FeedbackRequest request);
        Task<FeedbackResponse> ObterFeedbackPorIdAsync(string id);
        Task<IEnumerable<FeedbackResponse>> ObterFeedbacksPorEncontroAsync(string encontroId);
        Task<IEnumerable<FeedbackResponse>> ObterFeedbacksPorUsuarioAsync(string usuarioId);
        Task<FeedbackResponse> AtualizarFeedbackAsync(string id, FeedbackRequest request);
        Task DeletarFeedbackAsync(string id);
        Task<bool> VerificarFeedbackExistenteAsync(string encontroId, string usuarioId);
    }
}

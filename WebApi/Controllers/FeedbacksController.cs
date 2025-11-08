using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Request;
using Application.Response;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackUseCase _feedbackUseCase;

        public FeedbacksController(IFeedbackUseCase feedbackUseCase)
        {
            _feedbackUseCase = feedbackUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackResponse>>> GetFeedbacks()
        {
            try
            {
                // Por padrão, retorna todos os feedbacks (ou pode implementar paginação)
                var feedbacks = await _feedbackUseCase.ObterFeedbacksPorEncontroAsync("");
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao buscar feedbacks", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackResponse>> GetFeedback(string id)
        {
            try
            {
                var feedback = await _feedbackUseCase.ObterFeedbackPorIdAsync(id);
                return Ok(feedback);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Feedback não encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao buscar feedback", details = ex.Message });
            }
        }

        [HttpGet("encontro/{encontroId}")]
        public async Task<ActionResult<IEnumerable<FeedbackResponse>>> GetFeedbacksPorEncontro(string encontroId)
        {
            try
            {
                var feedbacks = await _feedbackUseCase.ObterFeedbacksPorEncontroAsync(encontroId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao buscar feedbacks do encontro", details = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<FeedbackResponse>>> GetFeedbacksPorUsuario(string usuarioId)
        {
            try
            {
                var feedbacks = await _feedbackUseCase.ObterFeedbacksPorUsuarioAsync(usuarioId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao buscar feedbacks do usuário", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<FeedbackResponse>> PostFeedback([FromBody] FeedbackRequest request)
        {
            try
            {
                var feedback = await _feedbackUseCase.CriarFeedbackAsync(request);
                return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedback);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao criar feedback", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedback(string id, [FromBody] FeedbackRequest request)
        {
            try
            {
                var feedback = await _feedbackUseCase.AtualizarFeedbackAsync(id, request);
                return Ok(feedback);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Feedback não encontrado" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao atualizar feedback", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(string id)
        {
            try
            {
                await _feedbackUseCase.DeletarFeedbackAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao excluir feedback", details = ex.Message });
            }
        }

        [HttpGet("verificar/{encontroId}/{usuarioId}")]
        public async Task<ActionResult<bool>> VerificarFeedbackExistente(string encontroId, string usuarioId)
        {
            try
            {
                var existe = await _feedbackUseCase.VerificarFeedbackExistenteAsync(encontroId, usuarioId);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao verificar feedback", details = ex.Message });
            }
        }
    }
}
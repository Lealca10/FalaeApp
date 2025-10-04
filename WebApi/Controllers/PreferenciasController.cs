using Application.Request;
using Application.UseCases;
using Application.UsesCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreferenciasController : ControllerBase
    {
        private readonly IPreferenciasUseCase _preferenciasUseCase;

        public PreferenciasController(IPreferenciasUseCase preferenciasUseCase)
        {
            _preferenciasUseCase = preferenciasUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> SalvarPreferencias([FromBody] QuestionarioPreferenciasRequest request)
        {
            try
            {
                var result = await _preferenciasUseCase.SalvarPreferencias(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObterPreferencias(string usuarioId)
        {
            try
            {
                var result = await _preferenciasUseCase.ObterPreferencias(usuarioId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
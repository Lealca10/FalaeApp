using Microsoft.AspNetCore.Mvc;
using Application.Request;
using Application.UsesCases;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioUseCase _usuarioUseCase;

        public UsuariosController(IUsuarioUseCase usuarioUseCase)
        {
            _usuarioUseCase = usuarioUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroUsuarioRequest request)
        {
            try
            {
                var result = await _usuarioUseCase.CadastrarUsuario(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
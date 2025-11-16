using Microsoft.AspNetCore.Mvc;
using Application.Request;
using Application.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioUseCase _usuarioUseCase;

        public AuthController(IUsuarioUseCase usuarioUseCase)
        {
            _usuarioUseCase = usuarioUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request não pode ser nulo" });

            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Email obrigatório" });

            if (string.IsNullOrEmpty(request.Senha))
                return BadRequest(new { message = "Senha obrigatória" });

            var result = await _usuarioUseCase.Login(request);
            return Ok(result);
        }

        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaRequest request)
        {
            try
            {
                var result = await _usuarioUseCase.RecuperarSenha(request.Email);
                if (result)
                    return Ok(new { mensagem = "E-mail de recuperação enviado com sucesso." });

                return NotFound(new { mensagem = "E-mail não encontrado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

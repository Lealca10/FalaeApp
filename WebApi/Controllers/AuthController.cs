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

        [HttpPost("alterar-senha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
        {
            try
            {
                var result = await _usuarioUseCase.AlterarSenha(request.Email, request.SenhaAtual, request.NovaSenha);

                if (result)
                    return Ok(new { mensagem = "Senha alterada com sucesso" });

                return BadRequest(new { mensagem = "Não foi possível alterar a senha" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaRequest request)
        {
            try
            {
                var result = await _usuarioUseCase.RecuperarSenha(request);
                if (result)
                    return Ok(new { mensagem = "Senha alterada com sucesso." });

                return BadRequest(new { mensagem = "Não foi possível alterar a senha." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

namespace Application.Request
{
    public class AlterarSenhaRequest
    {
        public string Email { get; set; } = null!;
        public string SenhaAtual { get; set; } = null!;
        public string NovaSenha { get; set; } = null!;
    }
}

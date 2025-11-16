using Application.Request;
using Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUsuarioUseCase
    {
        Task<UsuarioResponse> CadastrarUsuario(CadastroUsuarioRequest request);
        Task<LoginResponse> Login(LoginRequest request);
        Task<bool> RecuperarSenha(RecuperarSenhaRequest request);
        Task<bool> AlterarSenha(string email, string senhaAtual, string novaSenha);
    }
}

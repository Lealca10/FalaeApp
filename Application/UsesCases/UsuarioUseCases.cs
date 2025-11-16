using Application.Request;
using Application.Response;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;

namespace Application.UseCases
{
    public class UsuarioUseCase : IUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public UsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IJwtService jwtService,
            IPasswordService passwordService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        public async Task<UsuarioResponse> CadastrarUsuario(CadastroUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                throw new Exception("Email e senha são obrigatórios");

            // Valida se o email já existe
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuarioExistente != null)
                throw new Exception("Email já cadastrado");

            var usuario = new UsuarioDomain
            {
                Nome = request.Nome,
                Cpf = request.Cpf,
                DataNascimento = request.DataNascimento,
                Cidade = request.Cidade,
                Email = request.Email,
                Senha = request.Senha // senha em texto claro
            };

            await _usuarioRepository.AddAsync(usuario);

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Idade = CalcularIdade(usuario.DataNascimento),
                Cidade = usuario.Cidade
            };
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                throw new Exception("Email e senha são obrigatórios");

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null)
                throw new Exception("Credenciais inválidas");

            if (string.IsNullOrEmpty(usuario.Senha))
                throw new Exception("Usuário não possui senha cadastrada");

            // Comparação direta da senha em texto claro
            if (usuario.Senha != request.Senha)
                throw new Exception("Credenciais inválidas");

            var token = _jwtService.GenerateToken(usuario);

            return new LoginResponse
            {
                Token = token,
                Usuario = new UsuarioResponse
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Idade = CalcularIdade(usuario.DataNascimento),
                    Cidade = usuario.Cidade
                }
            };
        }

        public async Task<bool> RecuperarSenha(RecuperarSenhaRequest request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            // Aqui você deve validar o código. Por enquanto, vamos aceitar qualquer código "0000"
            if (request.Codigo != "0000")
                throw new Exception("Código inválido");

            // Atualiza a senha
            usuario.Senha = request.NovaSenha; // se quiser, ainda pode criptografar aqui
            await _usuarioRepository.UpdateAsync(usuario);

            return true;
        }

        public async Task<bool> AlterarSenha(string email, string senhaAtual, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(novaSenha))
                throw new Exception("Email e nova senha são obrigatórios");

            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            // Se quiser validar a senha atual, descomente esta parte
            if (!string.IsNullOrEmpty(senhaAtual) && usuario.Senha != senhaAtual)
                throw new Exception("Senha atual inválida");

            usuario.Senha = novaSenha; // Atualiza para a nova senha

            await _usuarioRepository.UpdateAsync(usuario); // Certifique-se que o método UpdateAsync existe no seu repository

            return true;
        }

        private int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;
            if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }
}

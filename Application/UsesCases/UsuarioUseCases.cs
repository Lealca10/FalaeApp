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

        public async Task<bool> RecuperarSenha(string email)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario != null)
            {
                // aqui entraria lógica real de envio de email
                return true;
            }

            return false;
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

using Application.Request;
using Application.Response;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;

namespace Application.UseCases
{
    public interface IUsuarioUseCase
    {
        Task<UsuarioResponse> CadastrarUsuario(CadastroUsuarioRequest request);
        Task<LoginResponse> Login(LoginRequest request);
        Task<bool> RecuperarSenha(string email);
    }

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
            // Validar se email já existe
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
                Senha = _passwordService.HashPassword(request.Senha)
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
            // Log para debug
            Console.WriteLine($"Login attempt for email: {request.Email}");

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null)
            {
                Console.WriteLine($"Usuário não encontrado para email: {request.Email}");
                throw new Exception("Credenciais inválidas");
            }

            Console.WriteLine($"Usuário encontrado: {usuario.Nome}, Verificando senha...");

            var isPasswordValid = _passwordService.VerifyPassword(request.Senha, usuario.Senha);

            if (!isPasswordValid)
            {
                Console.WriteLine($"Senha inválida para usuário: {usuario.Email}");
                throw new Exception("Credenciais inválidas");
            }

            Console.WriteLine($"Login bem-sucedido para: {usuario.Email}");
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
                // Implementar lógica de envio de email
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

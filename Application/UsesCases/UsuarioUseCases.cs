using Application.Request;
using Application.Response;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Application.UsesCases
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

        public UsuarioUseCase(IUsuarioRepository usuarioRepository, IJwtService jwtService, IPasswordService passwordService)
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
                Idade = DateTime.Now.Year - usuario.DataNascimento.Year,
                Cidade = usuario.Cidade
            };
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null || !_passwordService.VerifyPassword(request.Senha, usuario.Senha))
                throw new Exception("Credenciais inválidas");

            var token = _jwtService.GenerateToken(usuario);

            return new LoginResponse
            {
                Token = token,
                Usuario = new UsuarioResponse
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Idade = DateTime.Now.Year - usuario.DataNascimento.Year,
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
    }
}

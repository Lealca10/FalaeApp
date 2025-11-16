using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DatabaseContext _context;

        public UsuarioRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<UsuarioDomain> GetByIdAsync(string id)
        {
            return await _context.Usuarios
                .Include(u => u.Preferencias)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        //public async Task<UsuarioDomain> GetByEmailAsync(string email)
        //{
          //  return await _context.Usuarios
            //    .FirstOrDefaultAsync(u => u.Email == email);
        //}

        public async Task<UsuarioDomain> GetByEmailAsync(string email)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            // LOG CRÍTICO
            Console.WriteLine($"=== REPOSITORY DEBUG ===");
            Console.WriteLine($"Usuário: {usuario?.Nome}");
            Console.WriteLine($"Email: {usuario?.Email}");
            Console.WriteLine($"Senha PROPRIEDADE: {usuario?.Senha}");
            Console.WriteLine($"Senha is NULL: {usuario?.Senha == null}");
            Console.WriteLine($"Todas as propriedades:");
            Console.WriteLine($"  Id: {usuario?.Id}");
            Console.WriteLine($"  Nome: {usuario?.Nome}");
            Console.WriteLine($"  Email: {usuario?.Email}");
            Console.WriteLine($"  Senha: {usuario?.Senha}");
            Console.WriteLine($"  Cidade: {usuario?.Cidade}");
            Console.WriteLine($"  Ativo: {usuario?.Ativo}");
            Console.WriteLine($"=== FIM DEBUG ===");

            return usuario;
        }

        public async Task<IEnumerable<UsuarioDomain>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Preferencias)
                .Where(u => u.Ativo)
                .ToListAsync();
        }

        public async Task AddAsync(UsuarioDomain usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UsuarioDomain usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var usuario = await GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Ativo = false;
                await UpdateAsync(usuario);
            }
        }
    }
}
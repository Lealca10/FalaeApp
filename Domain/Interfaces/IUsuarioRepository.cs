using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioDomain> GetByIdAsync(string id);
        Task<UsuarioDomain> GetByEmailAsync(string email);
        Task<IEnumerable<UsuarioDomain>> GetAllAsync();
        Task AddAsync(UsuarioDomain usuario);
        Task UpdateAsync(UsuarioDomain usuario);
        Task DeleteAsync(string id);
    }
}

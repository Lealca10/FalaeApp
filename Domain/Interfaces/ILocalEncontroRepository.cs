using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILocalEncontroRepository
    {
        Task<LocalEncontroDomain> GetByIdAsync(string id);
        Task<IEnumerable<LocalEncontroDomain>> GetAllAsync();
        Task AddAsync(LocalEncontroDomain local);
        Task UpdateAsync(LocalEncontroDomain local);
        Task DeleteAsync(string id);
    }
}

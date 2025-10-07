using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPreferenciasRepository
    {
        Task<PreferenciasUsuarioDomain> GetByUsuarioIdAsync(string usuarioId);
        Task AddAsync(PreferenciasUsuarioDomain preferencias);
        Task UpdateAsync(PreferenciasUsuarioDomain preferencias);
    }
}

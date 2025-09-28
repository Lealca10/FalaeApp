using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UsesCases
{
    public class ObterFaturasUseCase : IConsultarFaturaEntidade
    {
        private readonly IConsultarFaturaEntidade _repositorio;

        public ObterFaturasUseCase(IConsultarFaturaEntidade repositorio)
        {
            _repositorio = repositorio;
        }

        public List<FaturaDomain> Executar()
        {
            return _repositorio.ObterTodasFaturas();
        }

        public List<FaturaDomain> ObterTodasFaturas()
        {
            throw new NotImplementedException();
        }
    }
}

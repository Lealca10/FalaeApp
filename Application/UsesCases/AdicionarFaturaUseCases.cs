using Application.Interfaces;
using Application.Request;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UsesCases
{
    public class AdicionarFaturaUseCases : IAdicionarFaturaUseCase
    {
        private readonly IAdicionarFaturaEntidade AdicionarFaturaEntidade;

        public AdicionarFaturaUseCases(IAdicionarFaturaEntidade adicionarFaturaEntidade)
        {
            AdicionarFaturaEntidade = adicionarFaturaEntidade;
        }

        public void AdicionarFatura(FaturaRequest fatura)
        {
            FaturaDomain faturaD = new FaturaDomain(fatura.descricao,
                                                    fatura.data,
                                                    fatura.valor,
                                                    fatura.categoria
                                                    );

            AdicionarFaturaEntidade.AdicionarFaturaDomain( faturaD );
        }
    }
}

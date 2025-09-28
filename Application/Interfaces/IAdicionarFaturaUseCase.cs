using Application.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdicionarFaturaUseCase
    {
        public void AdicionarFatura(FaturaRequest faturaRequest);
    }
}

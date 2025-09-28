using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FaturaDomain
    {
        public string descricao { get; private set; }
        public DateTime data { get; private set; }
        public double valor { get; private set; }
        public string categoria { get; private set; }

        public FaturaDomain() { }

        public FaturaDomain(string descricao, DateTime data, double valor, string categoria)
        {
            this.descricao = descricao;
            this.data = data;
            this.valor = valor;
            this.categoria = categoria;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class FaturaRequest
    {

        public string descricao { get; set; }
        public DateTime data { get; set; } 
        public double valor { get; set; }
        public string categoria { get; set; }   

        public FaturaRequest(string descricao, DateTime data, double valor, string categoria)
        {
            this.descricao = descricao;
            this.data = data;
            this.valor = valor;
            this.categoria = categoria;
        }
    }
}

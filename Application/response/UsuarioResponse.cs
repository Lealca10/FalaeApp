using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class UsuarioResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Cidade { get; set; } = string.Empty;
    }
}

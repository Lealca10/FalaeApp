using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.response
{
    public class MatchingResult
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public List<UsuarioInfo> ParticipantesSugeridos { get; set; } = new();
        public int PreferenciasCompatíveis { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.response
{
    public class EncontroResponse
    {
        public string Id { get; set; } = string.Empty;
        public string LocalId { get; set; } = string.Empty;
        public LocalEncontroInfo Local { get; set; } = new();
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = "agendado";
        public DateTime DataCriacao { get; set; }
        public List<UsuarioInfo> Participantes { get; set; } = new();
        public int TotalParticipantes { get; set; }
    }

    public class LocalEncontroInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int Capacidade { get; set; }
    }

    public class UsuarioInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UsuarioDomain
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;

        // Navigation properties
        public PreferenciasUsuarioDomain Preferencias { get; set; }
        public ICollection<EncontroDomain> Encontros { get; set; } = new List<EncontroDomain>();
        public ICollection<FeedbackEncontroDomain> Feedbacks { get; set; } = new List<FeedbackEncontroDomain>();
    }
}

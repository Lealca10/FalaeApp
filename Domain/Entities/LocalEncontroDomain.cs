using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class LocalEncontroDomain
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int Capacidade { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<EncontroDomain> Encontros { get; set; } = new List<EncontroDomain>();
    }
}

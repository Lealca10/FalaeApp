using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PreferenciasUsuarioDomain
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UsuarioId { get; set; } = string.Empty;
        public List<string> Interesses { get; set; } = new();
        public string FaixaEtariaPreferida { get; set; } = string.Empty;
        public string LocalizacaoPreferida { get; set; } = string.Empty;
        public List<string> Hobbies { get; set; } = new();

        // Gostos pessoais como JSON ou propriedades separadas
        public string GostosPessoaisJson { get; set; } = string.Empty;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        // Navigation property
        public UsuarioDomain Usuario { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FeedbackEncontroDomain
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EncontroId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public EncontroDomain Encontro { get; set; }
        public UsuarioDomain Usuario { get; set; }
    }
}

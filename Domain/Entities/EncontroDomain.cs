using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class EncontroDomain
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string LocalId { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = "agendado"; // agendado, realizado, cancelado
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public LocalEncontroDomain Local { get; set; }
        public ICollection<UsuarioDomain> Participantes { get; set; } = new List<UsuarioDomain>();
        public ICollection<FeedbackEncontroDomain> Feedbacks { get; set; } = new List<FeedbackEncontroDomain>();
    }
}

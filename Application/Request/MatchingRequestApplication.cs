using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class MatchingRequestApplication
    {
        public string LocalId { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public int MinimoPreferenciasIguais { get; set; } = 8;
        public int NumeroParticipantes { get; set; } = 5;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class QuestionarioPreferenciasRequest
    {
        public string UsuarioId { get; set; } = string.Empty;

        public string HorarioFavorito { get; set; } = string.Empty;
        public string TipoComidaFavorito { get; set; } = string.Empty;
        public int NivelEstresse { get; set; }
        public bool GostaViajar { get; set; }
        public string PreferenciaLocal { get; set; } = string.Empty;
        public string PreferenciaAmbiente { get; set; } = string.Empty;
        public int ImportanciaEspiritualidade { get; set; }
        public string PosicaoPolitica { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string PreferenciaMusical { get; set; } = string.Empty;
        public string MoodFilmesSeries { get; set; } = string.Empty;
        public string StatusRelacionamento { get; set; } = string.Empty;
        public bool TemFilhos { get; set; }
        public string PreferenciaAnimal { get; set; } = string.Empty;
        public string FraseDefinicao { get; set; } = string.Empty;
    }
}

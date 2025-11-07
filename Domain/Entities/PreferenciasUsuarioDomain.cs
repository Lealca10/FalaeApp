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
        public string HorarioFavorito { get; set; } = string.Empty; // Manhã, Tarde, Noite, Madrugada
        public string TipoComidaFavorito { get; set; } = string.Empty; // Italiana, Mexicana, etc.
        public int NivelEstresse { get; set; } // 1-10
        public bool GostaViajar { get; set; }
        public string PreferenciaLocal { get; set; } = string.Empty; // Cidade, Campo
        public string PreferenciaAmbiente { get; set; } = string.Empty; // Praia, Montanha
        public int ImportanciaEspiritualidade { get; set; } // 1-10
        public string PosicaoPolitica { get; set; } = string.Empty; // Direita, Centro-Direita, etc.
        public string Genero { get; set; } = string.Empty; // Mulher, Homem, Não-Binário
        public string PreferenciaMusical { get; set; } = string.Empty; // Rock, Pop, etc.
        public string MoodFilmesSeries { get; set; } = string.Empty; // Maratonador, Gosta de Estreias, etc.
        public string StatusRelacionamento { get; set; } = string.Empty; // Casado(a), Solteiro(a), etc.
        public bool TemFilhos { get; set; }
        public string PreferenciaAnimal { get; set; } = string.Empty; // Cão, Gato
        public string FraseDefinicao { get; set; } = string.Empty; // Mantra ou frase que define
        public string IdiomaPreferido { get; set; } = string.Empty; // Português, Espanhol, Inglês
        public string InvestimentoEncontro { get; set; } = string.Empty; // Pouco, Médio, Alto

        // Gostos pessoais como JSON ou propriedades separadas
        public string GostosPessoaisJson { get; set; } = string.Empty;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        // Navigation property
        public UsuarioDomain Usuario { get; set; }
    }
}

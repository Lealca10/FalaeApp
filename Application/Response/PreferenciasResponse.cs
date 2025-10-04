using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class PreferenciasResponse
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DataAtualizacao { get; set; }

        // Opcional: retornar os dados salvos
        public string HorarioFavorito { get; set; } = string.Empty;
        public string TipoComidaFavorito { get; set; } = string.Empty;
        public string PreferenciaMusical { get; set; } = string.Empty;
    }
}

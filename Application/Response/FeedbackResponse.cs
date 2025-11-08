using Application.response;

namespace Application.Response
{
    public class FeedbackResponse
    {
        public string Id { get; set; } = string.Empty;
        public string EncontroId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }

        // Informações do usuário
        public UsuarioInfo Usuario { get; set; } = new();

        // Informações do encontro (opcional)
        public EncontroInfo Encontro { get; set; } = new();
    }

    public class EncontroInfo
    {
        public string Id { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = string.Empty;
        public LocalEncontroInfo Local { get; set; } = new();
    }
}
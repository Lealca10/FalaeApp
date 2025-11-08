namespace Application.Request
{
    public class FeedbackRequest
    {
        public string EncontroId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
namespace Application.Request
{
    public class CriarEncontroComUsuarioRequest
    {
        public string LocalId { get; set; }
        public DateTime DataHora { get; set; }
        public int MinimoPreferenciasIguais { get; set; }
        public int NumeroParticipantes { get; set; }
    }
}

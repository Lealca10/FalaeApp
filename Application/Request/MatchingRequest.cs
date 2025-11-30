namespace Application.Request
{
    public class MatchingRequest
    {
        public string LocalId { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public int MinimoPreferenciasIguais { get; set; } = 8;
        public int NumeroParticipantes { get; set; } = 5;
    }
}

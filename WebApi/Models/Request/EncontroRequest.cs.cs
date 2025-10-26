namespace WebApi.Models.Request
{
    public class EncontroRequest
    {
        public string LocalId { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public int MinimoPreferenciasIguais { get; set; } = 8;
    }
}

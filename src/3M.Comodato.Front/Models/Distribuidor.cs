namespace _3M.Comodato.Front.Models
{
    public class Distribuidor
    {
        public string NomeDI { get; set; }
        public string Modelo { get; set; }
        public int MesesPrevistos { get; set; }
        public int MesesAlocado { get; set; }
        public decimal ValorAtivo { get; set; }
        public decimal ValorAluguel { get; set; }
        public string Status { get; set; }
        public string NR_NOTA { get; set; }
        public string VL_NOTA { get; set; }
    }
}
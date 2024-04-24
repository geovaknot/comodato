namespace _3M.Comodato.API.Models
{
    public class EstoquePecaBase
    {
        public long ID_ESTOQUE { get; set; }
        public long ID_ESTOQUE_PECA { get; set; }
        public string CD_PECA { get; set; }
        public string DS_PECA { get; set; }
        public string TX_UNIDADE { get; set; }
        public decimal QT_PECA_ATUAL { get; set; }
        public decimal QTD_RECEBIDA_NAO_APROVADA { get; set; }
    }
}
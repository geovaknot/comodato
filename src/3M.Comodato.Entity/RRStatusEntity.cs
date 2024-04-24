namespace _3M.Comodato.Entity
{
    public class RRStatusEntity
    {
        public long ID_RR_STATUS { get; set; }
        public int? NR_ORDEM_FLUXO { get; set; }
        public int ST_STATUS_RR { get; set; }
        public string DS_STATUS_NOME { get; set; }
        public string DS_STATUS_NOME_REDUZ { get; set; }
        public string DS_TRANSICAO { get; set; }
        public string DS_COMENTARIO { get; set; }
        public string DS_STATUS_DESCRICAO { get; set; }
    }
}

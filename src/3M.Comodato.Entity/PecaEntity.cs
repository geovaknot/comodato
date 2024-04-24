using System;

namespace _3M.Comodato.Entity
{
    public class PecaEntity : BaseEntity
    {
        public string CD_PECA { get; set; }
        public string DS_PECA { get; set; }
        public string TX_UNIDADE { get; set; }
        public decimal QTD_ESTOQUE { get; set; }
        public decimal QTD_MINIMA { get; set; }
        public decimal VL_PECA { get; set; }
        public string TP_PECA { get; set; }
        public string FL_ATIVO_PECA { get; set; }
        public string CD_CRITICIDADE_ABC { get; set; }
        public string CD_GRUPO_MODELO { get; set; }
        public long ID_PLANO_ZERO { get; set; }
        public string CD_PECA_RECUPERADA { get; set; }
        public decimal QTD_PlanoZero { get; set; }
    }

}

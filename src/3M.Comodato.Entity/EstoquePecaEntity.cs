using System;

namespace _3M.Comodato.Entity
{
    public class EstoquePecaEntity : BaseEntity
    {
        public long ID_ESTOQUE_PECA { get; set; }
        public PecaEntity peca { get; set; } = new PecaEntity();
        public decimal QT_PECA_ATUAL { get; set; }
        public decimal QT_PECA_MIN { get; set; }
        public decimal QTD_RECEBIDA_NAO_APROVADA { get; set; }
        public DateTime? DT_ULT_MOVIM { get; set; }
        public EstoqueEntity estoque { get; set; } = new EstoqueEntity();
        public string DT_MOVIMENTACAO_AJUSTE_SAIDA { get; set; }
    }

    public class EstoquePecaSinc
    {
        public Int64 ID_ESTOQUE_PECA { get; set; }
        public String CD_PECA { get; set; }
        public Decimal QT_PECA_ATUAL { get; set; }
        public Decimal QT_PECA_DISPONIVEL { get; set; }
        public Decimal QT_PECA_MIN { get; set; }
        public DateTime DT_ULT_MOVIM { get; set; }
        public Int64 ID_ESTOQUE { get; set; }
        public Decimal QTD_RECEBIDA_NAO_APROVADA { get; set; }
        public Int64 CD_CLIENTE { get; set; }
    }


}

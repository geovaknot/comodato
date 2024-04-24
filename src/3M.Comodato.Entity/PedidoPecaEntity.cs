using System;

namespace _3M.Comodato.Entity
{
    public class PedidoPecaEntity : BaseEntity
    {
        public Int64 ID_ITEM_PEDIDO { get; set; }
        public long TOKEN { get; set; }
        public Decimal? QTD_SOLICITADA { get; set; }
        public Decimal? QTD_APROVADA { get; set; }
        public Decimal? QTD_RECEBIDA { get; set; }
        public Decimal? QTD_ULTIMO_RECEBIMENTO { get; set; }
        public string TX_APROVADO { get; set; }
        public Int64 NR_DOC_ORI { get; set; }
        public string ST_STATUS_ITEM { get; set; }
        public string DS_OBSERVACAO { get; set; }
        public string DS_DIR_FOTO { get; set; }
        public Decimal? VL_PECA { get; set; }
        public decimal QTD_APROVADA_3M1 { get; set; }
        public decimal QTD_APROVADA_3M2 { get; set; }
        public Int64 ID_LOTE_APROVACAO { get; set; }
        public Int64 NR_LINHA { get; set; }
        public byte TIPO_PECA { get; set; }
        public string DESCRICAO_PECA { get; set; }
        public string Estoque_Cli_Aprov { get; set; }

        public string Estoque_Tec_Aprov { get; set; }

        public PedidoEntity pedido { get; set; } = new PedidoEntity();
        public PecaEntity peca { get; set; } = new PecaEntity();


        public EstoqueEntity estoque3M1 { get; set; } = new EstoqueEntity();
        public EstoqueEntity estoque3M2 { get; set; } = new EstoqueEntity();
        public string Duplicado { get; set; }
        public string CD_PECA_REFERENCIA { get; set; }

    }

    public class PedidoPecaSinc
    {
        public Int64? ID_ITEM_PEDIDO { get; set; }
        public Int64? ID_PEDIDO { get; set; }
        public long TOKEN { get; set; }
        public String CD_PECA { get; set; }
        public Int64? QTD_SOLICITADA { get; set; }
        public Int64? QTD_APROVADA { get; set; }
        public String TX_APROVADO { get; set; }
        public Int64? NR_DOC_ORI { get; set; }
        public Int64? QTD_RECEBIDA { get; set; }
        public Char ST_STATUS_ITEM { get; set; }
        public String DS_OBSERVACAO { get; set; }
        public String DS_DIR_FOTO { get; set; }
        public Int64? ID_ESTOQUE_DEBITO { get; set; }
        public decimal QTD_APROVADA_3M1 { get; set; }
        public decimal VL_PECA { get; set; }
        public string Estoque_Cli_Aprov { get; set; }

        public string Estoque_Tec_Aprov { get; set; }
        public decimal QTD_APROVADA_3M2 { get; set; }
        public String IDENTIFICADOR_FK_ID_PEDIDO { get; set; }
        public String IDENTIFICADOR_PK_ID_PEDIDO_PECA { get; set; }
        public byte TIPO_PECA { get; set; }
        public string DESCRICAO_PECA { get; set; }
        public string atualizado { get; set; }
        public string Duplicado { get; set; }
        public Int64 QTD_PENDENTE { get; set; }
        public Decimal? QTD_ULTIMO_RECEBIMENTO { get; set; }
        public string CD_PECA_REFERENCIA { get; set; }
    }
}

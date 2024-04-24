using System;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class PedidoPeca : BaseModel
    {
        public Int64 ID_ITEM_PEDIDO { get; set; }

        public long TOKEN { get; set; }

        public string QTD_SOLICITADA { get; set; }

        public string QTD_APROVADA { get; set; }

        public string QTD_RECEBIDA { get; set; }

        public string TX_APROVADO { get; set; }

        public Int64 NR_DOC_ORI { get; set; }

        public string ST_STATUS_ITEM { get; set; }

        public string DS_ST_STATUS_ITEM { get; set; }

        public string DS_OBSERVACAO { get; set; }

        public string DS_DIR_FOTO { get; set; }

        public byte TIPO_PECA { get; set; }

        public string DESCRICAO_PECA { get; set; }

        public PedidoEntity pedido { get; set; } = new PedidoEntity();

        public PecaEntity peca { get; set; } = new PecaEntity();


        //public EstoqueEntity estoque { get; set; } = new EstoqueEntity();

        public EstoqueEntity estoque3M1 { get; set; } = new EstoqueEntity();
        public EstoqueEntity estoque3M2 { get; set; } = new EstoqueEntity();

        public string QT_PECA_ATUAL { get; set; }

        public string QT_SUGERIDA_PZ { get; set; }

        public string QTD_APROVADA_3M1 { get; set; }
        public string QTD_APROVADA_3M2 { get; set; }

        public string ID_LOTE_APROVACAO { get; set; }

        public Int64? QTD_ESTOQUE_CLIENTE { get; set; }
    }
}
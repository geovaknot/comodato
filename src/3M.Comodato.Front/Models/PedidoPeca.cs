using _3M.Comodato.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class PedidoPeca : BaseModel
    {
        //PedidoEntity _PedidoEntity = null;
        //PecaEntity _PecaEntity = null;
        //EstoqueEntity _EstoqueEntity = null;

        public Int64 ID_ITEM_PEDIDO { get; set; }

        public string QTD_SOLICITADA { get; set; }

        public string Estoque_Cli_Aprov { get; set; }

        public string Estoque_Tec_Aprov { get; set; }

        public string QTD_APROVADA { get; set; }

        public string QTD_RECEBIDA { get; set; }

        public string QTD_ULTIMO_RECEBIMENTO { get; set; }

        public string TX_APROVADO { get; set; }

        public Int64 NR_DOC_ORI { get; set; }

        public string ST_STATUS_ITEM { get; set; }

        public string DS_ST_STATUS_ITEM { get; set; }

        //public Decimal VL_PECA { get; set; }
        public string VL_TOTAL_PECA { get; set; }

        public string VL_TOTAL_PECA_SOLICITADA { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_OBSERVACAO { get; set; }

        public string DS_DIR_FOTO { get; set; }

        public PedidoEntity pedido { get; set; } = new PedidoEntity();
        public PecaEntity peca { get; set; } = new PecaEntity();

        //public EstoqueEntity estoque { get; set; } = new EstoqueEntity();
        public EstoqueEntity Estoque3M1 { get; set; } = new EstoqueEntity();
        public EstoqueEntity Estoque3M2 { get; set; } = new EstoqueEntity();

        public string QTD_APROVADA_3M1 { get;  set; }
        public string QTD_APROVADA_3M2 { get;  set; }

        public Int64 ID_LOTE_APROVACAO { get; set; }
        public Int64 NR_LINHA { get; set; }

        public string InformadoDados { get; set; }
        public string Duplicado { get; set; }

        public string CD_PECA_REFERENCIA { get; set; }
    }
}
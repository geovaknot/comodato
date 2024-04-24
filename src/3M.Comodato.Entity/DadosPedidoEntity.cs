using System;

namespace _3M.Comodato.Entity
{
    public class DadosPedidoEntity : BaseEntity
    {
        public Int64 ID { get; set; }

        public Int64? ID_ITEM_PEDIDO { get; set; }
        public Int64? ID_PEDIDO { get; set; }

        public string CD_PECA { get; set; }

        public Int64? QTD_SOLICITADA { get; set; }

        public Int64? QTD_APROVADA { get; set; }

        public Int64? QTD_APROVADA_3M1 { get; set; }

        public Int64? QTD_APROVADA_3M2 { get; set; }

        public Int64? ID_ESTOQUE_DEBITO { get; set; }

        public Int64? VOLUME { get; set; }

        public string DS_APROVADOR { get; set; }

        public string DS_TELEFONE { get; set; }

        public Int64? RAMAL { get; set; }

        public Int64? ID_ESTOQUE_DEBITO_3M2 { get; set; }

        
        public string RESP_CLIENTE { get; set; }
        public int? PesoLiquido { get; set; }
        public int? PesoBruto { get; set; }
        public string pecasLote { get; set; }

        public Int64? NR_REMESSA { get; set; }
    }
}

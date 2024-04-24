using System;

namespace _3M.Comodato.Entity
{
    public class PedidoPecaLogEntity : BaseEntity
    {
        public Int64 ID_ITEM_PEDIDO_LOG { get; set; }
        public Int64? ID_ITEM_PEDIDO { get; set; }
        public Int64? QTD_PECA_RECEBIDA { get; set; }
        public DateTime? DATA_RECEBIMENTO { get; set; }
        public PedidoPecaEntity pedidopeca { get; set; } = new PedidoPecaEntity();

    }

    public class PedidoPecaLogSinc
    {
        public Int64? ID_ITEM_PEDIDO_LOG { get; set; }
        public Int64 ID_ITEM_PEDIDO { get; set; }
        public Int64? QTD_PECA_RECEBIDA { get; set; }
        public DateTime? DATA_RECEBIMENTO { get; set; }
    }
}


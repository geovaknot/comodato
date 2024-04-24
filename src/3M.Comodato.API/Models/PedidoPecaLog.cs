using System;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class PedidoPecaLog : BaseModel
    {
        public Int64 ID_ITEM_PEDIDO_LOG { get; set; }
        public Int64? ID_ITEM_PEDIDO { get; set; }
        public Int64? QTD_PECA_RECEBIDA { get; set; }
        public DateTime? DATA_RECEBIMENTO { get; set; }

        public PedidoPecaEntity pedidopeca { get; set; } = new PedidoPecaEntity();

        
    }
}
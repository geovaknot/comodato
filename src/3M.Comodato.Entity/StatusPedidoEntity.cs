using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class StatusPedidoEntity : BaseEntity
    {
        public Int64 ID_STATUS_PEDIDO { get; set; }
        public string DS_STATUS_PEDIDO { get; set; }
        public string DS_STATUS_PEDIDO_ACAO { get; set; }

        public Int64 ID_STATUS_PEDIDO_ATUAL { get; set; }
    }

    public class StatusPedidoSincEntity 
    {
        public Int64 ID_STATUS_PEDIDO { get; set; }
        public string DS_STATUS_PEDIDO { get; set; }
        public string DS_STATUS_PEDIDO_ACAO { get; set; }
    }

}

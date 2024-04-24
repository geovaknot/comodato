using System;

namespace _3M.Comodato.Entity
{
    public class WfPedidoEquipItemEntity : BaseEntity
    {
        public long ID_WF_PEDIDO_EQUIP_ITEM { get; set; }
        public string CD_ATIVO_FIXO { get; set; }
        public long ID_WF_PEDIDO_EQUIP { get; set; }
        public string DS_ANEXO { get; set; }
        public string ST_STATUS { get; set; }

        public ModeloEntity Modelo { get; set; } = new ModeloEntity();
        public AtivoClienteEntity Ativo { get; set; } = new AtivoClienteEntity();

        public WfPedidoEquipEntity PedidoEquip { get; set; }  = new WfPedidoEquipEntity();

        //public DateTime DT_RETIRADA { get; set; }
        //public DateTime DT_ENTREGA_3M { get; set; }
    }
}

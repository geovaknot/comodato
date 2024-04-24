using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class WfPedidoEquipLogEntity: BaseEntity
    {
        private WfPedidoEquipEntity _WfPedidoEquipEntity = null;
        private WfStatusPedidoEquipEntity _WfStatusPedidoEquipEntity = null;

        public Int64 ID_WF_PEDIDO_EQUIP_LOG { get; set; }
        public string CD_GRUPO_RESPONS { get; set; }

        public WfPedidoEquipEntity pedidoEquip
        {
            get
            {
                if (_WfPedidoEquipEntity == null) _WfPedidoEquipEntity = new WfPedidoEquipEntity();
                return _WfPedidoEquipEntity;
            }
            set
            {
                if (_WfPedidoEquipEntity == null) _WfPedidoEquipEntity = new WfPedidoEquipEntity();
                _WfPedidoEquipEntity = value;
            }
        }

        public WfStatusPedidoEquipEntity statusPedidoEquip
        {
            get
            {
                if (_WfStatusPedidoEquipEntity == null) _WfStatusPedidoEquipEntity = new WfStatusPedidoEquipEntity();
                return _WfStatusPedidoEquipEntity;
            }
            set
            {
                if (_WfStatusPedidoEquipEntity == null) _WfStatusPedidoEquipEntity = new WfStatusPedidoEquipEntity();
                _WfStatusPedidoEquipEntity = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class WfPedidoComentEntity: BaseEntity
    {
        UsuarioEntity _UsuarioEntity = null;
        WfPedidoEquipEntity _WfPedidoEquipEntity = null;

        public long ID_WF_PEDIDO_COMENT { get; set; }
        //public long ID_WF_PEDIDO_EQUIP { get; set; }
        public string DS_COMENT { get; set; }

        public UsuarioEntity usuario
        {
            get
            {
                if (_UsuarioEntity == null) _UsuarioEntity = new UsuarioEntity();
                return _UsuarioEntity;
            }
            set
            {
                if (_UsuarioEntity == null) _UsuarioEntity = new UsuarioEntity();
                _UsuarioEntity = value;
            }
        }

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
    }
}

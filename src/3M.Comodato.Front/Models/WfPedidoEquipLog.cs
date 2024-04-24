using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class WfPedidoEquipLog : BaseModel
    {
        UsuarioEntity _UsuarioEntity = null;
        WfStatusPedidoEquipEntity _WfStatusPedidoEquipEntity = null;

        public Int64 ID_WF_PEDIDO_EQUIP_LOG { get; set; }
        public Int64 ID_WF_PEDIDO_EQUIP { get; set; }

        public string DT_REGISTRO { get; set; }

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

        public WfStatusPedidoEquipEntity wfStatusPedidoEquip
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
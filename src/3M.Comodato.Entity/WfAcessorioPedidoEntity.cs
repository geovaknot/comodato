using System;

namespace _3M.Comodato.Entity
{
    public class WfAcessorioPedidoEntity : BaseEntity
    {

        #region "Private Properties"
        
        #endregion

        #region "Public Properties"
        public Int64 ID_WF_ACESSORIO_EQUIP { get; set; }
        public Int64 ID_WF_PEDIDO_EQUIP { get; set; }
        public String CD_MODELO { get; set; }
        public Int64 QTD_SOLICITADO { get; set; }

        #endregion
    }
}

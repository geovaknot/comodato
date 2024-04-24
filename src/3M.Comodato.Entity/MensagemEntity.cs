using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class MensagemEntity : BaseEntity
    {
        private PedidoEntity _PedidoEntity = null;
        private UsuarioEntity _UsuarioEntity = null;

        public Int64 ID_MENSAGEM { get; set; }
        public DateTime? DT_OCORRENCIA { get; set; }
        public string DS_MENSAGEM { get; set; }

        public PedidoEntity pedido
        {
            get
            {
                if (_PedidoEntity == null) _PedidoEntity = new PedidoEntity();
                return _PedidoEntity;
            }
            set
            {
                if (_PedidoEntity == null) _PedidoEntity = new PedidoEntity();
                _PedidoEntity = value;
            }
        }

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
    }
}

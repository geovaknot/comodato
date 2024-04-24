using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Mensagem : BaseModel
    {
        private PedidoEntity _PedidoEntity = null;
        private UsuarioEntity _UsuarioEntity = null;

        public Int64 ID_MENSAGEM { get; set; }
        public string DT_OCORRENCIA { get; set; }
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
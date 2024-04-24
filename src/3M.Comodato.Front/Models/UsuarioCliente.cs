using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class UsuarioCliente : BaseModel
    {
        private ClienteEntity _Cliente = null;
        private UsuarioEntity _Usuario = null;

        public Int64 nidUsuarioCliente { get; set; }

        public List<Cliente> clientes { get; set; }
        public List<Usuario> usuarios { get; set; }

        public ClienteEntity cliente
        {
            get
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                return _Cliente;
            }
            set
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                _Cliente = value;
            }
        }

        public UsuarioEntity usuario
        {
            get
            {
                if (_Usuario == null) _Usuario = new UsuarioEntity();
                return _Usuario;
            }
            set
            {
                if (_Usuario == null) _Usuario = new UsuarioEntity();
                _Usuario = value;
            }
        }

    }

    public class UsuarioClienteDetalhe : UsuarioCliente
    {
        public int? nvlQtdeUsuarios { get; set; }

    }

    public class ListaUsuarioCliente : BaseModel
    {
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public Int64 nvlQtdeUsuarios { get; set; }
    }

    public class ListaUsuarioClienteSelecinado : BaseModel
    {
        private ClienteEntity _Cliente = null;
        private UsuarioEntity _Usuario = null;

        public Int64 nidUsuarioCliente { get; set; }

        public ClienteEntity cliente
        {
            get
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                return _Cliente;
            }
            set
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                _Cliente = value;
            }
        }

        public UsuarioEntity usuario
        {
            get
            {
                if (_Usuario == null) _Usuario = new UsuarioEntity();
                return _Usuario;
            }
            set
            {
                if (_Usuario == null) _Usuario = new UsuarioEntity();
                _Usuario = value;
            }
        }

    }

}
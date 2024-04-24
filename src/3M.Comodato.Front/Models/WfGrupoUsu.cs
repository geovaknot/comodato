using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    public class WfGrupoUsu : BaseModel
    {

        private WfGrupoEntity _WFGrupo = null;
        private UsuarioEntity _Usuario = null;

        public Int64 ID_GRUPOWF_USU { get; set; }
        public Int64 ID_GRUPOWF { get; set; }
        public string CD_GRUPOWF { get; set; }
        public Int64 ID_USUARIO { get; set; }
        public Int32 NM_PRIORIDADE { get; set; }

        public List<WfGrupo> gruposwf { get; set; }
        public List<Usuario> usuarios { get; set; }
        public Dictionary<int, int> Prioridade { get; set; }
        public SelectList ListaTipoGrupo => new SelectList(ControlesUtility.Dicionarios.TipoPedidoWorkflow(), "key", "value");

        public WfGrupoEntity wfGrupo
        {
            get
            {
                if (_WFGrupo == null) _WFGrupo = new WfGrupoEntity();
                return _WFGrupo;
            }
            set
            {
                if (_WFGrupo == null) _WFGrupo = new WfGrupoEntity();
                _WFGrupo = value;
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

    public class ListaUsuarioXWFGrupo : BaseModel
    {
        public Int64 ID_GRUPOWF_USU { get; set; }
        public Int64 ID_GRUPOWF { get; set; }
        public string CD_GRUPOWF { get; set; }
        public Int64 ID_USUARIO { get; set; }
        public string cnmNome { get; set; }
        public Int32 NM_PRIORIDADE { get; set; }
    }
}
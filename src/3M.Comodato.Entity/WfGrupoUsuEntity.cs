using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class WfGrupoUsuEntity : BaseEntity
    {
        private UsuarioEntity _UsuarioEntity = null;
        private WfGrupoEntity _WFGrupoEntity = null;

        public Int64 ID_GRUPOWF_USU { get; set; }
        public int NM_PRIORIDADE { get; set; }

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

        public WfGrupoEntity grupoWf
        {
            get
            {
                if (_WFGrupoEntity == null) _WFGrupoEntity = new WfGrupoEntity();
                return _WFGrupoEntity;
            }
            set
            {
                if (_WFGrupoEntity == null) _WFGrupoEntity = new WfGrupoEntity();
                _WFGrupoEntity = value;
            }
        }
    }
}

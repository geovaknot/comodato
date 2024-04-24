using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    
    public class PerfilFuncaoEntity : BaseEntity
    {
        private PerfilEntity _PerfilEntity = null;
        private FuncaoEntity _FuncaoEntity = null;

        public Int64 nidPerfilFuncao { get; set; }
        

        public PerfilEntity perfil
        {
            get
            {
                if (_PerfilEntity == null) _PerfilEntity = new PerfilEntity();
                return _PerfilEntity;
            }
            set
            {
                if (_PerfilEntity == null) _PerfilEntity = new PerfilEntity();
                _PerfilEntity = value;
            }
        }

        public FuncaoEntity funcao
        {
            get
            {
                if (_FuncaoEntity == null) _FuncaoEntity = new FuncaoEntity();
                return _FuncaoEntity;
            }
            set
            {
                if (_FuncaoEntity == null) _FuncaoEntity = new FuncaoEntity();
                _FuncaoEntity = value;
            }
        }
    }
}

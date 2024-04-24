using System;

namespace _3M.Comodato.Entity
{
    public class UsuarioPerfilEntity : BaseEntity
    {
        private UsuarioEntity _UsuarioEntity = null;
        private PerfilEntity _PerfilEntity = null;
        
        public Int64 nidUsuarioPerfil { get; set; }

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

        //Propriedade especialmente criada para uso no controle de acesso (NÃO REMOVER)
        public bool bidPermitirTrocarSenha { get; set; }

        public string token { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace _3M.Comodato.Entity
{
    public class VendedorEntity : BaseEntity
    {
        private UsuarioEntity _UsuarioGerenteRegionalEntity = null;
        private UsuarioEntity _UsuarioEntity = null;

        public Int64 CD_VENDEDOR { get; set; }
        public string NM_VENDEDOR { get; set; }
        public string NM_APE_VENDEDOR { get; set; }
        public string EN_ENDERECO { get; set; }
        public string EN_BAIRRO { get; set; }
        public string EN_CIDADE { get; set; }
        public string EN_ESTADO { get; set; }
        public string EN_CEP { get; set; }
        public string EN_CX_POSTAL { get; set; }
        public string TX_TELEFONE { get; set; }
        public string TX_FAX { get; set; }
        public string TX_EMAIL { get; set; }
        public Int64 ID_USUARIO { get; set; }
        public Int64 ID_USUARIO_REGIONAL { get; set; }
        //public string FL_ATIVO => !bidAtivo.HasValue ? "" : bidAtivo.Value ? "S" : "N";
        public string FL_ATIVO { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public UsuarioEntity usuarioGerenteRegional
        {
            get
            {
                if (_UsuarioGerenteRegionalEntity == null)
                {
                    _UsuarioGerenteRegionalEntity = new UsuarioEntity();
                }

                return _UsuarioGerenteRegionalEntity;
            }
            set
            {
                if (_UsuarioGerenteRegionalEntity == null)
                {
                    _UsuarioGerenteRegionalEntity = new UsuarioEntity();
                }

                _UsuarioGerenteRegionalEntity = value;
            }
        }

        public UsuarioEntity usuario
        {
            get
            {
                if (_UsuarioEntity == null)
                {
                    _UsuarioEntity = new UsuarioEntity();
                }

                return _UsuarioEntity;
            }
            set
            {
                if (_UsuarioEntity == null)
                {
                    _UsuarioEntity = new UsuarioEntity();
                }

                _UsuarioEntity = value;
            }
        }


    }
}

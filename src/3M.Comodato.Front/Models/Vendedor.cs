using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Vendedor : BaseModel
    {
        private UsuarioEntity _UsuarioGerenteRegionalEntity = null;
        private UsuarioEntity _UsuarioEntity = null;
        public Int64 CD_VENDEDOR { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_VENDEDOR { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string NM_APE_VENDEDOR { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ENDERECO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_BAIRRO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_CIDADE { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ESTADO { get; set; }

        [StringLength(9, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression("[0-9]{5}-[0-9]{3}", ErrorMessage = "Utilize o formato 00000-000.")]
        public string EN_CEP { get; set; }

        [StringLength(9, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_CX_POSTAL { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_TELEFONE { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_FAX { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string TX_EMAIL { get; set; }


        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string FL_ATIVO { get; set; }

        public Dictionary<string, string> SimNao { get; set; }


        public UsuarioEntity usuarioGerenteRegional
        {
            get
            {
                if (_UsuarioGerenteRegionalEntity == null) _UsuarioGerenteRegionalEntity = new UsuarioEntity();
                return _UsuarioGerenteRegionalEntity;
            }
            set
            {
                if (_UsuarioGerenteRegionalEntity == null) _UsuarioGerenteRegionalEntity = new UsuarioEntity();
                _UsuarioGerenteRegionalEntity = value;
            }
        }

        public List<Usuario> usuariosGerentesRegionais { get; set; }

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

        public List<Usuario> usuarios { get; set; }
    }
}
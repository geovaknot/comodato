using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Tecnico : BaseModel
    {
        private UsuarioEntity _UsuarioCoordenadorEntity = null;
        private UsuarioEntity _UsuarioSupervisorEntity = null;

        private UsuarioEntity _UsuarioEntity = null;
        private EmpresaEntity _Empresa = null;

        [StringLength(6, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public String CD_TECNICO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_TECNICO { get; set; }

        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_REDUZIDO { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ENDERECO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_BAIRRO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_CIDADE { get; set; }

        [StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ESTADO { get; set; }

        [StringLength(9, ErrorMessage = "Limite de caracteres ultrapassado!")]
        // [RegularExpression("^[0-9]*$", ErrorMessage = "Digite apenas números.")]
        [RegularExpression("[0-9]{5}-[0-9]{3}", ErrorMessage = "Utilize o formato 00000-000.")]
        public string EN_CEP { get; set; }

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
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string TP_TECNICO { get; set; }

        public string VL_CUSTO_HORA { get; set; }
        public string CD_BCPS { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string FL_ATIVO { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string FL_FERIAS { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string FL_INATIVARESTOQUE { get; set; }

        public Dictionary<string, string> tiposTecnicos { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public List<Peca> pecas { get; set; }

        public string cdsTP_TECNICO { get; set; }

        public string cdsFL_ATIVO { get; set; }

        public string cdsFL_FERIAS { get; set; }

        public UsuarioEntity usuarioCoordenador
        {
            get
            {
                if (_UsuarioCoordenadorEntity == null) _UsuarioCoordenadorEntity = new UsuarioEntity();
                return _UsuarioCoordenadorEntity;
            }
            set
            {
                if (_UsuarioCoordenadorEntity == null) _UsuarioCoordenadorEntity = new UsuarioEntity();
                _UsuarioCoordenadorEntity = value;
            }
        }

        public List<Usuario> usuariosCoordenadores { get; set; }
        public List<Usuario> usuariosSupervisoresTecnico { get; set; }
        
        public UsuarioEntity usuariosSupervisorTecnico
        {
            get
            {
                if (_UsuarioSupervisorEntity == null) _UsuarioSupervisorEntity = new UsuarioEntity();
                return _UsuarioSupervisorEntity;
            }
            set
            {
                if (_UsuarioSupervisorEntity == null) _UsuarioSupervisorEntity = new UsuarioEntity();
                _UsuarioSupervisorEntity = value;
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

        public List<Usuario> usuarios { get; set; }

        public EmpresaEntity empresa
        {
            get
            {
                if (_Empresa == null) _Empresa = new EmpresaEntity();
                return _Empresa;
            }
            set
            {
                if (_Empresa == null) _Empresa = new EmpresaEntity();
                _Empresa = value;
            }
        }

        public List<Empresa> empresas { get; set; }

        public TecnicoEntity tecnicoTransferenciaCarteira { get; set; }
        public List<TecnicoEntity> tecnicosTransferenciaCarteira { get; set; }
    }
}
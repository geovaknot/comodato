using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class Cliente : BaseModel
    {
        private GrupoEntity _GrupoEntity = null;
        private VendedorEntity _VendedorEntity = null;
        private ExecutivoEntity _ExecutivoEntity = null;
        private RegiaoEntity _RegiaoEntity = null;
        private UsuarioEntity _UsuarioEntity = null;


        //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
        private UsuarioEntity _UsuarioTecnicoRegionalEntity = null;
        //END - 14422 - Tela Cliente - Campo Tecnico Regional

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Cliente", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já importado do BPCS! Redirecionando...")]
        [Range(0, 999999, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int CD_CLIENTE { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_RAC { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression("[0-9]{2}.[0-9]{3}.[0-9]{3}/[0-9]{4}-[0-9]{2}", ErrorMessage = "Utilize o formato 00.000.000/0000-00")]
        public string NR_CNPJ { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_CLIENTE { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ENDERECO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_BAIRRO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_CIDADE { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ESTADO { get; set; }

        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression("[0-9]{5}-[0-9]{3}", ErrorMessage = "Utilize o formato 00000-000.")]
        public string EN_CEP { get; set; }

        //[StringLength(6, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //public string CD_REGIAO { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string TX_EMAIL { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_FILIAL { get; set; }

        [StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_ABC { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CL_CLIENTE { get; set; }

        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_TELEFONE { get; set; }

        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_FAX { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] 
        public DateTime? DT_DESATIVACAO { get; set; }

        //SL00035945
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DESATIVACAO_Edit { get; set; }


        public bool? BPCS { get; set; }
        public string CD_BCPS { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int QT_PERIODO { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string FL_PESQ_SATISF { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(1, double.MaxValue, ErrorMessage = "Conteúdo obrigatório!")]
        public string ID_SEGMENTO { get; set; }

        [DataType(DataType.MultilineText)]
        public string EmailsInfo { get; set; }

        public string NM_TECNICO { get; set; }

        public string CD_TECNICO { get; set; }

        public int CD_ORDEM { get; set; }

        public bool FL_KAT_FIXO { get; set; }

        public string DS_CLASSIFICACAO_KAT { get; set; }

        public Dictionary<string, string> CLASSIFICACOES_KAT { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public string FL_KAT_FIXO_SimNao { get; set; }

        public string FL_AtivaPlanoZero { get; set; }

        public Int64? QTD_PeriodoPlanoZero { get; set; }

        [RequiredIfAny("TX_TELEFONERESPONSAVELPECAS")]
        [Display(Name = "Nome")]
        public string TX_NOMERESPONSAVELPECAS { get; set; }

        [RequiredIfAny("TX_NOMERESPONSAVELPECAS")]
        [Display(Name = "Telefone")]
        public string TX_TELEFONERESPONSAVELPECAS { get; set; }

        public GrupoEntity grupo
        {
            get
            {
                if (_GrupoEntity == null)
                {
                    _GrupoEntity = new GrupoEntity();
                }

                return _GrupoEntity;
            }
            set
            {
                if (_GrupoEntity == null)
                {
                    _GrupoEntity = new GrupoEntity();
                }

                _GrupoEntity = value;
            }
        }

        public List<Grupo> grupos { get; set; }



        //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
        public List<Usuario> usuariosTecnicosRegionais { get; set; }
        public UsuarioEntity UsuarioTecnicoRegional
        {
            get
            {
                if (_UsuarioTecnicoRegionalEntity == null) _UsuarioTecnicoRegionalEntity = new UsuarioEntity();
                return _UsuarioTecnicoRegionalEntity;
            }
            set
            {
                if (_UsuarioTecnicoRegionalEntity == null) _UsuarioTecnicoRegionalEntity = new UsuarioEntity();
                _UsuarioTecnicoRegionalEntity = value;
            }
        }
        //END - 14422 - Tela Cliente - Campo Tecnico Regional


        public VendedorEntity vendedor
        {
            get
            {
                if (_VendedorEntity == null)
                {
                    _VendedorEntity = new VendedorEntity();
                }

                return _VendedorEntity;
            }
            set
            {
                if (_VendedorEntity == null)
                {
                    _VendedorEntity = new VendedorEntity();
                }

                _VendedorEntity = value;
            }
        }

        public List<Vendedor> vendedores { get; set; }

        public ExecutivoEntity executivo
        {
            get
            {
                if (_ExecutivoEntity == null)
                {
                    _ExecutivoEntity = new ExecutivoEntity();
                }

                return _ExecutivoEntity;
            }
            set
            {
                if (_ExecutivoEntity == null)
                {
                    _ExecutivoEntity = new ExecutivoEntity();
                }

                _ExecutivoEntity = value;
            }
        }

        public List<Executivo> executivos { get; set; }

        public RegiaoEntity regiao
        {
            get
            {
                if (_RegiaoEntity == null)
                {
                    _RegiaoEntity = new RegiaoEntity();
                }

                return _RegiaoEntity;
            }
            set
            {
                if (_RegiaoEntity == null)
                {
                    _RegiaoEntity = new RegiaoEntity();
                }

                _RegiaoEntity = value;
            }
        }

        public List<Regiao> regioes { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public List<ListaAtivoCliente> listaAtivoCliente { get; set; }

        public List<Usuario> usuarios { get; set; }

        public List<Segmento> segmentos { get; set; }

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

    public class ClienteBPCS : BaseModel
    {
        private GrupoEntity _GrupoEntity = null;
        private VendedorEntity _VendedorEntity = null;
        private ExecutivoEntity _ExecutivoEntity = null;
        private RegiaoEntity _RegiaoEntity = null;
        private UsuarioEntity _UsuarioEntity = null;

        //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
        private UsuarioEntity _UsuarioTecnicoRegionalEntity = null;
        //END - 14422 - Tela Cliente - Campo Tecnico Regional


        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Cliente", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já importado do BPCS! Redirecionando...")]
        [Range(0, 999999, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int CD_CLIENTE { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_RAC { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[RegularExpression("[0-9]{2}.[0-9]{3}.[0-9]{3}/[0-9]{4}-[0-9]{2}", ErrorMessage = "Utilize o formato 00.000.000/0000-00")]
        public string NR_CNPJ { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_CLIENTE { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ENDERECO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_BAIRRO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_CIDADE { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string EN_ESTADO { get; set; }

        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[RegularExpression("[0-9]{5}-[0-9]{3}", ErrorMessage = "Utilize o formato 00000-000.")]
        public string EN_CEP { get; set; }

        //[StringLength(6, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //public string CD_REGIAO { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string TX_EMAIL { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_FILIAL { get; set; }

        [StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_ABC { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CL_CLIENTE { get; set; }

        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        ////[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_TELEFONE { get; set; }

        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        ////[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_FAX { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DT_DESATIVACAO { get; set; }

        public bool? BPCS { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int QT_PERIODO { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string FL_PESQ_SATISF { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(1, double.MaxValue, ErrorMessage = "Conteúdo obrigatório!")]
        public string ID_SEGMENTO { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailsInfo { get; set; }

        public string NM_TECNICO { get; set; }

        public int CD_ORDEM { get; set; }

        public bool FL_KAT_FIXO { get; set; }

        public string CD_BCPS { get; set; }

        public string DS_CLASSIFICACAO_KAT { get; set; }

        public Dictionary<string, string> CLASSIFICACOES_KAT { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public string FL_KAT_FIXO_SimNao { get; set; }

        public string FL_AtivaPlanoZero { get; set; }

        public Int64? QTD_PeriodoPlanoZero { get; set; }

        [RequiredIfAny("TX_TELEFONERESPONSAVELPECAS")]
        [Display(Name = "Nome")]
        public string TX_NOMERESPONSAVELPECAS { get; set; }

        [RequiredIfAny("TX_NOMERESPONSAVELPECAS")]
        [Display(Name = "Telefone")]
        public string TX_TELEFONERESPONSAVELPECAS { get; set; }

        public GrupoEntity grupo
        {
            get
            {
                if (_GrupoEntity == null)
                {
                    _GrupoEntity = new GrupoEntity();
                }

                return _GrupoEntity;
            }
            set
            {
                if (_GrupoEntity == null)
                {
                    _GrupoEntity = new GrupoEntity();
                }

                _GrupoEntity = value;
            }
        }

        public List<Grupo> grupos { get; set; }

        public VendedorEntity vendedor
        {
            get
            {
                if (_VendedorEntity == null)
                {
                    _VendedorEntity = new VendedorEntity();
                }

                return _VendedorEntity;
            }
            set
            {
                if (_VendedorEntity == null)
                {
                    _VendedorEntity = new VendedorEntity();
                }

                _VendedorEntity = value;
            }
        }


        //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
        public List<Usuario> usuariosTecnicosRegionais { get; set; }
        public UsuarioEntity UsuarioTecnicoRegional
        {
            get
            {
                if (_UsuarioTecnicoRegionalEntity == null) _UsuarioTecnicoRegionalEntity = new UsuarioEntity();
                return _UsuarioTecnicoRegionalEntity;
            }
            set
            {
                if (_UsuarioTecnicoRegionalEntity == null) _UsuarioTecnicoRegionalEntity = new UsuarioEntity();
                _UsuarioTecnicoRegionalEntity = value;
            }
        }
        //END - 14422 - Tela Cliente - Campo Tecnico Regional



        public List<Vendedor> vendedores { get; set; }

        public ExecutivoEntity executivo
        {
            get
            {
                if (_ExecutivoEntity == null)
                {
                    _ExecutivoEntity = new ExecutivoEntity();
                }

                return _ExecutivoEntity;
            }
            set
            {
                if (_ExecutivoEntity == null)
                {
                    _ExecutivoEntity = new ExecutivoEntity();
                }

                _ExecutivoEntity = value;
            }
        }

        public List<Executivo> executivos { get; set; }

        public RegiaoEntity regiao
        {
            get
            {
                if (_RegiaoEntity == null)
                {
                    _RegiaoEntity = new RegiaoEntity();
                }

                return _RegiaoEntity;
            }
            set
            {
                if (_RegiaoEntity == null)
                {
                    _RegiaoEntity = new RegiaoEntity();
                }

                _RegiaoEntity = value;
            }
        }

        public List<Regiao> regioes { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public List<ListaAtivoCliente> listaAtivoCliente { get; set; }

        public List<Usuario> usuarios { get; set; }

        public List<Segmento> segmentos { get; set; }

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

    public class RelatorioKAT : BaseModel
    {
        public ClienteEntity cliente { get; set; } = new ClienteEntity();

        public decimal VENDAS { get; set; }
        public decimal GM { get; set; }
        public int GM_GRUPO_EMP { get; set; }
        public int CRITICIDADE_AMB { get; set; }
        public int QTD_ATIVOS { get; set; }
        public int NOTA_QTD_ATIVOS { get; set; }
        public int COMPLEXIDADE_EQUIP { get; set; }
        public decimal SCORE { get; set; }
        public string CLASSIFICACAO { get; set; }
        public int PERIODOS { get; set; }

    }
}
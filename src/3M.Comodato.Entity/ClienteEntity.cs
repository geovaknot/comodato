using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class ClienteEntity : BaseEntity
    {
        private GrupoEntity _GrupoEntity = null;
        private VendedorEntity _VendedorEntity = null;
        private ExecutivoEntity _ExecutivoEntity = null;
        private RegiaoEntity _RegiaoEntity = null;
        private UsuarioEntity _UsuarioEntity = null;

        //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
        private UsuarioEntity _UsuarioTecnicoRegionalEntity = null;
        //END - 14422 - Tela Cliente - Campo Tecnico Regional

        public long CD_CLIENTE { get; set; }
        public string CD_RAC { get; set; }
        public string CD_BCPS { get; set; }
        public string NR_CNPJ { get; set; }
        public string NM_CLIENTE { get; set; }

        public string NM_CLIENTE_Codigo { get; set; }

        public string EN_ENDERECO { get; set; }
        public string EN_BAIRRO { get; set; }
        public string EN_CIDADE { get; set; }
        public string EN_ESTADO { get; set; }
        public string EN_CEP { get; set; }
        public string TX_EMAIL { get; set; }
        //public string CD_REGIAO { get; set; }
        public string CD_FILIAL { get; set; }
        public string CD_ABC { get; set; }
        public string CL_CLIENTE { get; set; }
        public string TX_TELEFONE { get; set; }
        public string TX_FAX { get; set; }
        public DateTime? DT_DESATIVACAO { get; set; }
        public bool? BPCS { get; set; }
        public int QT_PERIODO { get; set; }
        public String FL_PESQ_SATISF { get; set; }
        public bool? FL_KAT_FIXO { get; set; }
        public string DS_CLASSIFICACAO_KAT { get; set; }

        public string CD_TECNICO { get; set; }

        public string FL_KAT_FIXO_SimNao { get; set; }
        public string EmailsInfo { get; set; }
        public string FL_AtivaPlanoZero { get; set; }

        public Int64? QTD_PeriodoPlanoZero { get; set; }

        public string TX_NOMERESPONSAVELPECAS { get; set; }

        public string TX_TELEFONERESPONSAVELPECAS { get; set; }

        public GrupoEntity grupo
        {
            get
            {
                if (_GrupoEntity == null) _GrupoEntity = new GrupoEntity();
                return _GrupoEntity;
            }
            set
            {
                if (_GrupoEntity == null) _GrupoEntity = new GrupoEntity();
                _GrupoEntity = value;
            }
        }


        //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
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
                if (_VendedorEntity == null) _VendedorEntity = new VendedorEntity();
                return _VendedorEntity;
            }
            set
            {
                if (_VendedorEntity == null) _VendedorEntity = new VendedorEntity();
                _VendedorEntity = value;
            }
        }

        public ExecutivoEntity executivo
        {
            get
            {
                if (_ExecutivoEntity == null) _ExecutivoEntity = new ExecutivoEntity();
                return _ExecutivoEntity;
            }
            set
            {
                if (_ExecutivoEntity == null) _ExecutivoEntity = new ExecutivoEntity();
                _ExecutivoEntity = value;
            }
        }

        public RegiaoEntity regiao
        {
            get
            {
                if (_RegiaoEntity == null) _RegiaoEntity = new RegiaoEntity();
                return _RegiaoEntity;
            }
            set
            {
                if (_RegiaoEntity == null) _RegiaoEntity = new RegiaoEntity();
                _RegiaoEntity = value;
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

        public SegmentoEntity Segmento { get; set; } = new SegmentoEntity();


        public override string ToString()
        {
            return $"{this.NM_CLIENTE} ({this.CD_CLIENTE}) - {this.EN_CIDADE} - {this.EN_ESTADO}";
        }

    }

    public class ClienteSinc 
    {
        public Int32 CD_CLIENTE { get; set; }
        public String CD_GRUPO { get; set; }
        public String CD_RAC { get; set; }
        public Int64 CD_VENDEDOR { get; set; }
        public String NR_CNPJ { get; set; }
        public String NM_CLIENTE { get; set; }
        public String EN_ENDERECO { get; set; }
        public String EN_BAIRRO { get; set; }
        public String EN_CIDADE { get; set; }
        public String EN_ESTADO { get; set; }
        public String EN_CEP { get; set; }
        public String CD_REGIAO { get; set; }
        public String CD_FILIAL { get; set; }
        public String CD_ABC { get; set; }
        public String CL_CLIENTE { get; set; }
        public String TX_TELEFONE { get; set; }
        public String TX_FAX { get; set; }
        public DateTime? DT_DESATIVACAO { get; set; }
        public Int32 CD_EXECUTIVO { get; set; }
        public bool? BPCS { get; set; }
        public Int32 QT_PERIODO { get; set; }
        public String TX_EMAIL { get; set; }
        public String FL_PESQ_SATISF { get; set; }
        public decimal PERCENTUAL_REALIZADO { get; set; }
        public string FL_AtivaPlanoZero { get; set; }

        public Int64? QTD_PeriodoPlanoZero { get; set; }
    }
}

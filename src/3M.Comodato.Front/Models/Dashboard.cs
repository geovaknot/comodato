using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Dashboard : BaseModel
    {
        private GrupoEntity _GrupoEntity = null;
        private RegiaoEntity _RegiaoEntity = null;
        private ModeloEntity _ModeloEntity = null;
        private VendedorEntity _VendedorEntity = null;
        private UsuarioEntity _UsuarioRegionalEntity = null;
        private LinhaProdutoEntity _LinhaProdutoEntity = null;

        public string tipoDashboard { get; set; }
        public Dictionary<string, string> tiposDashboard { get; set; }

        public string CLIENTE { get; set; }
        //public string EQUIPAMENTO { get; set; }
        //public string VENDEDOR { get; set; }
        //public string MODELO { get; set; }
        //public string LINHA_PRODUTO { get; set; }
        public string TECNICO { get; set; }

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

        public List<Grupo> grupos { get; set; }

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

        public List<Regiao> regioes { get; set; }

        public ModeloEntity modelo
        {
            get
            {
                if (_ModeloEntity == null) _ModeloEntity = new ModeloEntity();
                return _ModeloEntity;
            }
            set
            {
                if (_ModeloEntity == null) _ModeloEntity = new ModeloEntity();
                _ModeloEntity = value;
            }
        }

        public List<Modelo> modelos { get; set; }

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

        public List<Vendedor> vendedores { get; set; }

        public UsuarioEntity usuarioRegional
        {
            get
            {
                if (_UsuarioRegionalEntity == null) _UsuarioRegionalEntity = new UsuarioEntity();
                return _UsuarioRegionalEntity;
            }
            set
            {
                if (_UsuarioRegionalEntity == null) _UsuarioRegionalEntity = new UsuarioEntity();
                _UsuarioRegionalEntity = value;
            }
        }

        public List<Usuario> usuariosRegionais { get; set; }

        public LinhaProdutoEntity linhaProduto
        {
            get
            {
                if (_LinhaProdutoEntity == null) _LinhaProdutoEntity = new LinhaProdutoEntity();
                return _LinhaProdutoEntity;
            }
            set
            {
                if (_LinhaProdutoEntity == null) _LinhaProdutoEntity = new LinhaProdutoEntity();
                _LinhaProdutoEntity = value;
            }
        }

        public List<LinhaProduto> linhasProdutos { get; set; }
        
    }

    public class ListaCliente : BaseModel
    {
        private ClienteEntity _ClienteEntity = null;

        public ClienteEntity cliente
        {
            get
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                return _ClienteEntity;
            }
            set
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                _ClienteEntity = value;
            }
        }

        public Int64 PERIODO { get; set; }
        public Int64 ATIVOS { get; set; }
        public Int64 VISITAS { get; set; }
        public string AGORA { get; set; }
        public Int64? ST_TP_STATUS_VISITA_OS { get; set; }
        public string TECNICO { get; set; }
        public string PESQUISA { get; set; }
        public Int64? SOLICIT { get; set; }
        public string KAT { get; set; }
        public string GM_ANO_ANTERIOR { get; set; }
        public string GM_ANO_ATUAL { get; set; }
        public string VENDAS_ANO_ANTERIOR { get; set; }
        public string VENDAS_ANO_ATUAL { get; set; }
        public string ICONEVENDAS { get; set; }

    }

    public class ListaModelo : BaseModel
    {
        private ModeloEntity _ModeloEntity = null;

        public ModeloEntity modelo
        {
            get
            {
                if (_ModeloEntity == null) _ModeloEntity = new ModeloEntity();
                return _ModeloEntity;
            }
            set
            {
                if (_ModeloEntity == null) _ModeloEntity = new ModeloEntity();
                _ModeloEntity = value;
            }
        }

        public Int64 ATIVOS { get; set; }
    }

    public class LinhaProdutoGp
    {
         public int CD_LINHA_PRODUTO { get; set; }
         public string DS_LINHA_PRODUTO { get; set; }
    }

    public class ListaEquipamento : ListaModelo
    {
        public string DEPRECIACAO { get; set; }
        public string CUSTO_MANUTENCAO { get; set; }
        public decimal TOT_PECAS { get; set; }
        public decimal TOT_MAO_OBRA { get; set; }
        public decimal TOT_DEPRECIACAO { get; set; }
        public decimal ManutHs { get; set; }
        public decimal ManutPc { get; set; }
    }

    public class EquipamentosGrid
    {
        public List<ListaEquipamento> ListaEquipamento { get; set; }
        public string ManutHs { get; set; }
        public string ManutPc { get; set; }
    }

    public class ListaEquipamentoWorkFlow : ListaModelo
    {
        public Int64 ENV { get; set; }
        public decimal MEN { get; set; }
        public Int64 DEV { get; set; }
        public Int64 PRJ { get; set; }
        public decimal PERCENTUAL { get; set; }
        public string CORFUNDO { get; set; }
    }

    public class ListaVolumeVenda : BaseModel
    {
        private LinhaProdutoEntity _LinhaProdutoEntity = null;

        public LinhaProdutoEntity linhaProduto
        {
            get
            {
                if (_LinhaProdutoEntity == null) _LinhaProdutoEntity = new LinhaProdutoEntity();
                return _LinhaProdutoEntity;
            }
            set
            {
                if (_LinhaProdutoEntity == null) _LinhaProdutoEntity = new LinhaProdutoEntity();
                _LinhaProdutoEntity = value;
            }
        }

        public string TOT_VENDAS { get; set; }
        public string QT_VENDAS_CV { get; set; }
        public string QT_VENDAS { get; set; }
        public string DEPCOM { get; set; }
        public string LESAFO { get; set; }
    }

    public class ListaHistorico: BaseModel
    {
        public string ITEM { get; set; }
        public string JANEIRO { get; set; }
        public string FEVEREIRO { get; set; }
        public string MARCO { get; set; }
        public string ABRIL { get; set; }
        public string MAIO { get; set; }
        public string JUNHO { get; set; }
        public string JULHO { get; set; }
        public string AGOSTO { get; set; }
        public string SETEMBRO { get; set; }
        public string OUTUBRO { get; set; }
        public string NOVEMBRO { get; set; }
        public string DEZEMBRO { get; set; }

        public decimal QT_VENDAS_CV { get; set; }
        public decimal QT_VENDAS { get; set; }
        public decimal DEPCOM { get; set; }
        public decimal LESAFO { get; set; }
        public int CD_MES { get; set; }
        public bool TOTAL { get; set; }
    }

    public class ListaHistoricoValores
    {
        public string ANO { get; set; }
        public string MES { get; set; }
        public decimal VENDAS_2650 { get; set; }
        public decimal VENDAS_2690 { get; set; }
        public decimal VENDAS_TOTAL { get; set; }
        public decimal DEPRECIACAO_2650 { get; set; }
        public decimal DEPRECIACAO_2690 { get; set; }
        public decimal DEPRECIACAO_TOTAL { get; set; }
        public decimal MANUTENCAO_2650 { get; set; }
        public decimal MANUTENCAO_2690 { get; set; }
        public decimal MANUTENCAO_TOTAL { get; set; }
        public bool TOTAL { get; set; }
    }

    public class ListaTecnico : BaseModel
    {
        private TecnicoEntity _TecnicoEntity = null;

        public TecnicoEntity tecnico
        {
            get
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                return _TecnicoEntity;
            }
            set
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                _TecnicoEntity = value;
            }
        }

        public Int64 CARGA { get; set; }
        public Int64 PERCENTUAL { get; set; }
        public Int64 VISITAS { get; set; }
        public string AGORA { get; set; }
        public Int64? ST_TP_STATUS_VISITA_OS { get; set; }
    }

}
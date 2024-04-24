using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class Dashboard : BaseModel
    {
        public decimal TOT_VENDAS { get; set; }
        public decimal QT_VENDAS_CV { get; set; }
        public decimal QT_VENDAS { get; set; }
        public decimal DEPCOM { get; set; }
        public decimal LESAFO { get; set; }

    }

    public class ListaVolumeVenda : Dashboard
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

        public int TOT_VENDAS_PERC { get; set; }
    }

    public class ListaVenda : Dashboard
    {
        public int CD_MES { get; set; }
        public string DS_MES { get; set; }
    }

    public class ListaFamiliaModelo : BaseModel
    {
        private GrupoModeloEntity _GrupoModeloEntity = null;

        public GrupoModeloEntity grupoModelo
        {
            get
            {
                if (_GrupoModeloEntity == null) _GrupoModeloEntity = new GrupoModeloEntity();
                return _GrupoModeloEntity;
            }
            set
            {
                if (_GrupoModeloEntity == null) _GrupoModeloEntity = new GrupoModeloEntity();
                _GrupoModeloEntity = value;
            }
        }

        public Int64 TOTAL { get; set; }
    }

    public class ListaValorPecaEnviadaMes : BaseModel
    {
        public string cdsMes { get; set; }
        public int MES { get; set; }
        public decimal TOTAL_3M1 { get; set; }
        public decimal TOTAL_3M2 { get; set; }
        public decimal TOTAL_3M3 { get; set; }
        public decimal TOTAL_3M4 { get; set; }
        public decimal TOTAL_METAS { get; set; }
    }

    public class ListaTotalAtivo : BaseModel
    {
        public int ANO { get; set; }
        public decimal TOTAL { get; set; }
    }

    public class ListaAtendimento : BaseModel
    {
        public string TITULO { get; set; }
        public Int64 TOTAL { get; set; }
    }

    public class ListaTrocaPeca : BaseModel
    {
        public string DS_MES { get; set; }
        public Int64 TOTAL { get; set; }
        public Int64 QT_HORAS { get; set; }
    }

    public class ListaAtendimentoRegional : BaseModel
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

        public Int64 TOTAL_VISITA_REALIZADO { get; set; }
        public Int64 TOTAL_VISITA_PLANEJADO { get; set; }
    }

    public class ListaPeriodoRealizado : BaseModel
    {
        public string DS_MES { get; set; }
        public Int64 TOTAL { get; set; }
        public Int64 META { get; set; }
    }

    public class ListaTipoSolicitacao : BaseModel
    {
        private WfTipoSolicitacaoEntity _WfTipoSolicitacaoEntity = null;

        public WfTipoSolicitacaoEntity wfTipoSolicitacao
        {
            get
            {
                if (_WfTipoSolicitacaoEntity == null) _WfTipoSolicitacaoEntity = new WfTipoSolicitacaoEntity();
                return _WfTipoSolicitacaoEntity;
            }
            set
            {
                if (_WfTipoSolicitacaoEntity == null) _WfTipoSolicitacaoEntity = new WfTipoSolicitacaoEntity();
                _WfTipoSolicitacaoEntity = value;
            }
        }

        public decimal TOTAL { get; set; }
    }

    public class ListaEnvioEquipamentoLinhaProduto : BaseModel
    {
        public int CD_MES { get; set; }
        public string DS_MES { get; set; }
        public Int64 TOTAL_ROLOS { get; set; }
        public Int64 TOTAL_IDENTIFICACAO { get; set; }
        public Int64 TOTAL_UNITIZACAO { get; set; }
        public Int64 TOTAL_FECHADOR { get; set; }
        //TOTAL_UNITIZACAO TOTAL_FECHADOR
    }

    public class ListaMaquinaEnviadaDevolvidaMes : BaseModel
    {
        public int CD_MES { get; set; }
        public string DS_MES { get; set; }
        public Int64 TOTAL_ENVIO { get; set; }
        public Int64 TOTAL_DEVOLUCAO { get; set; }
    }
}

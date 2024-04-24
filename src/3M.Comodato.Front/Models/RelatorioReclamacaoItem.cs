using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    [Flags]
    public enum AcessoReclamacao
    {
        Edicao = 1,
        Visualizacao = 2,
        EdicaoStatus = 4,
    }

    public class RelatorioReclamacaoItem : BaseModel
    {

        public AcessoReclamacao ModoAcesso { get; set; }
        public AcessoReclamacao Acesso { get; set; }
        public bool ReadOnly => !ModoAcesso.HasFlag(AcessoReclamacao.Edicao);

        public AtivoFixoEntity _AtivoFixoEntity = null;


        public bool PerfilAnalista { get; set; }
        public int codigoRR { get; set; }
        public string DataCadastro { get; set; }
        public string Status { get; set; }
        public string Ativo { get; set; }

        public string TecSolicitante { get; set; }
        public string TecRegional { get; set; }
        public string Cliente { get; set; }
        public string Cd_Cliente { get; set; }

        public string DS_MOTIVO { get; set; }
        public string DS_Descricao { get; set; }

        public string Peca { get; set; }

        public string ActionTipoPedido { get; set; }
        public long ST_STATUS_RR { get; internal set; }
        public string IconeClass { get; internal set; }
        public long ID_RELATORIO_RECLAMACAO { get; internal set; }
        public Int32 TEMPO_ATENDIMENTO { get; set; }
        public string VL_Hora_Atendimento { get; set; }
        public string VL_Minuto_Atendimento { get; set; }

        public string TipoReclamacaoRR { get; set; }
        public string TipoAtendimento { get; set; }
        public string DS_ARQUIVO_FOTO{ get; set; }
        public string DS_TIPO_FOTO { get; set; }

        public long ID_USUARIO_RESPONS { get; set; }
        public string CD_GRUPO_RESPONS { get; set; }
        public SelectList ListaCliente { get; set; }
        public SelectList ListaPeca { get; set; }
        public List<PecaEntity> ListaPecas { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string NM_Fornecedor { get; set; }
        public Decimal Vl_Mao_Obra { get; set; }
        public Decimal Custo_Peca { get; set; }
        public Decimal Custo_Total { get; set; }
        public Int64? ID_OS { get; set; }
        public String DataInicio { get; set; }
        public String DataFim { get; set; }
        public Boolean HabilitaCampo { get; set; }
        public Boolean HabilitaImprimir { get; set; }
    }

    public class RelatorioReclamacaoItemFiltro : RelatorioReclamacaoItem
    {
        public int Tecnico { get; set; }
        public int codigoReclamacao { get; set; }
       

        //public SelectList ListaVisaoPedidos => new SelectList(Utility.ControlesUtility.Dicionarios.VisaoPedidosWorkflow(), "key", "value", 1);
        public SelectList ListaTecnico { get; set; }
        public SelectList ListaCliente { get; set; }
        public SelectList ListaStatus { get; set; }
        public SelectList ListaAtivo { get; set; }

        public SelectList ListaTipoSolicitacao { get; set; }
        public SelectList ListaPeca { get; set; }

        public string TipoSolicitacao { get; set; }
        public SelectList ListaSolicitante { get; set; }


        private ClienteEntity _Cliente = null;
        private TecnicoEntity _Tecnico = null;
        private PecaEntity _PecaEntity = null;



        public List<Cliente> clientes { get; set; }
        public List<Tecnico> tecnicos { get; set; }
        public List<PecaEntity> pecas { get; set; }




        public PecaEntity peca
        {
            get
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                return _PecaEntity;
            }
            set
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                _PecaEntity = value;
            }
        }

        public ClienteEntity cliente
        {
            get
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                return _Cliente;
            }
            set
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                _Cliente = value;
            }
        }

        public TecnicoEntity tecnico
        {
            get
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                return _Tecnico;
            }
            set
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                _Tecnico = value;
            }
        }

        public AtivoFixoEntity ativoFixo
        {
            get
            {
                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                return _AtivoFixoEntity;
            }
            set
            {

                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                _AtivoFixoEntity = value;
            }
        }

        public List<Ativo> ativos { get; set; }
        public List<RRStatus> status { get; set; }



    }


    public class RRAcompanhamentoReclamacao : RelatorioReclamacaoItem
    {
        public bool usuarioAnalistaTecnico { get; set; }
        private AcompanhamentoReclamacao _AcompanhamentoReclamacao = null;
        public AcompanhamentoReclamacao acompanhamentoReclamacao
        {
            get
            {
                if (_AcompanhamentoReclamacao == null) _AcompanhamentoReclamacao = new AcompanhamentoReclamacao();
                return _AcompanhamentoReclamacao;
            }
            set
            {
                if (_AcompanhamentoReclamacao == null) _AcompanhamentoReclamacao = new AcompanhamentoReclamacao();
                _AcompanhamentoReclamacao = value;
            }
        }


        public class AcompanhamentoReclamacao : RRAcompanhamentoAcoesHistoricos
        {

            public List<WfGrupoEntity> wfGrupos { get; set; }

            private RRStatusEntity _TpStatusEntity = null;
            public List<WfGrupoUsuEntity> wfGruposUsu { get; set; }

            public RRStatusEntity RRStatus
            {
                get
                {
                    if (_TpStatusEntity == null) _TpStatusEntity = new RRStatusEntity();
                    return _TpStatusEntity;
                }
                set
                {
                    if (_TpStatusEntity == null) _TpStatusEntity = new RRStatusEntity();
                    _TpStatusEntity = value;
                }
            }

            public List<RRStatusEntity> RRAllStatus { get; set; }

        }


        public class RRAcompanhamentoAcoesHistoricos : RRAcompanhamentoStatusNome
        {
            RRComent _RRComent = null;

            public RRComent rrComent
            {
                get
                {
                    if (_RRComent == null) _RRComent = new RRComent();
                    return _RRComent;
                }
                set
                {
                    if (_RRComent == null) _RRComent = new RRComent();
                    _RRComent = value;
                }
            }
        }


        public class RRAcompanhamentoStatusNome
        {

            public string DS_STATUS_NOME_REDUZ_Novo { get; set; }
            public string DS_STATUS_NOME_REDUZ_TecRegional { get; set; }
            public string DS_STATUS_NOME_REDUZ_AnaliseTecnica { get; set; }
            public string DS_STATUS_NOME_REDUZ_EmCompras { get; set; }
            public string DS_STATUS_NOME_REDUZ_Finalizado { get; set; }
            public string DS_STATUS_NOME_REDUZ_EnviadoTecnicoCampo { get; set; }
            public string DS_STATUS_NOME_REDUZ_Reprovado { get; set; }
        }
    }




}

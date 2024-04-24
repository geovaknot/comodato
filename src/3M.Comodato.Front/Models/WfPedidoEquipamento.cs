using _3M.Comodato.Utility;
using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    [Flags]
    public enum Acesso
    {
        Edicao = 1,
        Visualizacao = 2,
        EdicaoStatus = 4,
    }
    public class WfPedidoEquipamento : BaseModel
    {
        public Acesso ModoAcesso { get; set; }
        public bool ReadOnly => !ModoAcesso.HasFlag(Acesso.Edicao);

        public long ID_WF_PEDIDO_EQUIP { get; set; }
        public string CD_WF_PEDIDO_EQUIP { get; set; }
        public string DT_PEDIDO { get; set; }

        public string NM_EMITENTE { get; set; }
        public long ID_USU_SOLICITANTE { get; set; }
        public long ST_STATUS_PEDIDO { get; internal set; }
        public long ID_USU_EMITENTE { get; internal set; }

        public string CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string NR_CNPJ { get; set; }
        public string EN_ENDERECO { get; set; }
        public string EN_BAIRRO { get; set; }
        public string EN_CEP { get; set; }
        public string EN_CIDADE { get; set; }
        public string EN_ESTADO { get; set; }

        public long ID_SEGMENTO { get; set; }
        public string DS_SEGMENTO { get; set; }

        public string DS_CONTATO_NOME { get; set; }
        public string TX_TELEFONE { get; set; }
        public string TX_EMAIL { get; set; }

        public SelectList ListaVendedor { get; set; }
        public SelectList ListaCliente { get; set; }
        public SelectList ListaAtivosCliente { get; set; } //= new List<ListaAtivoCliente>();

        public long ID_USUARIO_RESPONS { get; set; }
        public string CD_GRUPO_RESPONS { get; set; }

        public string DS_ARQUIVO { get; set; }
        public string DS_TITULO_ANEXO { get; set; }
        public string DS_DESCRICAO_ANEXO { get; set; }

        public string DS_OBSERVACAO { get; set; }


    }

    public class WfPedidoEnvioEquipamento : WfPedidoEquipamento
    {
        public SelectList ListaCategoria { get; set; }
        public SelectList ListaSegmento { get; set; }
        public SelectList ListaTipoSolicitacao { get; set; }
        public SelectList ListaModelo { get; set; }
        public SelectList ListaEtiqueta { get; internal set; }

        public SelectList ListaTipoLocacao => new SelectList(ControlesUtility.Dicionarios.TipoLocacao(), "key", "value");
        public SelectList ListaLinha => new SelectList(ControlesUtility.Dicionarios.TipoEmpacotamento(), "key", "value");
        public SelectList ListaTensao => new SelectList(ControlesUtility.Dicionarios.Tensao(), "key", "value");
        public SelectList ListaUnidadeMedida => new SelectList(ControlesUtility.Dicionarios.UnidadeMedida(), "key", "value");


        public SelectList ListaCondicaoLimpeza => new SelectList(ControlesUtility.Dicionarios.Limpeza(), "key", "value");
        public SelectList ListaCondicaoTemperatura => new SelectList(ControlesUtility.Dicionarios.Temperatura(), "key", "value");
        public SelectList ListaCondicaoUmidade => new SelectList(ControlesUtility.Dicionarios.Umidade(), "key", "value");

        public SelectList ListaTipoProduto => new SelectList(ControlesUtility.Dicionarios.TipoProduto(), "key", "value");
        public SelectList ListaLocalInstalacao => new SelectList(ControlesUtility.Dicionarios.LocalInstalacao(), "key", "value");
        public SelectList ListaVelocidadeLinha => new SelectList(ControlesUtility.Dicionarios.VelocidadeLinha(), "key", "value");
        public SelectList ListaGuiaPosicionamento => new SelectList(ControlesUtility.Dicionarios.GuiaPosicionamento(), "key", "value");

        public SelectList ListaFita { get; set; }
        public SelectList ListaModeloFita { get; set; }
        public SelectList ListaLarguraFita { get; set; }


        public string Categoria { get; set; }
        public string DescricaoCategoria { get; set; }
        public string LinhaProduto { get; set; }
        public string DescricaoSegmento { get; set; }
        public string TipoSolicitacao { get; set; }
        public string TipoLocacao { get; set; }
        public string Modelo { get; set; }
        public string Linha { get; set; }
        public string Tensao { get; set; }
        public string VolumeAno { get; set; }
        public string UnidadeMedida { get; set; }
        public string QuantidadeEquipamento { get; set; }

        public string NumeroAtivoTroca { get; set; }
        public string NumeroNFTroca { get; set; }
        public string DataNFTroca { get; set; }
        public string ModeloEquipamentoTroca { get; set; }

        public string CondicaoLimpeza { get; set; }
        public string CondicaoTemperatura { get; set; }
        public string CondicaoUmidade { get; set; }

        public string ModeloAlturaMinima { get; set; }
        public string ModeloAlturaMaxima { get; set; }
        public string ModeloLarguraMinima { get; set; }
        public string ModeloLarguraMaxima { get; set; }
        public string ModeloComprimentoMinimo { get; set; }
        public string ModeloComprimentoMaximo { get; set; }

        public string ValorAlturaMinima { get; set; }
        public string ValorAlturaMaxima { get; set; }
        public string ValorLarguraMinima { get; set; }
        public string ValorLarguraMaxima { get; set; }
        public string ComprimentoMinimo { get; set; }
        public string ComprimentoMaximo { get; set; }
        public string PesoMinimo { get; set; }
        public string PesoMaximo { get; set; }

        public string Fita { get; set; }
        public string ModeloFita { get; set; }
        public string LarguraFita { get; set; }

        public string TipoProduto { get; set; }
        public string LocalInstalacao { get; set; }
        public string VelocidadeLinha { get; set; }
        public string GuiaPosicionamento { get; set; }

        public bool AplicadorDireito { get; set; }
        public bool AplicadorEsquerdo { get; set; }
        public bool AplicadorSuperior { get; set; }

        public string Etiqueta { get; set; }
        public decimal AlturaEtiqueta { get; set; }
        public decimal LarguraEtiqueta { get; set; }
        public string PvaGramaCaixaPVA { get; set; }
        public decimal StrechPesoPallet { get; set; }
        public decimal StrechAlturaPallet { get; set; }
        public int InkjetCaracteresCaixa { get; set; }
        public SelectList ListaModeloIdentificador { get; internal set; }

        private AcompanhamentoPedidoEnvio _AcompanhamentoPedidoEnvio = null;
        public AcompanhamentoPedidoEnvio acompanhamentoPedidoEnvio
        {
            get
            {
                if (_AcompanhamentoPedidoEnvio == null) _AcompanhamentoPedidoEnvio = new AcompanhamentoPedidoEnvio();
                return _AcompanhamentoPedidoEnvio;
            }
            set
            {
                if (_AcompanhamentoPedidoEnvio == null) _AcompanhamentoPedidoEnvio = new AcompanhamentoPedidoEnvio();
                _AcompanhamentoPedidoEnvio = value;
            }
        }

        public string DS_TITULO { get; set; }
    }

    public class WfPedidoDevolucaoEquipamento : WfPedidoEquipamento
    {
        public string PossuiNotaFiscal3M { get; set; }
        public string ValorNotaFiscal3M { get; set; }
        public int MotivoDevolucao { get; set; }

        public SelectList ListaMotivoDevolucao => new SelectList(ControlesUtility.Dicionarios.MotivoDevolucaoWorkflow(), "key", "value");

        private AcompanhamentoPedidoDevolucao _AcompanhamentoPedidoDevolucao = null;
        public AcompanhamentoPedidoDevolucao acompanhamentoPedidoDevolucao
        {
            get
            {
                if (_AcompanhamentoPedidoDevolucao == null) _AcompanhamentoPedidoDevolucao = new AcompanhamentoPedidoDevolucao();
                return _AcompanhamentoPedidoDevolucao;
            }
            set
            {
                if (_AcompanhamentoPedidoDevolucao == null) _AcompanhamentoPedidoDevolucao = new AcompanhamentoPedidoDevolucao();
                _AcompanhamentoPedidoDevolucao = value;
            }
        }

        public string DS_TITULO { get; set; }
    }

    public class AcessorioPedidoEnvio
    {
        public long CodigoAcessorio { get; set; }
        public long CodigoWorkflow { get; set; }
        public Modelo Modelo { get; set; } = new Modelo();
        public int Quantidade { get; set; }

        public long ST_STATUS_PEDIDO { get; set; }
    }

    public class AcompanhamentoStatusNome
    {
        // Devolução
        public string DS_STATUS_NOME_REDUZ_Rascunho { get; set; }
        public string DS_STATUS_NOME_REDUZ_PendenteAnexar { get; set; }
        public string DS_STATUS_NOME_REDUZ_AnaliseLogistica { get; set; }
        public string DS_STATUS_NOME_REDUZ_Solicitado { get; set; }
        public string DS_STATUS_NOME_REDUZ_PendenciaCliente { get; set; }
        public string DS_STATUS_NOME_REDUZ_RetiradaAgendada { get; set; }
        public string DS_STATUS_NOME_REDUZ_Coletado { get; set; }
        public string DS_STATUS_NOME_REDUZ_AguardandoProgTMS { get; set; }
        public string DS_STATUS_NOME_REDUZ_DevolucaoConcluida { get; set; }
        public string DS_STATUS_NOME_REDUZ_DevolvidoPlanejam { get; set; }
        public string DS_STATUS_NOME_REDUZ_Cancelado { get; set; }


        // Envio
        public string DS_STATUS_NOME_REDUZ_AnaliseMarketing { get; set; }
        public string DS_STATUS_NOME_REDUZ_AnaliseAreaTecnica { get; set; }
        public string DS_STATUS_NOME_REDUZ_AnaliseEspecial { get; set; }
        public string DS_STATUS_NOME_REDUZ_AnalisePlanejamento { get; set; }
        public string DS_STATUS_NOME_REDUZ_EnviadoCliente { get; set; }
        public string DS_STATUS_NOME_REDUZ_EntregueCliente { get; set; }
        public string DS_STATUS_NOME_REDUZ_Instalado { get; set; }
        public string DS_STATUS_NOME_REDUZ_EmCompras { get; set; }
    }

    public class AcompanhamentoAcoesHistoricos : AcompanhamentoStatusNome
    {
        WfPedidoComent _WfPedidoComent = null;

        public WfPedidoComent wfPedidoComent
        {
            get
            {
                if (_WfPedidoComent == null) _WfPedidoComent = new WfPedidoComent();
                return _WfPedidoComent;
            }
            set
            {
                if (_WfPedidoComent == null) _WfPedidoComent = new WfPedidoComent();
                _WfPedidoComent = value;
            }
        }
    }

    public class AcompanhamentoPedidoEnvio : AcompanhamentoAcoesHistoricos
    {
        //private WfGrupoEntity _WFGrupoEntity = null;
        //private WfGrupoUsuEntity _UsuarioXWFGrupoEntity = null;
        private WfStatusPedidoEquipEntity _TpStatusPedidoEntity = null;
        //private EmpresaEntity _EmpresaEntity = null;

        //public WfGrupoEntity wFGrupo
        //{
        //    get
        //    {
        //        if (_WFGrupoEntity == null) _WFGrupoEntity = new WfGrupoEntity();
        //        return _WFGrupoEntity;
        //    }
        //    set
        //    {
        //        if (_WFGrupoEntity == null) _WFGrupoEntity = new WfGrupoEntity();
        //        _WFGrupoEntity = value;
        //    }
        //}

        public List<WfGrupoEntity> wfGrupos { get; set; }

        //public WfGrupoUsuEntity usuarioXWFGrupo
        //{
        //    get
        //    {
        //        if (_UsuarioXWFGrupoEntity == null) _UsuarioXWFGrupoEntity = new WfGrupoUsuEntity();
        //        return _UsuarioXWFGrupoEntity;
        //    }
        //    set
        //    {
        //        if (_UsuarioXWFGrupoEntity == null) _UsuarioXWFGrupoEntity = new WfGrupoUsuEntity();
        //        _UsuarioXWFGrupoEntity = value;
        //    }
        //}

        public List<WfGrupoUsuEntity> wfGruposUsu { get; set; }

        public WfStatusPedidoEquipEntity wfStatusPedidoEquip
        {
            get
            {
                if (_TpStatusPedidoEntity == null) _TpStatusPedidoEntity = new WfStatusPedidoEquipEntity();
                return _TpStatusPedidoEntity;
            }
            set
            {
                if (_TpStatusPedidoEntity == null) _TpStatusPedidoEntity = new WfStatusPedidoEquipEntity();
                _TpStatusPedidoEntity = value;
            }
        }

        public List<WfStatusPedidoEquipEntity> wfStatusPedidosEquip { get; set; }

        //public EmpresaEntity empresa
        //{
        //    get
        //    {
        //        if (_EmpresaEntity == null) _EmpresaEntity = new EmpresaEntity();
        //        return _EmpresaEntity;
        //    }
        //    set
        //    {
        //        if (_EmpresaEntity == null) _EmpresaEntity = new EmpresaEntity();
        //        _EmpresaEntity = value;
        //    }
        //}

        //public List<Empresa> empresas { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public string DT_RETIRADA_AGENDADA { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public string DT_RETIRADA_REALIZADA { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public string DT_PROGRAMADA_TMS { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public string DT_DEVOLUCAO_3M { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //public string DT_DEVOLUCAO_PLANEJAMENTO { get; set; }

    }

    public class AcompanhamentoPedidoDevolucao : AcompanhamentoAcoesHistoricos
    {
        //private WfGrupoEntity _WFGrupoEntity = null;
        //private WfGrupoUsuEntity _UsuarioXWFGrupoEntity = null;
        private WfStatusPedidoEquipEntity _TpStatusPedidoEntity = null;
        private EmpresaEntity _EmpresaEntity = null;

        //public WfGrupoEntity wFGrupo
        //{
        //    get
        //    {
        //        if (_WFGrupoEntity == null) _WFGrupoEntity = new WfGrupoEntity();
        //        return _WFGrupoEntity;
        //    }
        //    set
        //    {
        //        if (_WFGrupoEntity == null) _WFGrupoEntity = new WfGrupoEntity();
        //        _WFGrupoEntity = value;
        //    }
        //}

        public List<WfGrupoEntity> wfGrupos { get; set; }

        //public WfGrupoUsuEntity usuarioXWFGrupo
        //{
        //    get
        //    {
        //        if (_UsuarioXWFGrupoEntity == null) _UsuarioXWFGrupoEntity = new WfGrupoUsuEntity();
        //        return _UsuarioXWFGrupoEntity;
        //    }
        //    set
        //    {
        //        if (_UsuarioXWFGrupoEntity == null) _UsuarioXWFGrupoEntity = new WfGrupoUsuEntity();
        //        _UsuarioXWFGrupoEntity = value;
        //    }
        //}

        public List<WfGrupoUsuEntity> wfGruposUsu { get; set; }

        public WfStatusPedidoEquipEntity wfStatusPedidoEquip
        {
            get
            {
                if (_TpStatusPedidoEntity == null) _TpStatusPedidoEntity = new WfStatusPedidoEquipEntity();
                return _TpStatusPedidoEntity;
            }
            set
            {
                if (_TpStatusPedidoEntity == null) _TpStatusPedidoEntity = new WfStatusPedidoEquipEntity();
                _TpStatusPedidoEntity = value;
            }
        }

        public List<WfStatusPedidoEquipEntity> wfStatusPedidosEquip { get; set; }

        public EmpresaEntity empresa
        {
            get
            {
                if (_EmpresaEntity == null) _EmpresaEntity = new EmpresaEntity();
                return _EmpresaEntity;
            }
            set
            {
                if (_EmpresaEntity == null) _EmpresaEntity = new EmpresaEntity();
                _EmpresaEntity = value;
            }
        }

        public List<Empresa> empresas { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_RETIRADA_AGENDADA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_RETIRADA_REALIZADA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_PROGRAMADA_TMS { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DEVOLUCAO_3M { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DEVOLUCAO_PLANEJAMENTO { get; set; }

    }
}
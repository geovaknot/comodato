using System;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    public class WfPedidoEquipamentoItem : BaseModel
    {
        public Acesso Acesso { get; set; }

        public string CodigoPedido { get; set; }
        public string TituloPedido { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Status { get; set; }
        public string TipoPedido { get; set; }
        public string Solicitante { get; set; }
        public string Cliente { get; set; }
        public string Modelo { get; set; }

        public string ActionTipoPedido { get; set; }
        public int CodigoStatusPedido { get; internal set; }
        public string IconeClass { get; internal set; }
        public long ID_WF_PEDIDO_EQUIP { get; internal set; }
    }

    public class WfPedidoEquipamentoItemFiltro : WfPedidoEquipamentoItem
    {
        public int VisaoPedidos { get; set; }
        public String DataCadastroInicio { get; set; }
        public String DataCadastroFim { get; set; }

        public SelectList ListaVisaoPedidos => new SelectList(Utility.ControlesUtility.Dicionarios.VisaoPedidosWorkflow(), "key", "value", 1);
        public SelectList ListaSolicitante { get; set; }
        public SelectList ListaStatus { get; set; }

        public SelectList ListaTipoSolicitacao { get; set; }
        public string TipoSolicitacao { get; set; }
        
    }

    public class WfPedidoEquipamentoItemDevolucao : BaseModel
    {
        public string CD_WF_PEDIDO_EQUIP { get; set; }
        public DateTime? DT_PEDIDO { get; set; }
        public Decimal ST_STATUS_PEDIDO { get; set; }
        public string TP_PEDIDO { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string Cliente { get; set; }
        public string Modelo { get; set; }
        public string CD_TROCA { get; set; }
        public string CD_ATIVO_FIXO_TROCA { get; set; }
        public string DS_CONTATO_NOME { get; set; }
        public string DS_CONTATO_EMAIL { get; set; }
        public string DS_CONTATO_TEL_NUM { get; set; }
        public string CD_LINHA { get; set; }
        public Int32 CD_UNIDADE_MEDIDA { get; set; }
        public Int32 QT_EQUIPAMENTO { get; set; }
        public Decimal VL_ALTURA_MAX { get; set; }
        public Decimal VL_LARGURA_MAX { get; set; }
        public Decimal VL_COMPRIM_MAX { get; set; }
        public Decimal VL_PESO_MAXIMO { get; set; }
        public String DS_TITULO { get; set; }
        public Int32 CD_MOTIVO_DEVOLUCAO { get; set; }
        public Decimal VL_NOTA_FISCAL_3M { get; set; }
        public String Transportadora { get; set; }
        public DateTime? DT_RETIRADA_AGENDADA { get; set; }
        public DateTime? DT_RETIRADA_REALIZADA { get; set; }
        public DateTime? DT_PROGRAMADA_TMS { get; set; }
        public DateTime? DT_DEVOLUCAO_3M { get; set; }
        public DateTime? DT_DEVOLUCAO_PLANEJAMENTO { get; set; }
        public string DS_OBSERVACAO { get; set; }

    }
}
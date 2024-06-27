using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Pedido : BaseModel
    {
        //private StatusPedidoEntity _StatusPedidoEntity = null;
        //private TecnicoEntity _TecnicoEntity = null;
        //private ClienteEntity _ClienteEntity = null;

        [Range(0, Int64.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 ID_PEDIDO { get; set; }

        [Range(0, Int64.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public Int64 NR_DOCUMENTO { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_CRIACAO { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_ENVIO { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_Aprovacao { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_RECEBIMENTO { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string TX_OBS { get; set; }

        public string PENDENTE { get; set; }

        public string UsuarioSolicitante { get; set; }

        public string UsuarioAprovador { get; set; }

        public string FL_EMERGENCIA { get; set; }

        [Range(0, Int64.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 NR_DOC_ORI { get; set; }

        public StatusPedidoEntity statusPedido { get; set; } = new StatusPedidoEntity();

        public string TP_TIPO_PEDIDO { get; set; }

        public Int64 CD_PEDIDO { get; set; }

        public string CD_PEDIDO_Formatado { get; set; }
        public Int64? nidUsuario { get; set; }
        public Int64? nidUsuarioAprovador { get; set; }

        public TecnicoEntity tecnico { get; set; } = new TecnicoEntity();

        public ClienteEntity cliente { get; set; } = new ClienteEntity();
        public string TP_Especial { get; set; }

        public string Origem { get; set; }

        public string Responsavel { get; set; }
        public string Telefone { get; set; }
    }

    public class SolicitacaoPecas : Pedido
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_CRIACAO_INICIO { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_CRIACAO_FIM { get; set; }

        public List<Cliente> clientes { get; set; }
        public List<Tecnico> tecnicos { get; set; }
        public List<Pedido> pedidos { get; set; }
        public List<StatusPedido> statusPedidos { get; set; }
    }

    public class ListaSolicitacaoPecas : Pedido
    {
        //public string CD_PEDIDO_Formatado { get; set; }

        //private TecnicoEntity _TecnicoEntity = null;

        //public TecnicoEntity tecnico
        //{
        //    get
        //    {
        //        if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
        //        return _TecnicoEntity;
        //    }
        //    set
        //    {
        //        if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
        //        _TecnicoEntity = value;
        //    }
        //}

        public Int64 QTD_SOLICITADA { get; set; }

        public string DS_TP_TIPO_PEDIDO { get; set; }

        public bool ExibirExcluir { get; set; }

        public List<Lote> listaLotes { get; set; } = new List<Lote>();

    }

    public class PreenchimentoSolicitacaoPecas : Pedido
    {
        //private StatusPedidoEntity _StatusPedidoAtualEntity = null;

        public string TituloPagina { get; set; }

        public string tipoOrigemPagina { get; set; }

        //public string ID_PEDIDO_Formatado { get; set; }

        //public string CD_PEDIDO_Formatado { get; set; }

        public string DS_TP_TIPO_PEDIDO { get; set; }

        public List<StatusPedido> statusPedidos { get; set; }

        public StatusPedidoEntity statusPedidoAtual { get; set; } = new StatusPedidoEntity();

        public PedidoPecaAvulso pedidoPecaAvulso { get; set; }

        public PedidoPecaCliente pedidoPecaCliente { get; set; }

        public PedidoPecaTecnico pedidoPecaTecnico { get; set; }

        public List<Cliente> clientes { get; set; }

        public List<Lote> listaLotes { get; set; }

        public string DS_ARQUIVO { get; set; }
    }
    public class PedidoPecaAvulso : BaseModel
    {
        //private PecaEntity _PecaEntity = null;

        public List<PecaEntity> listaPecas { get; set; }

        public PecaEntity peca { get; set; } = new PecaEntity();
        public PedidoPeca pedidoPeca { get; set; } = new PedidoPeca();

        public string QTD_ESTOQUE_3M1 { get; set; }
        public string QTD_ESTOQUE_3M2 { get; set; }
    }

    public class PedidoPecaCliente : BaseModel
    {
        //private PecaEntity _PecaEntity = null;

        public List<PecaEntity> listaPecas { get; set; }


        public PecaEntity peca { get; set; } = new PecaEntity();

        public PedidoPeca pedidoPeca { get; set; } = new PedidoPeca();

        public string QTD_ESTOQUE_3M1 { get; set; }
        public string QTD_ESTOQUE_3M2 { get; set; }

        public string QTD_SUGERIDA_PZ { get; set; }
        public string QTD_ESTOQUE { get; set; }
        public string DT_ULTIMA_UTILIZACAO { get; set; }
    }

    public class PedidoPecaTecnico : BaseModel
    {
        //private PecaEntity _PecaEntity = null;

        public List<PecaEntity> listaPecas { get; set; }

        public PecaEntity peca { get; set; } = new PecaEntity();

        public PedidoPeca pedidoPeca { get; set; } = new PedidoPeca();

        public string QTD_ESTOQUE_3M { get; set; }
        public string QTD_ESTOQUE_3M1 { get; set; }
        public string QTD_ESTOQUE_3M2 { get; set; }
        public string QTD_ESTOQUE_CLIENTE { get; set; }

        public string QTD_SUGERIDA_PZ { get; set; }

        public string QTD_ESTOQUE { get; set; }

        public string DT_ULTIMA_UTILIZACAO { get; set; }
    }

    public class ListaPedidoPecas : Pedido
    {
        public Peca peca { get; set; }

        public EstoqueMovimentacao estoqueMovimentacao { get; set; }

        public EstoquePeca estoquePeca { get; set; }

        public EstoquePeca estoquePeca3M { get; set; }

        public EstoquePeca estoquePeca3M2 { get; set; }

        public EstoquePeca estoquePecaTEC { get; set; } = new EstoquePeca();

        public EstoquePeca estoquePecaCLI { get; set; } = new EstoquePeca();

        public PedidoPeca pedidoPeca { get; set; } = new PedidoPeca();

        public PlanoZero planoZero { get; set; }

        public bool permiteEditar { get; set; }

        public bool permiteExcluir { get; set; }

        public string cssRegraGRIDAplicar { get; set; }

        public string CD_ESTOQUE { get; set; } = null;

        public bool permiteSelecionar { get; set; }

        public string TP_Especial { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }
        public string EnviaBPCS { get; set; }

    }
}
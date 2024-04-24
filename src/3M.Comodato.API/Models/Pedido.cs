using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class Pedido : BaseModel
    {
        private StatusPedidoEntity _StatusPedidoEntity = null;
        private TecnicoEntity _TecnicoEntity = null;
        private ClienteEntity _ClienteEntity = null;

        public Int64 ID_PEDIDO { get; set; }

        public Int64 NR_DOCUMENTO { get; set; }

        public string DT_CRIACAO { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }

        public string DT_ENVIO { get; set; }
        public string DT_Aprovacao { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string UsuarioAprovador { get; set; }

        public string DT_RECEBIMENTO { get; set; }

        public string TX_OBS { get; set; }

        public string PENDENTE { get; set; }

        public Int64 NR_DOC_ORI { get; set; }

        public StatusPedidoEntity statusPedido
        {
            get
            {
                if (_StatusPedidoEntity == null) _StatusPedidoEntity = new StatusPedidoEntity();
                return _StatusPedidoEntity;
            }
            set
            {
                if (_StatusPedidoEntity == null) _StatusPedidoEntity = new StatusPedidoEntity();
                _StatusPedidoEntity = value;
            }
        }

        public string TP_TIPO_PEDIDO { get; set; }

        public Int64 CD_PEDIDO { get; set; }

        public string FL_EMERGENCIA { get; set; }

        public string EnviaBPCS { get; set; }
        public string Origem { get; set; }


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
    }

    public class ListaSolicitacaoPecas : Pedido
    {
        public string CD_PEDIDO_Formatado { get; set; }

        public Int64 QTD_SOLICITADA { get; set; }

        public string DS_TP_TIPO_PEDIDO { get; set; }

    }

    public class ListaPedidoPecas : Pedido
    {
        public Peca peca { get; set; }

        public EstoqueMovimentacao estoqueMovimentacao { get; set; }

        public EstoquePeca estoquePeca { get; set; }

        public EstoquePeca estoquePeca3M { get; set; }

        public PedidoPeca pedidoPeca { get; set; }

        public PlanoZero planoZero { get; set; }

        //public bool permiteEditar { get; set; }

        //public bool permiteExcluir { get; set; }

        //public string cssRegraGRIDAplicar { get; set; }

        //public string tipoOrigemPagina { get; set; }

    }

}
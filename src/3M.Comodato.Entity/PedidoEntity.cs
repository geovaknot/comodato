using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class PedidoEntity : BaseEntity
    {
        private TecnicoEntity _TecnicoEntity = null;
        private StatusPedidoEntity _StatusPedidoEntity = null;
        private ClienteEntity _ClienteEntity = null;

        public Int64 ID_PEDIDO { get; set; }
        public long TOKEN { get; set; }
        public Int64 NR_DOCUMENTO { get; set; }
        public DateTime? DT_CRIACAO { get; set; }
        public DateTime? DT_Aprovacao { get; set; }
        
        public DateTime? DT_ENVIO { get; set; }
        public DateTime? DT_RECEBIMENTO { get; set; }
        public string TX_OBS { get; set; }
        public string PENDENTE { get; set; }
        public Int64? NR_DOC_ORI { get; set; }
        public string TP_TIPO_PEDIDO { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string UsuarioAprovador { get; set; }
        public Int64 CD_PEDIDO { get; set; }
        public string FL_EMERGENCIA { get; set; }
        public Int64? nidUsuario { get; set; }
        public Int64? nidUsuarioAprovador { get; set; }

        public string pecasLote { get; set; }

        public string TP_Especial { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }
        public string Origem { get; set; }
        public string EnviaBPCS { get; set; }

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

    public class PedidoSinc
    {
        public Int64? ID_PEDIDO { get; set; }
        public Int64? nidUsuario { get; set; }
        public Int64? nidUsuarioAprovador { get; set; }
        public long TOKEN { get; set; }
        public String CD_TECNICO { get; set; }
        public Int64? NR_DOCUMENTO { get; set; }
        public DateTime DT_CRIACAO { get; set; }
        public DateTime? DT_ENVIO { get; set; }
        public DateTime? DT_Aprovacao { get; set; }
        
        public DateTime? DT_RECEBIMENTO { get; set; }
        public String TX_OBS { get; set; }
        public String PENDENTE { get; set; }
        public Int64? NR_DOC_ORI { get; set; }
        public Int64 ID_STATUS_PEDIDO { get; set; }
        public Char TP_TIPO_PEDIDO { get; set; }
        public Int64? CD_CLIENTE { get; set; }
        public Int64? CD_PEDIDO { get; set; }
        public String IDENTIFICADOR_PK_ID_PEDIDO { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string UsuarioAprovador { get; set; }
        public Int64? ID_PEDIDO_INSERIDO{ get; set; }
        public string TP_Especial { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }
    }

}

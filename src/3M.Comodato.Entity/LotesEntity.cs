using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LotesEntity
    {
        public Int64 ID_PEDIDO { get; set; }
		public DateTime DT_CRIACAO { get; set; }
        public Int64 ID_LOTE_APROVACAO { get; set; } //tbLoteAprovacao
        public DateTime DT_APROVACAO { get; set; } //tbLoteAprovacao
        public string DS_STATUS_PEDIDO { get; set; }
        public string TP_TIPO_PEDIDO { get; set; }
        
        public string DS_TECNICO { get; set; }
        public string Nm_Empresa { get; set; }
        public string DS_CLIENTE { get; set; }
        
        public string CD_PECA { get; set; }
        public Int64 QTD_SOLICITADA { get; set; }
        public Int64 QTD_APROVADA { get; set; }
        public Int64 QTD_RECEBIDA { get; set; }
        public string ST_STATUS_ITEM { get; set; }
        public decimal VL_PECA { get; set; }

        public string DS_ARQUIVO { get; set; } //tbLoteAprovacao
        public Int64 ID_USUARIO { get; set; } //tbLoteAprovacao
    }

    //public class tbLoteEntity : LotesEntity
    //{
    //    public string DS_ARQUIVO { get; set; }

    //}
}
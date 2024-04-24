using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class RRStatus
    {
        public Int32 ID_RR_STATUS { get; set; }
        public string ST_STATUS_RR { get; set; }
        public string DS_STATUS_NOME { get; set; }
        public string DS_STATUS_NOME_REDUZ { get; set; }
        public string DS_TRANSICAO { get; set; }
        public string DS_COMENTARIO { get; set; }
        public string DS_STATUS_DESCRICAO { get; set; }
        public Int32 NR_ORDEM_FLUXO  { get; set; }
    }
}



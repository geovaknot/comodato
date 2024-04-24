using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class ManutencaoDetalhesCliente
    {
        public Int64? ID_OS { get; set; }
        public Int64? CD_CLIENTE { get; set; }
        public Int64? CD_LINHA_PRODUTO { get; set; }
        public Double TOT_PECAS { get; set; }
        public Double TOT_MAO_OBRA { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
    }
}
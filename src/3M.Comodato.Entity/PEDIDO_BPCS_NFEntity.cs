using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class PEDIDO_BPCS_NFEntity
    {
        public Int64 ID { get; set; }
        public Int64? ID_PEDIDO { get; set; }

        public Int64? ID_ITEM_PEDIDO { get; set; }


        public string CD_PECA { get; set; }

        public int? NR_LINHA { get; set; }

        public Int64? NR_NF { get; set; }

        public Int64? NR_CONTROL { get; set; }

        public Int64? NR_SESM { get; set; }
    }
}

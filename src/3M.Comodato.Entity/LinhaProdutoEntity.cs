using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LinhaProdutoEntity : BaseEntity
    {
        public int CD_LINHA_PRODUTO { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public decimal VL_EXPECTATIVA_PADRAO { get; set; }
    }


}

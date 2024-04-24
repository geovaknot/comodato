using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class DepreciacaoEntity : BaseEntity
    {
        public string CD_ATIVO_FIXO { get; set; }
        public int NR_MESES { get; set; }
        public DateTime? DT_INICIO_DEPREC { get; set; }
        public decimal VL_CUSTO_ATIVO { get; set; }
        public decimal VL_DEPREC_TOTAL { get; set; }
        public decimal VL_DEPREC_ULT_MES { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class ClienteComboEntity
    {
        public long CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string EN_CIDADE { get; set; }
        public string EN_ESTADO { get; set; }
        public DateTime? DT_DESATIVACAO { get; set; }

    }
}

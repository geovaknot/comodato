using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class TipoEntity : BaseEntity
    {
        public Int64 CD_TIPO { get; set; }
        public string DS_TIPO { get; set; }
        public bool? FL_SEGMENTO_DI { get; set; }
    }
}

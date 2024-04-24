using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class TB_PONDERACAO_pzEntity: BaseEntity
    {
        public int ID { get; set; }
        public Int64 MIN_CLIENTES { get; set; }
        public Int64 MAX_CLIENTES { get; set; }
        public int FATOR { get; set; }
        public bool Ativo { get; set; }
    }
}

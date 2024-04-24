using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class TB_PONDERACAO_pz: BaseModel
    {
        public int ID { get; set; }
        public Int64 MIN_CLIENTES { get; set; }
        public Int64 MAX_CLIENTES { get; set; }
        public int FATOR { get; set; }
        public bool Ativo { get; set; }
    }
}
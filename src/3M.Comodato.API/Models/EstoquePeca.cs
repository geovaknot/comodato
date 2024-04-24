using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class EstoquePeca : BaseModel
    {
        public Int64 ID_ESTOQUE_PECA { get; set; }
        //public string CD_PECA { get; set; }
        public Peca Peca { get; set; } = new Peca();
        public string QT_PECA_ATUAL { get; set; }
        public string QT_PECA_MIN { get; set; }
        public string DT_ULT_MOVIM { get; set; }
        //public string ID_ESTOQUE { get; set; }
        public Estoque estoque { get; set; } = new Estoque();
        public Peca peca { get; set; } = new Peca();
    }
}
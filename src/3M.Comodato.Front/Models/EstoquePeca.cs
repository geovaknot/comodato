using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class EstoquePeca : BaseModel
    {
        public string ID_ESTOQUE_PECA { get; set; }
        //public string CD_PECA { get; set; }
        public Peca Peca { get; set; } = new Peca();
        public string QT_PECA_ATUAL { get; set; }

        public Int32 QT_PECA { get; set; }

        public string QT_PECA_MIN { get; set; }
        public string DT_ULT_MOVIM { get; set; }
        //public string ID_ESTOQUE { get; set; }
        public Estoque Estoque { get; set; } = new Estoque();

        public string CodigoPeca => Peca.CD_PECA;
        public string DescricaoPeca => Peca.DS_PECA;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class Estoque : BaseModel
    {
        public long ID_ESTOQUE { get; set; }

        //public string ccdEstoque { get; set; }

        //public string cdsEstoque { get; set; }

        public string TP_ESTOQUE_TEC_3M { get; set; }

        //public string ccdTecnico { get; set; }

        //public Tecnico Tecnico { get; set; } = new Tecnico();

        public long nidUsuarioResponsavel { get; set; }

        public new string cdsAtivo { get; set; }
    }
}
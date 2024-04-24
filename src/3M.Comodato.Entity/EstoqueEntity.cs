using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class EstoqueEntity : BaseEntity
    {
        public long ID_ESTOQUE { get; set; }
        public string CD_ESTOQUE { get; set; }
        public string DS_ESTOQUE { get; set; }
        public long ID_USU_RESPONSAVEL { get; set; }
        //public string CD_TECNICO { get; set; }
        public DateTime? DT_CRIACAO { get; set; }
        public TecnicoEntity tecnico { get; set; } = new TecnicoEntity();
        public string TP_ESTOQUE_TEC_3M { get; set; }
        public string FL_ATIVO { get; set; }
        public int? Web { get; set; }
        public int? Tecnico { get; set; }
        public ClienteEntity Cliente { get; set; } = new ClienteEntity();
    }

    public class EstoqueSinc : BaseEntity
    {
        public Int64 ID_ESTOQUE { get; set; }
        public String CD_ESTOQUE { get; set; }
        public String DS_ESTOQUE { get; set; }
        public Int64? ID_USU_RESPONSAVEL { get; set; }
        public DateTime DT_CRIACAO { get; set; }
        public String CD_TECNICO { get; set; }
        public String CD_CLIENTE { get; set; }
        public String TP_ESTOQUE_TEC_3M { get; set; }
        public String FL_ATIVO { get; set; }
    }

}

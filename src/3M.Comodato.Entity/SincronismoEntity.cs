using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class SincronismoEntity
    {

        public class SincronismoSinc
        {
            public Int64 ID_SINCRONISMO { get; set; }
            public String NM_NOME_TABELA { get; set; }
            public DateTime DT_ULT_ATUALIZA { get; set; }
            public Char TP_ULT_ATUALIZA { get; set; }
            public String TP_SINCRONISMO { get; set; }
            public String TP_ATUALIZACAO { get; set; }
            public String CD_REGRA_PRI { get; set; }
            public DateTime DT_ULT_SINCRONISMO { get; set; }
            public String DS_OBSERVACOES { get; set; }
        }
    }
}

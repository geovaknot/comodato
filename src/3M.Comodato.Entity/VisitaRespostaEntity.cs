using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class VisitaRespostaEntity
    {
        public int ID_RESPOSTA_SATISF { get; set; }
        public int ID_PESQUISA_SATISF { get; set; } = 0;
        public int ID_VISITA { get; set; }
        public DateTime DataResposta { get; set; }
        public string DS_NOME_RESPONDEDOR { get; set; }

        public Decimal NotaPesquisa { get; set; }

        public String Justificativa { get; set; }

        public string RespostaPesquisa1 { get; set; }

        public string RespostaPesquisa2 { get; set; }

        public string RespostaPesquisa3 { get; set; }

        public string RespostaPesquisa4 { get; set; }

        public string RespostaPesquisa5 { get; set; }

        public Int64 nidUsuarioAtualizacao { get; set; }
        public Int64 ID_OS { get; set; }

    }
}

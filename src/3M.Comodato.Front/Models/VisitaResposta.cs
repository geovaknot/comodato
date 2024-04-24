using System;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class VisitaResposta : BaseModel
    {
        public int ID_RESPOSTA_SATISF { get; set; }
        public int ID_PESQUISA_SATISF { get; set; } = 0;
        public int ID_VISITA { get; set; }
        public DateTime DataResposta { get; set; }
        public string DS_NOME_RESPONDEDOR { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public Decimal NotaPesquisa { get; set; }

        [DataType(DataType.MultilineText)]
        public String Justificativa { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa1 { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa2 { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa3 { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa4 { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa5 { get; set; }

        public Int64 nidUsuarioAtualizacao { get; set; }
        public Int64 ID_OS { get; set; }

    }
}
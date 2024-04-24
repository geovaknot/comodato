using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class SituacaoAtivo : BaseModel
    {
        public Int64 CD_SITUACAO_ATIVO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_SITUACAO_ATIVO { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class TBFuncao : BaseModel
    {
        public int CD_FUNCAO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_FUNCAO { get; set; }
    }
}
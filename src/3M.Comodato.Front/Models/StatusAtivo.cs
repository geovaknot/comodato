using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class StatusAtivo : BaseModel
    {
        public Int64 CD_STATUS_ATIVO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_STATUS_ATIVO { get; set; }
    }
}
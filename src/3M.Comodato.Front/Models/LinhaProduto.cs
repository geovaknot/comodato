using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class LinhaProduto : BaseModel
    {
        public int CD_LINHA_PRODUTO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_LINHA_PRODUTO { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string VL_EXPECTATIVA_PADRAO { get; set; }

    }
}
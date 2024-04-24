using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class Funcao : BaseModel
    {
        public Int64 nidFuncao { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string ccdFuncao { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsFuncao { get; set; }
    }
}
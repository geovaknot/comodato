using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Categoria : BaseModel
    {
        public int ID_CATEGORIA { get; set; }

        [StringLength(75, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_CATEGORIA { get; set; }

        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public char FL_ATIVO { get; set; }

        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string CD_CATEGORIA { get; set; }
    }
}
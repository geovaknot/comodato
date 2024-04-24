using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace _3M.Comodato.Front.Models
{
    public class Tipo : BaseModel
    {
        public Int64 CD_TIPO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_TIPO { get; set; }

        public bool FlagSegmentoDI { get; set; }
    }
}
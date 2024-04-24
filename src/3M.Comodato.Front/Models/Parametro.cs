using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class Parametro : BaseModel
    {
        public Int64 nidParametro { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string ccdParametro { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string cdsParametro { get; set; }

        [StringLength(20000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string cvlParametro { get; set; }

        public string flgTipoParametro { get; set; }

        public Dictionary<string, string> tiposParametros { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Lote : BaseModel
    {
        //[Range(0, Int64.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 ID_LOTE_APROVACAO { get; set; }

        //[Range(0, Int64.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 ID_USUARIO { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_CRIACAO { get; set; }

        public string DS_ARQUIVO { get; set; }
    }
}
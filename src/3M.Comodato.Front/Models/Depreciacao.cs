using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Depreciacao : BaseModel
    {
        //[StringLength(6, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        //public string CD_ATIVO_FIXO { get; set; }

        [Range(0, 999, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int NR_MESES { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_INICIO_DEPREC { get; set; }
        
        public string VL_CUSTO_ATIVO { get; set; }
        
        public string VL_DEPREC_TOTAL { get; set; }
        
        public string VL_DEPREC_ULT_MES { get; set; }
    }
}
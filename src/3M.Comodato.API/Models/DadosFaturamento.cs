using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class DadosFaturamento : BaseModel
    {
        [Required(ErrorMessage ="Conteúdo obrigatório!")]
        public Int64 ID { get; set; }

        //[StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_Material { get; set; }

        [StringLength(8, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_Cliente { get; set; }

        //[StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string DepartamentoVenda { get; set; }

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string NRAtivo { get; set; }

        public double AluguelApos3anos { get; set; }

        public Int64 NIDUsuarioSolicitacao { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DT_UltimoFaturamento { get; set; }
        public string Data_Fat { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_Solicitacao { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string HR_Solicitacao { get; set; }

        public Int64 ID_ATIVO_CLIENTE { get; set; }






    }
}
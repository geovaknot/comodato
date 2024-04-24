using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    public class Contrato : BaseModel
    {
        [Display(Name = "Nº Contrato")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Conteúdo inválido!")]
        public string nnrContrato { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public String ddtEmissao { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public String ddtRecebimento { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public String ddtOk { get; set; }

        [StringLength(255, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string cdsObservacao { get; set; }

        [DataType(DataType.MultilineText)]
        public string cdsClausulas { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string cdsContratoTipo { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string cdsStatus { get; set; }

        public string dsStatus { get; set; }


        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(1, int.MaxValue, ErrorMessage = "Conteúdo inválido!")]
        public int nidCliente { get; set; }

        public string dsCliente { get; set; }
    }
}
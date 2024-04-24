using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Sumarizados : BaseModel
    {
        public DateTime? DT_INICIAL { get; set; }
        public DateTime? DT_FINAL { get; set; }
        public string DS_PECA { get; set; }
        public string TX_UNIDADE { get; set; }
        public decimal QTD_RECEBIDA { get; set; }
        public decimal QTD_APROVADA { get; set; }
        public decimal VL_PECA { get; set; }
        public decimal VL_TOTAL_PECA { get; set; }
    }

    public class SumarizadosDetalhe : Sumarizados
    {
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] //dd/MM/yyyy 00:00:00.000
        public new string DT_INICIAL { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] //dd/MM/yyyy 23:59:59.000
        public new string DT_FINAL { get; set; }
    }
}
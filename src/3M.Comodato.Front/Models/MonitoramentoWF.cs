using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class MonitoramentoWF : BaseModel
    {
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] //dd/MM/yyyy 00:00:00.000
        public string DT_INICIAL { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] //dd/MM/yyyy 23:59:59.000
        public string DT_FINAL { get; set; }

        public string TIPO { get; set; }
        public string TipoPedido { get; set; }
        public SelectList ListaStatus { get; set; }
    }
}
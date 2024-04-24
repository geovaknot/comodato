using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    public class Segmento: BaseModel
    {
        public Int64 id_segmento { get; set; }

        [Display(Name = "Cód. Segmento")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Remote("SegmentoUnicoJaExiste", "Segmento", AdditionalFields = "id_segmento", ErrorMessage = "O Segmento informado não é permitido!")]
        public string ds_segmentomin { get; set; }

        [Display(Name = "Segmento")]
        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string ds_segmento { get; set; }

        [Display(Name = "Criticidade")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Conteúdo inválido!")]
        public Int32 nm_criticidade { get; set; }


        [Display(Name = "Descrição")]
        [DataType(DataType.MultilineText)]
        public string ds_descricao { get; set; }
    }
}
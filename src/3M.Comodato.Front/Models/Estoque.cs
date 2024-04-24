using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Estoque : BaseModel
    {
        public long nidEstoque { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string ccdEstoque { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [StringLength(150, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string cdsEstoque { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsTipoEstoque { get; set; }

        public string ccdTecnico { get; set; }

        public Tecnico Tecnico { get; set; } = new Tecnico();

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string nidUsuarioResponsavel { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public new string cdsAtivo { get; set; }

        public Int64? CD_CLIENTE { get; set; }
    }

}
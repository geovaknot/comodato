using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class GrupoModelo : BaseModel
    {
        //[Require(ErrorMessage = "Conteúdo obrigatório!")]
        public int ID_GRUPO_MODELO { get; set; }

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string CD_GRUPO_MODELO { get; set; }

        [StringLength(250, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_GRUPO_MODELO { get; set; }

    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class Grupo : BaseModel
    {
        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Grupo", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já castrado!")]
        public string CD_GRUPO { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_GRUPO { get; set; }

        public bool CancelarVerificarCodigo { get; set; }
    }
}
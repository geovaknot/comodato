using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class StatusContrato : BaseModel
    {
        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "StatusContrato", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já castrado!")]
        public string CD_STATUS_CONTRATO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_STATUS_CONTRATO { get; set; }

        public bool CancelarVerificarCodigo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class Perfil : BaseModel
    {
        public Int64 nidPerfil { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsPerfil { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public int? ccdPerfil { get; set; }
    }
}
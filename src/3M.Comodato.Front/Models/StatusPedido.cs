using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class StatusPedido : BaseModel
    {
        public Int64 ID_STATUS_PEDIDO { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_STATUS_PEDIDO { get; set; }
    }
}
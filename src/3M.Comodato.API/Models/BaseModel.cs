using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.API.Models
{
    public class BaseModel
    {
        // Campo nidXXXXX da tabela criptografada
        public string idKey { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? dtmDataHoraAtualizacao { get; set; }

        public bool bidAtivo { get; set; }

        // Status convertido para os textos (Sim ou Não)
        public string cdsAtivo { get; set; }

        public string JavaScriptToRun { get; set; }
    }
}
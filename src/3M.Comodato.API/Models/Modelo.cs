using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class Modelo : BaseModel
    {
        public string CD_MODELO { get; set; }

        public string DS_MODELO { get; set; }

        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        //public string CD_GRUPO_MODELO { get; set; }

        //public List<GrupoModelo> gruposModelos { get; set; }

        public string FL_ATIVO { get; set; }

        public string cdsFL_ATIVO { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public bool CancelarVerificarCodigo { get; set; }
    }
}
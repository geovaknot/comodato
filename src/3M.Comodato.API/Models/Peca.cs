using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class Peca : BaseModel
    {
        public string CD_PECA { get; set; }

        public string DS_PECA { get; set; }

        public string TX_UNIDADE { get; set; }

        public string QTD_ESTOQUE { get; set; }

        public string QTD_MINIMA { get; set; }

        public string VL_PECA { get; set; }

        public string TP_PECA { get; set; }

        public string FL_ATIVO_PECA { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public Dictionary<string, string> tiposPecas { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public string cdsTP_PECA { get; set; }

        public string cdsFL_ATIVO_PECA { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LogErroEntity
    {
            public int ErroID { get; set; }
            public DateTime Data { get; set; }
            public string ErrorMessage { get; set; }
            public string InnerMessage { get; set; }
            public string InnerStackTrace { get; set; }
            public string StackTrace { get; set; }
            public string Sistema { get; set; }
            public string Cliente { get; set; }
            public string Usuario { get; set; }
            public string Local { get; set; }
            public string Classe { get; set; }
            public string Metodo { get; set; }
            public string Parameters { get; set; }
            public string Servidor { get; set; }
            public string Url { get; set; }
            public string UrlReferrer { get; set; }
            public string Linha { get; set; }
            public string objetoErro { get; set; }
            public string Browser { get; set; }
            public string IP { get; set; }
            public string Tipo { get; set; }
    }
}

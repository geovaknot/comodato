using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class TipoContato : BaseModel
    {
        public Int64 nidTipoContato { get; set; }
        public string cdsTipoContato { get; set; }
    }
}
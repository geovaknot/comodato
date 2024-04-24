using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class WfGrupo : BaseModel
    {
        public int ID_GRUPOWF { get; set; }
        public string CD_GRUPOWF { get; set; }
        public string DS_GRUPOWF { get; set; }
        public string TP_GRUPOWF { get; set; }
    }
}
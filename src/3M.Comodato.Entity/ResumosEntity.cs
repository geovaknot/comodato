using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class ResumosEntity
    {
        public Int64 CD_LINHA_PRODUTO { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public Int64 CD_VENDEDOR { get; set; }
        public string NM_VENDEDOR { get; set; }
        public string CD_GRUPO { get; set; }
        public string DS_GRUPO { get; set; }
        public Int64 CD_EXECUTIVO { get; set; }
        public string NM_EXECUTIVO { get; set; }
        public string CD_REGIAO { get; set; }
        public string DS_REGIAO { get; set; }
    }
}
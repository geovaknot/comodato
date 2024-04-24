using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class MonitoramentoWFEntity
    {
        public DateTime? DT_INICIAL { get; set; }
        public DateTime? DT_FINAL { get; set; }
        public string TIPO { get; set; }

        public string STATUS { get; set; }
        public string GRUPO { get; set; }
        public string RESPONSAVEL { get; set; }
        public decimal TEMPO_MEDIO { get; set; }
        public int QT_FLUXOS { get; set; }
    }
}
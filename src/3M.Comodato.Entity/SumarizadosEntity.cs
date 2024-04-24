using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class SumarizadosEntity
    {
        public DateTime? DT_INICIAL { get; set; }
        public DateTime? DT_FINAL { get; set; }
        public string DS_PECA { get; set; }
        public string TX_UNIDADE { get; set; }
        public decimal QTD_RECEBIDA { get; set; }
        public decimal QTD_APROVADA { get; set; }
        public decimal VL_PECA { get; set; }
        public decimal VL_TOTAL_PECA { get; set; }
    }
}
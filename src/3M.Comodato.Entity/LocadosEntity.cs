using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LocadosEntity
    {
        public DateTime? DT_INICIAL { get; set; }
        public DateTime? DT_FINAL { get; set; }
        public string CD_ATIVO_FIXO { get; set; }
        public string DS_MODELO { get; set; }
        public string DT_NOTAFISCAL { get; set; }
        //public string QTD_VENCIDOS { get; set; }
        public string NR_NOTAFISCAL { get; set; }
        public string DS_TIPO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }        
        public string VL_ALUGUEL { get; set; }
        public string TX_TERMOPGTO { get; set; }
        //public string VENC_1 { get; set; }
        //public string VENC_2 { get; set; }
        //public string VENC_3 { get; set; }
        //public string VENC_4 { get; set; }
        public Int64 CD_VENDEDOR { get; set; }
        public string CD_GRUPO { get; set; }
    }
}

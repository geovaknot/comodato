using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class DevolvidosEntity
    {
        public DateTime? DT_DEV_INICIAL { get; set; }
        public DateTime? DT_DEV_FINAL { get; set; }
        public string DT_DEVOLUCAO { get; set; }
        public string CD_ATIVO_FIXO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string DT_NOTAFISCAL { get; set; }
        public string NR_NOTAFISCAL { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string DS_MOTIVO_DEVOLUCAO { get; set; }
        public string ID_ATIVO_CLIENTE { get; set; }
        public string DS_MODELO { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public string VL_RESIDUAL { get; set; }
        public Int64 CD_VENDEDOR { get; set; }
        public string NM_VENDEDOR { get; set; }
        public string CD_GRUPO { get; set; }
        public string DS_GRUPO { get; set; }
    }
}
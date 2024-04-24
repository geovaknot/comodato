using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class RecolhidosEntity : BaseEntity
    {
        public DateTime? DT_DEV_INICIAL { get; set; }
        public DateTime? DT_DEV_FINAL { get; set; }
        public string DT_DEVOLUCAO { get; set; }
        public string CD_ATIVO_FIXO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string DS_MOTIVO_DEVOLUCAO { get; set; }
        public string ID_ATIVO_CLIENTE { get; set; }
    }
}
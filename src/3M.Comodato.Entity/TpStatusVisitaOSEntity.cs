using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class TpStatusVisitaOSEntity : BaseEntity
    {
        public Int64 ID_TP_STATUS_VISITA_OS { get; set; }
        public int ST_TP_STATUS_VISITA_OS { get; set; }
        public string DS_TP_STATUS_VISITA_OS { get; set; }
        public string FL_STATUS_OS { get; set; }
        public string DS_TP_STATUS_VISITA_OS_ACAO { get; set; }
    }
}

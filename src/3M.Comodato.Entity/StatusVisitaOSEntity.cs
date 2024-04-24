using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class StatusVisitaOSEntity
    {
        public Int64 ID_TP_STATUS_VISITA_OS { get; set; }
        public Int32 ST_TP_STATUS_VISITA_OS { get; set; }
        public String DS_TP_STATUS_VISITA_OS { get; set; }
        public Char FL_STATUS_OS { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String DS_TP_STATUS_VISITA_OS_ACAO { get; set; }
    }
}

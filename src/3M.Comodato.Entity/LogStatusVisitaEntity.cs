using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LogStatusVisitaEntity: BaseEntity
    {
        private VisitaEntity _VisitaEntity = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;

        public Int64 ID_LOG_STATUS_VISITA { get; set; }
        public DateTime? DT_DATA_LOG_VISITA { get; set; }

        public VisitaEntity visita
        {
            get
            {
                if (_VisitaEntity == null) _VisitaEntity = new VisitaEntity();
                return _VisitaEntity;
            }
            set
            {
                if (_VisitaEntity == null) _VisitaEntity = new VisitaEntity();
                _VisitaEntity = value;
            }
        }

        public TpStatusVisitaOSEntity tpStatusVisitaOS
        {
            get
            {
                if (_TpStatusVisitaOSEntity == null) _TpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();
                return _TpStatusVisitaOSEntity;
            }
            set
            {
                if (_TpStatusVisitaOSEntity == null) _TpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();
                _TpStatusVisitaOSEntity = value;
            }
        }
        
    }

    
    public class LogStatusVisitaSinc
    {
        public Int64? ID_LOG_STATUS_VISITA { get; set; }
        public Int64? ID_VISITA { get; set; }
        public DateTime DT_DATA_LOG_VISITA { get; set; }
        public Int32 ST_TP_STATUS_VISITA_OS { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_FK_ID_VISITA { get; set; }
    }
}

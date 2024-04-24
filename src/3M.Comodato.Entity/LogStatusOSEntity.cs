using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LogStatusOSEntity : BaseEntity
    {
        private OSEntity _OSEntity = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;

        public Int64 ID_LOG_STATUS_OS { get; set; }
        public DateTime? DT_DATA_LOG_OS { get; set; }

        public OSEntity OS
        {
            get
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                return _OSEntity;
            }
            set
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                _OSEntity = value;
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


    public class LogStatusOSSinc
    {
        public Int64? ID_LOG_STATUS_OS { get; set; }
        public Int64? ID_OS { get; set; }
        public DateTime DT_DATA_LOG_OS { get; set; }
        public Int32 ST_TP_STATUS_VISITA_OS { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_PK_ID_LOG_STATUS_OS { get; set; }
        public String IDENTIFICADOR_FK_ID_OS { get; set; }
    }

}

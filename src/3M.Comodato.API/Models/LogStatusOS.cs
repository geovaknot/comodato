using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class LogStatusOS : BaseModel
    {
        private OSEntity _OS = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;

        public Int64 ID_LOG_STATUS_OS { get; set; }
        public DateTime? DT_DATA_LOG_OS { get; set; }
        public string DT_DATA_LOG_OS_Formatado { get; set; }
        public string DT_DATA_LOG_OS_HORA_Formatado { get; set; }
        public OSEntity OS
        {
            get
            {
                if (_OS == null) _OS = new OSEntity();
                return _OS;
            }
            set
            {
                if (_OS == null) _OS = new OSEntity();
                _OS = value;
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

        public TimeSpan TempoGasto { get; set; }

        public TimeSpan TempoGastoTOTAL { get; set; }

    }
}
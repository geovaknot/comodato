using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class LogStatusOS : BaseModel
    {
        private OSEntity _OS = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private UsuarioEntity _Usuario = null;

        public Int64 ID_LOG_STATUS_OS { get; set; }
        public DateTime? DT_DATA_LOG_OS { get; set; }

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

        public UsuarioEntity usuario
        {
            get
            {
                if (_Usuario == null) _Usuario = new UsuarioEntity();
                return _Usuario;
            }
            set
            {
                if (_Usuario == null) _Usuario = new UsuarioEntity();
                _Usuario = value;
            }
        }

        public TimeSpan TempoGasto { get; set; }

        public TimeSpan TempoGastoTOTAL { get; set; }

    }
}
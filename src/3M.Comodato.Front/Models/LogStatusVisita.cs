using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class LogStatusVisita : BaseModel
    {
        private VisitaEntity _Visita = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private UsuarioEntity _Usuario = null;

        public Int64 ID_LOG_STATUS_VISITA { get; set; }
        public DateTime? DT_DATA_LOG_VISITA { get; set; }
        public string DT_DATA_LOG_VISITA_SHORT { get; set; }

        public VisitaEntity visita
        {
            get
            {
                if (_Visita == null) _Visita = new VisitaEntity();
                return _Visita;
            }
            set
            {
                if (_Visita == null) _Visita = new VisitaEntity();
                _Visita = value;
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
using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class LogStatusOsPadrao : BaseModel
    {
        private OSPadraoEntity _OS = null;
        private TpStatusOSPadraoEntity _TpStatusOSPadraoEntity = null;
        private UsuarioEntity _Usuario = null;

        public Int64 ID_LOG_STATUS_OS { get; set; }
        public DateTime? DT_DATA_LOG_OS { get; set; }
        public string DT_DATA_LOG_OS_SHORT { get; set; }

        public OSPadraoEntity osPadrao
        {
            get
            {
                if (_OS == null) _OS = new OSPadraoEntity();
                return _OS;
            }
            set
            {
                if (_OS == null) _OS = new OSPadraoEntity();
                _OS = value;
            }
        }

        public TpStatusOSPadraoEntity tpStatusOSPadrao
        {
            get
            {
                if (_TpStatusOSPadraoEntity == null) _TpStatusOSPadraoEntity = new TpStatusOSPadraoEntity();
                return _TpStatusOSPadraoEntity;
            }
            set
            {
                if (_TpStatusOSPadraoEntity == null) _TpStatusOSPadraoEntity = new TpStatusOSPadraoEntity();
                _TpStatusOSPadraoEntity = value;
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
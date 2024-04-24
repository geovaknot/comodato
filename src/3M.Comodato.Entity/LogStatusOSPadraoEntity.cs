using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class LogStatusOSPadraoEntity: BaseEntity
    {
        private OSPadraoEntity _OSEntity = null;
        private TpStatusOSPadraoEntity _TpStatusOSPadraoEntity = null;

        public Int64 ID_LOG_STATUS_OS { get; set; }
        public DateTime? DT_DATA_LOG_OS { get; set; }

        public OSPadraoEntity OS
        {
            get
            {
                if (_OSEntity == null) _OSEntity = new OSPadraoEntity();
                return _OSEntity;
            }
            set
            {
                if (_OSEntity == null) _OSEntity = new OSPadraoEntity();
                _OSEntity = value;
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

    }


    public class LogStatusOSPadraoSinc
    {
        public Int64? ID_LOG_STATUS_OS { get; set; }
        public Int64? ID_OS { get; set; }
        public DateTime DT_DATA_LOG_OS { get; set; }
        public Int32 ST_TP_STATUS_OS { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_PK_ID_LOG_STATUS_OS { get; set; }
        public String IDENTIFICADOR_FK_ID_OS { get; set; }
    }
}


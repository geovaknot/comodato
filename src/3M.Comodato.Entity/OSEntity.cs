using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class OSEntity : BaseEntity
    {
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private TecnicoEntity _TecnicoEntity = null;
        private VisitaEntity _VisitaEntity = null;
        private AtivoFixoEntity _AtivoFixoEntity = null;

        public Int64? ID_OS { get; set; }
        public DateTime? DT_DATA_ABERTURA { get; set; }
        public string TP_MANUTENCAO { get; set; }
        public string DS_OBSERVACAO { get; set; }

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

        public TecnicoEntity tecnico
        {
            get
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                return _TecnicoEntity;
            }
            set
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                _TecnicoEntity = value;
            }
        }

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

        public AtivoFixoEntity ativoFixo
        {
            get
            {
                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                return _AtivoFixoEntity;
            }
            set
            {
                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                _AtivoFixoEntity = value;
            }
        }
    }


    public class OsSinc
    {
        public Int64? ID_OS { get; set; }
        public DateTime DT_DATA_ABERTURA { get; set; }
        public Int32 ST_TP_STATUS_VISITA_OS { get; set; }
        public String CD_ATIVO_FIXO { get; set; }
        public String CD_TECNICO { get; set; }
        public Int64? ID_VISITA { get; set; }
        public Char TP_MANUTENCAO { get; set; }
        public String DS_OBSERVACAO { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_PK_ID_OS { get; set; }
        public String IDENTIFICADOR_FK_ID_VISITA { get; set; }
    }

}

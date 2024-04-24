using System;

namespace _3M.Comodato.Entity
{
    public class VisitaEntity : BaseEntity
    {
        private AgendaEntity _AgendaEntity = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public Int64 ID_VISITA { get; set; }
        public DateTime? DT_DATA_VISITA { get; set; }
        public string DS_OBSERVACAO { get; set; }
        public string DS_NOME_RESPONSAVEL { get; set; }
        public string FL_ENVIO_EMAIL_PESQ { get; set; }

        public AgendaEntity agenda
        {
            get
            {
                if (_AgendaEntity == null)
                {
                    _AgendaEntity = new AgendaEntity();
                }

                return _AgendaEntity;
            }
            set
            {
                if (_AgendaEntity == null)
                {
                    _AgendaEntity = new AgendaEntity();
                }

                _AgendaEntity = value;
            }
        }

        public TpStatusVisitaOSEntity tpStatusVisitaOS
        {
            get
            {
                if (_TpStatusVisitaOSEntity == null)
                {
                    _TpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();
                }

                return _TpStatusVisitaOSEntity;
            }
            set
            {
                if (_TpStatusVisitaOSEntity == null)
                {
                    _TpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();
                }

                _TpStatusVisitaOSEntity = value;
            }
        }

        public ClienteEntity cliente
        {
            get
            {
                if (_ClienteEntity == null)
                {
                    _ClienteEntity = new ClienteEntity();
                }

                return _ClienteEntity;
            }
            set
            {
                if (_ClienteEntity == null)
                {
                    _ClienteEntity = new ClienteEntity();
                }

                _ClienteEntity = value;
            }
        }

        public TecnicoEntity tecnico
        {
            get
            {
                if (_TecnicoEntity == null)
                {
                    _TecnicoEntity = new TecnicoEntity();
                }

                return _TecnicoEntity;
            }
            set
            {
                if (_TecnicoEntity == null)
                {
                    _TecnicoEntity = new TecnicoEntity();
                }

                _TecnicoEntity = value;
            }
        }

    }

    public class VisitaSinc
    {
        public Int64? ID_VISITA { get; set; }
        public DateTime DT_DATA_VISITA { get; set; }
        public Int32 ST_TP_STATUS_VISITA_OS { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public String CD_TECNICO { get; set; }
        public String DS_OBSERVACAO { get; set; }
        public String DS_NOME_RESPONSAVEL { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_PK_ID_VISITA { get; set; }
        public String FL_ENVIO_EMAIL_PESQ { get; set; }
    }

}

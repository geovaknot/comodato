using System;

namespace _3M.Comodato.Entity
{
    public class VisitaPadraoEntity : BaseEntity
    {
        public long ID_VISITA { get; set; }
        public long TOKEN { get; set; }
        public DateTime DT_DATA_VISITA { get; set; }
        public string DS_OBSERVACAO { get; set; }
        public string HR_INICIO { get; set; }
        public string HR_FIM { get; set; }
        public string Email { get; set; }
        public string DS_RESPONSAVEL { get; set; }

        public string Origem { get; set; }

        public string HR_DURACAO {
            get
            {
                try
                {
                    var startTime = new TimeSpan(Convert.ToInt32(HR_INICIO.Split(':')[0]), Convert.ToInt32(HR_INICIO.Split(':')[1]), 0);
                    var endTime = new TimeSpan(Convert.ToInt32(HR_FIM.Split(':')[0]), Convert.ToInt32(HR_FIM.Split(':')[1]), 0);
                    return endTime.Subtract(startTime).ToString("hhmm").Insert(2, ":");
                }
                catch
                {
                    return "00:00";
                }
            }
        }

        private AgendaEntity _agendaEntity = null;
        private TpStatusVisitaPadraoEntity _tpStatusVisitaEntity = null;
        private ClienteEntity _clienteEntity = null;
        private TecnicoEntity _tecnicoEntity = null;
        private TpMotivoVisitaPadraoEntity _tpMotivoVisitaEntity = null;
        private TecnicoClienteEntity _tecnicoClienteEntity = null;

        public AgendaEntity Agenda
        {
            get
            {
                if (_agendaEntity == null)
                    _agendaEntity = new AgendaEntity();
                return _agendaEntity;
            }
            set
            {
                if (_agendaEntity == null)
                    _agendaEntity = new AgendaEntity();
                _agendaEntity = value;
            }
        }

        public TpStatusVisitaPadraoEntity TpStatusVisita
        {
            get
            {
                if (_tpStatusVisitaEntity == null)
                    _tpStatusVisitaEntity = new TpStatusVisitaPadraoEntity();
                return _tpStatusVisitaEntity;
            }
            set
            {
                if (_tpStatusVisitaEntity == null)
                    _tpStatusVisitaEntity = new TpStatusVisitaPadraoEntity();
                _tpStatusVisitaEntity = value;
            }
        }

        public ClienteEntity Cliente
        {
            get
            {
                if (_clienteEntity == null)
                    _clienteEntity = new ClienteEntity();
                return _clienteEntity;
            }
            set
            {
                if (_clienteEntity == null)
                    _clienteEntity = new ClienteEntity();
                _clienteEntity = value;
            }
        }

        public TecnicoEntity Tecnico
        {
            get
            {
                if (_tecnicoEntity == null)
                    _tecnicoEntity = new TecnicoEntity();
                return _tecnicoEntity;
            }
            set
            {
                if (_tecnicoEntity == null)
                    _tecnicoEntity = new TecnicoEntity();
                _tecnicoEntity = value;
            }
        }

        public TecnicoClienteEntity TecnicoCliente
        {
            get
            {
                if (_tecnicoClienteEntity == null)
                    _tecnicoClienteEntity = new TecnicoClienteEntity();
                return _tecnicoClienteEntity;
            }
            set
            {
                if (_tecnicoClienteEntity == null)
                    _tecnicoClienteEntity = new TecnicoClienteEntity();
                _tecnicoClienteEntity = value;
            }
        }

        public TpMotivoVisitaPadraoEntity TpMotivoVisita
        {
            get
            {
                if (_tpMotivoVisitaEntity == null)
                    _tpMotivoVisitaEntity = new TpMotivoVisitaPadraoEntity();
                return _tpMotivoVisitaEntity;
            }
            set
            {
                if (_tpMotivoVisitaEntity == null)
                    _tpMotivoVisitaEntity = new TpMotivoVisitaPadraoEntity();
                _tpMotivoVisitaEntity = value;
            }
        }
    }
}

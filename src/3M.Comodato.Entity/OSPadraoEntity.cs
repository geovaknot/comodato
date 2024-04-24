using System;

namespace _3M.Comodato.Entity
{
    public class OSPadraoEntity : BaseEntity
    {
        public long ID_OS { get; set; }
        public long TOKEN { get; set; }
        public DateTime DT_DATA_OS { get; set; }
        //public int ST_STATUS_OS { get; set; }
        //public int CD_TIPO_OS { get; set; }
        //public long CD_CLIENTE { get; set; }
        //public string CD_TECNICO { get; set; }
        public string HR_INICIO { get; set; }
        public string HR_FIM { get; set; }

        public string DS_OBSERVACAO { get; set; }

        public string Email { get; set; }
        public string DS_RESPONSAVEL { get; set; }
        public string Origem { get; set; }

        public string Criado { get; set; }
        
        public string NOME_LINHA { get; set; }

        public int QT_PERIODO { get; set; }

        public string IDENTIFICADOR_PK_ID_OS { get; set; }

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
        private TpStatusOSPadraoEntity _tpStatusOSEntity = null;
        private TpOSPadraoEntity _tpOSEntity = null;
        private ClienteEntity _clienteEntity = null;
        private TecnicoEntity _tecnicoEntity = null;
        private AtivoFixoEntity _ativoFixoEntity = null;
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

        public TpStatusOSPadraoEntity TpStatusOS
        {
            get
            {
                if (_tpStatusOSEntity == null)
                    _tpStatusOSEntity = new TpStatusOSPadraoEntity();
                return _tpStatusOSEntity;
            }
            set
            {
                if (_tpStatusOSEntity == null)
                    _tpStatusOSEntity = new TpStatusOSPadraoEntity();
                _tpStatusOSEntity = value;
            }
        }

        public TpOSPadraoEntity TpOS
        {
            get
            {
                if (_tpOSEntity == null)
                    _tpOSEntity = new TpOSPadraoEntity();
                return _tpOSEntity;
            }
            set
            {
                if (_tpOSEntity == null)
                    _tpOSEntity = new TpOSPadraoEntity();
                _tpOSEntity = value;
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

        public AtivoFixoEntity AtivoFixo
        {
            get
            {
                if (_ativoFixoEntity == null)
                    _ativoFixoEntity = new AtivoFixoEntity();
                return _ativoFixoEntity;
            }
            set
            {
                if (_ativoFixoEntity == null)
                    _ativoFixoEntity = new AtivoFixoEntity();
                _ativoFixoEntity = value;
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
    }
}

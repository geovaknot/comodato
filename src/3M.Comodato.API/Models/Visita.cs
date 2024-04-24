using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class Visita : BaseModel
    {
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public Int64 ID_VISITA { get; set; }

        public string DT_DATA_VISITA { get; set; }

        public string DS_OBSERVACAO { get; set; }

        public string DS_NOME_RESPONSAVEL { get; set; }

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

        public ClienteEntity cliente
        {
            get
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                return _ClienteEntity;
            }
            set
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                _ClienteEntity = value;
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

        //public List<TpStatusVisitaOS> tiposStatusVisitaOS { get; set; }
    }

    public class VisitaTecnica : Visita
    {
        private AgendaEntity _AgendaEntity = new AgendaEntity();

        public string DT_DATA_VISITA_FIM { get; set; }
        public string HR_INICIO { get; set; }
        public string HR_FIM { get; set; }
        public string HR_TOTAL { get; set; }

        //public List<ListaAtivoCliente> listaAtivoCliente { get; set; }

        public AgendaEntity agenda
        {
            get
            {
                if (_AgendaEntity == null) _AgendaEntity = new AgendaEntity();
                return _AgendaEntity;
            }
            set
            {
                if (_AgendaEntity == null) _AgendaEntity = new AgendaEntity();
                _AgendaEntity = value;
            }
        }

        public string FL_ENVIO_EMAIL_PESQ { get; internal set; }
    }

}
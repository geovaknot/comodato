using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Visita : BaseModel
    {
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSAtualEntity = null;
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public Int64 ID_VISITA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_DATA_VISITA { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_OBSERVACAO { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
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

        public TpStatusVisitaOSEntity tpStatusVisitaOSAtual
        {
            get
            {
                if (_TpStatusVisitaOSAtualEntity == null) _TpStatusVisitaOSAtualEntity = new TpStatusVisitaOSEntity();
                return _TpStatusVisitaOSAtualEntity;
            }
            set
            {
                if (_TpStatusVisitaOSAtualEntity == null) _TpStatusVisitaOSAtualEntity = new TpStatusVisitaOSEntity();
                _TpStatusVisitaOSAtualEntity = value;
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

        public List<TpStatusVisitaOS> tiposStatusVisitaOS { get; set; }
    }

    public class VisitaTecnica : Visita
    {
        private AgendaEntity _AgendaEntity = new AgendaEntity();
        private OSEntity _OSEntity = new OSEntity();

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DATA_VISITA_FIM { get; set; }

        public string HR_INICIO { get; set; }
        public string HR_FIM { get; set; }
        public string HR_TOTAL { get; set; }

        public string ExisteOSAberta { get; set; }
        public string classColorStatus { get; set; }
        public int qtdeAtivosCliente { get; set; }
        
        public List<ListaAtivoCliente> listaAtivoCliente { get; set; }

        public List<LogStatusVisita> listaLogStatusVisita { get; set; }

        public List<OS> listaOS { get; set; }
        public List<Peca> listaPecas { get; set; }

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

        public OSEntity OS
        {
            get
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                return _OSEntity;
            }
            set
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                _OSEntity = value;
            }
        }
        public string tipoOrigemPagina { get; set; }

        public Int64 NR_DIAS_CONFIRMADO { get; set; }
    }

    public class ConsultaConfirmaVista
    {
        private ClienteEntity _Cliente = null;
        private TecnicoEntity _Tecnico = null;
        private AtivoFixoEntity _AtivoFixo = null;
        private OSEntity _OSEntity = null;
        private TpStatusVisitaOSEntity _tpStatusVisitaOS = null;

        public string DT_DATA_ABERTURA_INICIO { get; set; }
        public string DT_DATA_ABERTURA_FIM { get; set; }

        //public AvaliacaoVisita avaliacaoVisita { get; set; }
        public SatisfacaoResposta SatisfacaoResposta { get; set; } = new SatisfacaoResposta();

        public List<Cliente> clientes { get; set; }
        public List<Tecnico> tecnicos { get; set; }
        public List<Ativo> ativos { get; set; }
        public List<OS> OSs { get; set; }
        public List<TpStatusVisitaOS> tiposStatusVisitaOS { get; set; }

        public ClienteEntity cliente
        {
            get
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                return _Cliente;
            }
            set
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                _Cliente = value;
            }
        }

        public TecnicoEntity tecnico
        {
            get
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                return _Tecnico;
            }
            set
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                _Tecnico = value;
            }
        }

        public AtivoFixoEntity ativoFixo
        {
            get
            {
                if (_AtivoFixo == null) _AtivoFixo = new AtivoFixoEntity();
                return _AtivoFixo;
            }
            set
            {
                if (_AtivoFixo == null) _AtivoFixo = new AtivoFixoEntity();
                _AtivoFixo = value;
            }
        }

        public OSEntity OS
        {
            get
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                return _OSEntity;
            }
            set
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                _OSEntity = value;
            }
        }

        public TpStatusVisitaOSEntity tpStatusVisitaOS
        {
            get
            {
                if (_tpStatusVisitaOS == null) _tpStatusVisitaOS = new TpStatusVisitaOSEntity();
                return _tpStatusVisitaOS;
            }
            set
            {
                if (_tpStatusVisitaOS == null) _tpStatusVisitaOS = new TpStatusVisitaOSEntity();
                _tpStatusVisitaOS = value;
            }
        }

    }

    public class HistoricoVisita : BaseModel
    {
        public string DT_INICIO { get; set; }
        public string DT_FIM { get; set; }
        public ClienteEntity cliente { get; set; } = new ClienteEntity();
        public TecnicoEntity tecnico { get; set; } = new TecnicoEntity();
        public List<Cliente> clientes { get; set; } = new List<Cliente>();
        public List<Tecnico> tecnicos { get; set; } = new List<Tecnico>();
    }

    public class ListaHistoricoVisitas : BaseModel
    {
        public Int64 ID_VISITA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DATA_VISITA { get; set; }
        public string NM_CLIENTE { get; set; }
        public string NM_TECNICO { get; set; }
        public string DS_TP_STATUS_VISITA_OS { get; set; }
        public int QT_PERIODO { get; set; }
        public decimal QT_PERIODO_REALIZADO { get; set; }
        public string QT_PERIODO_REALIZADO_FORMATADO { get; set; }
        public string PERCENTUAL { get; set; }
        public TimeSpan TempoGastoTOTAL { get; set; }
        public String HoraMinuto { get; set; }
        public List<Models.LogStatusVisita> logStatusVisitas = new List<LogStatusVisita>();
    }
}
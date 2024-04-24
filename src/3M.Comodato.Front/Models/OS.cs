using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class OS : BaseModel
    {
        private VisitaEntity _VisitaEntity = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSEntity = null;
        private TpStatusVisitaOSEntity _TpStatusVisitaOSAtualEntity = null;
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;
        private AtivoFixoEntity _AtivoFixoEntity = null;

        public Int64 ID_OS { get; set; }
        public string ID_OS_Formatado { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_DATA_ABERTURA { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string TP_MANUTENCAO { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_OBSERVACAO { get; set; }

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

        public List<TpStatusVisitaOS> tiposStatusVisitaOS { get; set; }
        public List<Ativo> ativos { get; set; }

        public Dictionary<string, string> tiposManutencao { get; set; }

        public DateTime? DT_ULTIMA_MANUTENCAO { get; set; }
    }

    public class PreenchimentoOS : OS
    {
        private AgendaEntity _AgendaEntity = new AgendaEntity();

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DATA_ABERTURA_FIM { get; set; }

        public string HR_INICIO { get; set; }
        public string HR_FIM { get; set; }
        public string HR_TOTAL { get; set; }

        public string classColorStatus { get; set; }

        public string idKeyInicial { get; set; }
        //public List<ListaAtivoCliente> listaAtivoCliente { get; set; }

        public List<LogStatusVisita> listaLogStatusVisita { get; set; }

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

        public PecaOS pecaOS { get; set; }

        public PendenciaOS pendenciaOS { get; set; }

        public string tipoOrigemPagina { get; set; }
    }

}
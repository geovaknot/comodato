using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    //public class Agenda
    //{
    //}

    public class ListaAgendaAtendimento : BaseModel
    {
        public Int64 ID_VISITA { get; set; }
        public int ST_TP_STATUS_VISITA_OS { get; set; }
        //public Int64? ID_OS { get; set; }
        public string DS_TP_STATUS_VISITA_OS { get; set; }
        public Int64 ID_AGENDA { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string EN_CIDADE { get; set; }
        public string EN_ESTADO { get; set; }
        public string CD_REGIAO { get; set; }
        public string DS_REGIAO { get; set; }
        public string DT_DATA_VISITA { get; set; }
        public string CD_TECNICO_PRINCIPAL { get; set; }
        public string NM_TECNICO_PRINCIPAL { get; set; }
        public int QT_PERIODO { get; set; }
        public int NR_ORDENACAO { get; set; }
        public TimeSpan TempoGastoTOTAL { get; set; }
        public decimal QT_PERIODO_REALIZADO { get; set; }
        public string QT_PERIODO_REALIZADO_FORMATADO { get; set; }
        public string PERCENTUAL { get; set; }

        public List<ListaLogStatusOS> listaLogStatusOs { get; set; }
        public List<LogStatusVisita> listaLogStatusVisita { get; set; }
    }

    public class ListaLogStatusOS : LogStatusOS
    {
        private TecnicoEntity _Tecnico = null;

        //public Int64 ID_OS { get; set; }
        //public string CD_TECNICO { get; set; }
        //public DateTime DT_DATA_LOG { get; set; }
        //public int ST_TP_STATUS_VISITA_OS { get; set; }
        //public string DS_TP_STATUS_VISITA_OS { get; set; }
        //public TimeSpan TempoGasto { get; set; }
        //public TimeSpan TempoGastoTOTAL { get; set; }

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

    }

}
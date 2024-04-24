using _3M.Comodato.Front.Controllers;
using _3M.Comodato.Front.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.ReportDataSources
{
    public class ReportDataSourceCarteiraAtendimento : List<AgendaAtendimento>
    {
    }
    public class AgendaAtendimento
    {
        public string Agrupamento { get; set; }

        public object Ordenacao { get; set; }

        public string StatusVisita { get; set; }
        public string StatusOS { get; set; }

        public string CD_TECNICO_PRINCIPAL { get; set; }

        public long CodigoCliente { get; set; }

        public string NomeCliente { get; set; }

        public string Regiao { get; set; }

        public DateTime? UltimaVisita { get; set; }

        public DateTime? UltimaOS { get; set; }

        public string TecnicoPrincipal { get; set; }

        public int CodigoOrdem { get; set; }

        //public Decimal PeriodosAno { get; set; }

        public Int64? ID_VISITA { get; set; }

        public Int64? ID_OS { get; set; }
        public int QT_PERIODO { get; set; }
        public TimeSpan TempoGastoTOTAL { get; set; }
        public decimal QT_PERIODO_REALIZADO { get; set; }
        public string QT_PERIODO_REALIZADO_FORMATADO { get; set; }
        public decimal PERCENTUAL { get; set; }
        public string PERCENTUAL_FORMATADO { get; set; }

        public string CD_MODELO { get; set; }
        public string DS_MODELO { get; set; }
        public string CD_ATIVO_FIXO { get; set; }
        public string CD_GRUPO_MODELO { get; set; }
        public string CD_CLIENTE { get; set; }

        public List<LogStatusVisita> listaLogStatusVisita { get; set; }

        public List<LogStatusOsPadrao> listaLogStatusOsPadrao { get; set; }

        //public Decimal PeriodosRealizado => (Convert.ToDecimal(this.TempoGastoTOTAL.Hours) + (Convert.ToDecimal(this.TempoGastoTOTAL.Minutes) / 60)) / 3;

        //public string PercentualAtendimento
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Convert.ToDecimal((this.PeriodosRealizado * 100) / this.PeriodosAno).ToString("N2");
        //        }
        //        catch
        //        {
        //            return Convert.ToDecimal(0).ToString("N2");
        //        }

        //    }
        //}
        //public decimal PercentualAtendimento { get; set; }

        //private TimeSpan? _tempoTotalGasto = null;

        //internal TimeSpan TempoGastoTOTAL
        //{
        //    get
        //    {
        //        if (null == _tempoTotalGasto)
        //        {
        //            AgendaController agendaController = new AgendaController();
        //            _tempoTotalGasto = agendaController.CalcularTempoGastoOS(this.ListaLogStatusOs);
        //        }
        //        return _tempoTotalGasto.Value;
        //    }
        //}

        //private List<ListaLogStatusOS> _listaLogStatusOs = null;

        //internal List<ListaLogStatusOS> ListaLogStatusOs
        //{
        //    get
        //    {
        //        if (null == _listaLogStatusOs)
        //        {
        //            AgendaController agendaController = new AgendaController();
        //            _listaLogStatusOs = agendaController.ObterListaLogStatusOS(ID_VISITA: null, CD_CLIENTE: this.CodigoCliente);
        //        }
        //        return _listaLogStatusOs;
        //    }
        //}
    }


}
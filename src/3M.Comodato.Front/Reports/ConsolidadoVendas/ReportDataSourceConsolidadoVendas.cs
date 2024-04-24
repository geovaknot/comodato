using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.ReportDataSources
{
    public class ReportDataSourceConsolidadoVendas : List<ConsolidadoVendas>
    {
    }
    public class KatPorTecnico
    {
        public string NM_CLIENTE { get; set; }

        public string CD_CLIENTE { get; set; }
        public string CD_TECNICO { get; set; }

        public double KatRealizado { get; set; }

        public double KatMesPorTecnico { get; set; }
    }

    public class AnaliseConsumo
    {
        public Int64 CD_CLIENTE { get; set; }

        public double AQT_VENDAS { get; set; }

        public double ATOT_VENDAS { get; set; }

        public Int64 QT_EQP { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public double TOT_DEPRECIACAO { get; set; }
    }

    public class Manutencao
    {
        public string CD_CLIENTE { get; set; }

        public string NM_CLIENTE { get; set; }

        public Int64 QT_PECAS { get; set; }

        public double VL_PECAS { get; set; }

        public double QT_HORAS { get; set; }

        public double VL_HORAS { get; set; }
    }

    public class AnaliseVendaResumo
    {
        public string CD_CLIENTE { get; set; }
        public double OIR { get; set; }

        public double FGM { get; set; }
        
    }

    public class ConsolidadoVendas
    {
        public string NM_CLIENTE { get; set; }

        public string CD_CLIENTE { get; set; }

        public double KatRealizado { get; set; }

        public double KatMesPorTecnico { get; set; }

        public double OIR { get; set; }

        public double FGM { get; set; }

        public Int64 QT_PECAS { get; set; }

        public double VL_PECAS { get; set; }

        public double QT_HORAS { get; set; }

        public double VL_HORAS { get; set; }

        public double AQT_VENDAS { get; set; }

        public double ATOT_VENDAS { get; set; }

        public Int64 QT_EQP { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public double TOT_DEPRECIACAO { get; set; }

    }
}
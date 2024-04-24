using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class ListaAtivoCliente
    {
        public string CD_ATIVO_FIXO { get; set; }
        public string CD_CLIENTE { get; set; }
        public string cdsPrograma { get; set; }
        public string cdsTipo { get; set; }
        public DateTime? DT_INCLUSAO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }
        public string NR_NOTAFISCAL { get; set; }
        public DateTime? DT_NOTAFISCAL { get; set; }
        public DateTime? DT_DEVOLUCAO { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string DS_MOTIVO_DEVOLUCAO { get; set; }
        public string CD_MODELO { get; set; }
        public string DS_MODELO { get; set; }

        public string NM_CLIENTE { get; set; }
        public string DT_ULTIMA_MANUTENCAO { get; set; }
    }

    public class AtivoClienteSinc
    {
        public Int32 ID_ATIVO_CLIENTE { get; set; }
        public Int32 CD_CLIENTE { get; set; }
        public String CD_ATIVO_FIXO { get; set; }
        public DateTime DT_NOTAFISCAL { get; set; }
        public Int32 NR_NOTAFISCAL { get; set; }
        public DateTime DT_DEVOLUCAO { get; set; }
        public String CD_MOTIVO_DEVOLUCAO { get; set; }
        public String TX_OBS { get; set; }
        public Int32 CD_RAZAO { get; set; }
        public Int32 CD_TIPO { get; set; }
        public Int32 VL_ALUGUEL { get; set; }
        public String TX_TERMOPGTO { get; set; }
    }

}

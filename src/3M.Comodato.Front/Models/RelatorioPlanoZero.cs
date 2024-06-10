using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class RelatorioPlanoZero
    {
        public List<Periodo> Periodos { get; set; }
    }

    public class Periodo
    {
        public string dataMesAno { get; set; }
    }
}
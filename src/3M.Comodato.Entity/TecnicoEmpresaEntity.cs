using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class TecnicoEmpresaEntity : BaseEntity
    {
        public string CD_TECNICO { get; set; }
        public decimal CD_EMPRESA { get; set; }
        public decimal nidTecnicoEmpresa { get; set; }
    }
}

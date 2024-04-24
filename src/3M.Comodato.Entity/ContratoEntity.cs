using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class ContratoEntity : BaseEntity
    {
        public Decimal ID_CONTRATO { get; set; }

        public DateTime DT_EMISSAO { get; set; }

        public DateTime? DT_RECEBIMENTO { get; set; }

        public DateTime? DT_OK { get; set; }

        public string TX_OBS { get; set; }

        public string TX_CONTRATO_TIPO { get; set; }

        public Decimal NR_CONTRATO { get; set; }

        public ClienteEntity Cliente { get; set; } = new ClienteEntity();

        public IEnumerable<ModeloEntity> Modelos { get; set; } = new List<ModeloEntity>();

        public StatusContratoEntity Status { get; set; } = new StatusContratoEntity();

        public string DS_CLAUSULAS { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class AgendaEntity : BaseEntity
    {
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public Int64 ID_AGENDA { get; set; }
        public int NR_ORDENACAO { get; set; }

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

    }

    public class AgendaReordenar : AgendaEntity
    {
        public string TP_ACAO { get; set; }
    }


    public class AgendaSinc
    {
        public Int64 ID_AGENDA { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public String CD_TECNICO { get; set; }
        public Int32 NR_ORDENACAO { get; set; }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class TecnicoClienteEntity : BaseEntity
    {
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public int CD_ORDEM { get; set; }

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

    public class TecnicoClienteReordenar : TecnicoClienteEntity
    {
        public string TP_ACAO { get; set; }
    }

    public class TecnicoClienteSinc
    {
        public Int64 CD_CLIENTE { get; set; }
        public String CD_TECNICO { get; set; }
        public Int32 CD_ORDEM { get; set; }
    }

    public class TecnicoClienteInativar : TecnicoClienteEntity
    {
        public Int64 ID_ESCALA { get; set; }
    }

}

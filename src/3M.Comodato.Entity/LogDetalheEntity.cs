using System;

namespace _3M.Comodato.Entity
{
    
    public class LogDetalheEntity
    {
        private LogEntity _LogEntity = null;

        public Int64 nidLogDetalhe { get; set; }

        public LogEntity log
        {
            get
            {
                if (_LogEntity == null) _LogEntity = new LogEntity();
                return _LogEntity;
            }
            set
            {
                if (_LogEntity == null) _LogEntity = new LogEntity();
                _LogEntity = value;
            }
        }

        public string cnmCampo { get; set; }
        public string cdsValorNovo { get; set; }
        public string cdsValorAntigo { get; set; }
    }
}

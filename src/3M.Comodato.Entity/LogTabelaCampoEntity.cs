using System;

namespace _3M.Comodato.Entity
{
    public class LogTabelaCampoEntity
    {
        private LogTabelaEntity _LogTabelaEntity = null;        

        public Int64 nidLogTabelaCampo { get; set; }

        public LogTabelaEntity logTabela
        {
            get
            {
                if (_LogTabelaEntity == null) _LogTabelaEntity = new LogTabelaEntity();
                return _LogTabelaEntity;
            }
            set
            {
                if (_LogTabelaEntity == null) _LogTabelaEntity = new LogTabelaEntity();
                _LogTabelaEntity = value;
            }
        }

        public string cnmCampo { get; set; }
        public string cdsAlias { get; set; }
    }
}

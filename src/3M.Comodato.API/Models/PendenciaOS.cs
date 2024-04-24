using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.API.Models
{
    public class PendenciaOS : BaseModel
    {
        //private OSEntity _OSEntity = null;
        private PecaEntity _PecaEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public Int64 ID_PENDENCIA_OS { get; set; }

        public string DT_ABERTURA { get; set; }

        public string DS_DESCRICAO { get; set; }

        public string QT_PECA { get; set; }

        public string ST_STATUS_PENDENCIA { get; set; }

        public String ST_TP_PENDENCIA { get; set; }

        public string CD_TP_ESTOQUE_CLI_TEC { get; set; }

        public Dictionary<string, string> tiposEstoqueUtilizado { get; set; }

        public Dictionary<string, string> tiposStatusPendenciaOS { get; set; }

        public List<PecaEntity> listaPecas { get; set; }

        //public OSEntity OS
        //{
        //    get
        //    {
        //        if (_OSEntity == null) _OSEntity = new OSEntity();
        //        return _OSEntity;
        //    }
        //    set
        //    {
        //        if (_OSEntity == null) _OSEntity = new OSEntity();
        //        _OSEntity = value;
        //    }
        //}
        public OS OS { get; set; }

        public PecaEntity peca
        {
            get
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                return _PecaEntity;
            }
            set
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                _PecaEntity = value;
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

    public class ListaPendenciaOS : PendenciaOS
    {
        public string DS_STATUS_PENDENCIA { get; set; }
        public string DS_TP_ESTOQUE_CLI_TEC { get; set; }
    }
}
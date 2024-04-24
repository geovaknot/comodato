using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class PendenciaOSEntity : BaseEntity
    {
        private OSEntity _OSEntity = null;
        private PecaEntity _PecaEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public Int64 ID_PENDENCIA_OS { get; set; }
        public Int64 PENDENCIA_OS { get; set; }
        public long TOKEN { get; set; }
        public DateTime? DT_ABERTURA { get; set; }
        public string DT_ABERTURA_Formatado { get; set; }
        public string DS_DESCRICAO { get; set; }
        public decimal? QT_PECA { get; set; }
        public string QT_PECA_Formatado { get; set; }
        public string ST_STATUS_PENDENCIA { get; set; }
        public string CD_TP_ESTOQUE_CLI_TEC { get; set; }
        public string ST_TP_PENDENCIA { get; set; }
        public OSEntity OS
        {
            get
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                return _OSEntity;
            }
            set
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                _OSEntity = value;
            }
        }

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

    public class PendenciaOSSinc
    {
        public Int64? ID_PENDENCIA_OS { get; set; }
        public Int64? ID_OS { get; set; }
        public long TOKEN { get; set; }
        public DateTime DT_ABERTURA { get; set; }
        public DateTime? DT_DATA_OS { get; set; }
        public String DS_DESCRICAO { get; set; }
        public String CD_PECA { get; set; }
        public String CD_TECNICO { get; set; }
        public Int64 QT_PECA { get; set; }
        public Char ST_STATUS_PENDENCIA { get; set; }
        public Char CD_TP_ESTOQUE_CLI_TEC { get; set; }
        public Char ST_TP_PENDENCIA { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_PK_ID_PENDENCIA_OS { get; set; }
        public String IDENTIFICADOR_FK_ID_OS { get; set; }
        public String CD_CLIENTE { get; set; }
    }


}

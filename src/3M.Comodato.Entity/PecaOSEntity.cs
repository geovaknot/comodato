using System;

namespace _3M.Comodato.Entity
{
    public class PecaOSEntity : BaseEntity
    {
        private OSEntity _OSEntity = null;
        private PecaEntity _PecaEntity = null;

        public Int64 ID_PECA_OS { get; set; }
        public long TOKEN { get; set; }
        public decimal QT_PECA { get; set; }
        public decimal VL_VALOR_PECA { get; set; }
        public string CD_TP_ESTOQUE_CLI_TEC { get; set; }

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
    }

    public class PecaOSDetalhamentoEntity : PecaOSEntity
    {
        private TecnicoEntity _TecnicoEntity = null;
        private ClienteEntity _ClienteEntity = null;

        public string QT_PECA_Formatado { get; set; }

        public string DS_OBSERVACAO { get; set; }

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
    }

    public class PecaOSSinc
    {
        public Int64? ID_PECA_OS { get; set; }
        public Int64? ID_OS { get; set; }
        public long TOKEN { get; set; }
        public String CD_PECA { get; set; }
        public Int64 QT_PECA { get; set; }
        public Char CD_TP_ESTOQUE_CLI_TEC { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public DateTime? DT_DATA_OS { get; set; }
        public String IDENTIFICADOR_PK_ID_PECA_OS { get; set; }
        public String IDENTIFICADOR_FK_ID_OS { get; set; }
        public String DS_OBSERVACAO { get; set; }
        public bool EXCLUIR_PECA { get; set; }
        public decimal VL_VALOR_PECA { get; set; }
        public string DS_PECA {get;set;}
    }

}

using System;

namespace _3M.Comodato.Entity
{
    public class EstoqueMoviEntity : BaseEntity, ICloneable
    {
        public long ID_ESTOQUE_MOVI { get; set; }
        public TpEstoqueMoviEntity TP_MOVIMENTACAO { get; set; } = new TpEstoqueMoviEntity();
        public int? ID_OS { get; set; }
        public DateTime DT_MOVIMENTACAO { get; set; }
        public EstoqueEntity ESTOQUE_DESTINO { get; set; } = new EstoqueEntity();
        public PecaEntity Peca { get; set; } = new PecaEntity();
        public decimal QT_PECA { get; set; }
        public UsuarioEntity USU_MOVI { get; set; } = new UsuarioEntity();
        public EstoqueEntity ESTOQUE_ORIGEM { get; set; } = new EstoqueEntity();
        public string TP_ENTRADA_SAIDA { get; set; }
        public ClienteEntity cliente { get; set; } = new ClienteEntity();
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class EstoqueMoviSinc
    {
        public Int64? ID_ESTOQUE_MOVI { get; set; }
        public Char CD_TP_MOVIMENTACAO { get; set; }
        public Int64? ID_OS { get; set; }
        public DateTime DT_MOVIMENTACAO { get; set; }
        public Int64 ID_ESTOQUE { get; set; }
        public String CD_PECA { get; set; }
        public Int64 QT_PECA { get; set; }
        public Int64 ID_USU_MOVI { get; set; }
        public Int64 ID_ESTOQUE_ORIGEM { get; set; }
        public Char TP_ENTRADA_SAIDA { get; set; }
        public Int64? CD_CLIENTE { get; set; }
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public String IDENTIFICADOR_PK_ID_LOG_STATUS_OS { get; set; }
        public String IDENTIFICADOR_FK_ID_OS { get; set; }
    }


}

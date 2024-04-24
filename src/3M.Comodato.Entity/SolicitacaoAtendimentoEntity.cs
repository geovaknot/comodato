using System;

namespace _3M.Comodato.Entity
{
    public class SolicitacaoAtendimentoEntity : BaseEntity
    {
        public long ID_SOLICITA_ATENDIMENTO { get; set; }
        public long ID_USU_SOLICITANTE { get; set; }
        public DateTime DT_DATA_SOLICITACAO { get; set; }
        public string DS_OBSERVACAO { get; set; }
        public string DS_CONTATO { get; set; }

        public ClienteEntity CLIENTE { get; set; } = new ClienteEntity();
        public StatusAtendimentoEntity StatusAtendimento { get; set; } = new StatusAtendimentoEntity();
        public TipoAtendimentoEntity TipoAtendimento { get; set; } = new TipoAtendimentoEntity();
        public AtivoFixoEntity AtivoFixo { get; set; } = new AtivoFixoEntity();
        public OSEntity OS { get; set; } = new OSEntity();

        public int QT_EQUIPAMENTO { get; set; }
    }
}

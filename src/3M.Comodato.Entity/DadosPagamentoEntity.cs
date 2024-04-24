using System;

namespace _3M.Comodato.Entity
{
    public class DadosPagamentoEntity : BaseEntity
    {
        private OSEntity _OSEntity = null;
        private PecaEntity _PecaEntity = null;

        public Int64 ID { get; set; }

        public string CD_Cliente { get; set; }

        public string NRAtivo { get; set; }

        public DateTime DT_Solicitacao { get; set; }
        public string DT_DATA_Solicitacao { get; set; }

        public DateTime DataEmissaoNF { get; set; }
        public string DT_DATA_Emissao { get; set; }

        public Int64 NRSolicitacaoSESM { get; set; }

        public Int64? NR_NotaFiscal { get; set; }

        public string SerieNF { get; set; }

        public Int64 NRLinhaNF { get; set; }

        public Int64 ID_DADOS_FATURAMENTO { get; set; }
        public string SituacaoBpcs { get; set; }
        public string DSStatus { get; set; }

    }
}

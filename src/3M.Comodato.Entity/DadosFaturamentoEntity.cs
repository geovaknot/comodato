using System;

namespace _3M.Comodato.Entity
{
    public class DadosFaturamentoEntity : BaseEntity
    {
        private OSEntity _OSEntity = null;
        private PecaEntity _PecaEntity = null;

        public Int64 ID { get; set; }

        public string CD_Material { get; set; }

        public string CD_Cliente { get; set; }

        public string DepartamentoVenda { get; set; }

        public string NRAtivo { get; set; }

        public string AtivoFixo { get; set; }

        public double AluguelApos3anos { get; set; }

        public Int64 NIDUsuarioSolicitacao { get; set; }

        public DateTime DT_UltimoFaturamento { get; set; }
        string Data_Fat { get; set; }

        public string DT_Solicitacao { get; set; }

        public string HR_Solicitacao { get; set; }

        public Int64 ID_ATIVO_CLIENTE { get; set; }

        public bool EnviadoBcps { get; set; }
        public bool Ativo { get; set; }
        public string FaturamentosID { get; set; }
        public string SituacaoBpcs { get; set; }
        public string DSStatus { get; set; }

    }
}



using System;

namespace _3M.Comodato.Entity
{
    public class TpEstoqueMoviEntity : BaseEntity
    {
        public long ID_TP_ESTOQUE_MOVI { get; set; }

        /// <summary>
        ///1- Mov. Completa de Estoque
        ///2- Mov.peças entre estoques
        ///3- Mov.Ajuste de peças em estoque
        ///4- Utilização em Atendimento
        ///5- Remessa 3M p/ Estoque Intermediário
        /// </summary>
        public string CD_TP_MOVIMENTACAO { get; set; }
        public string DS_TP_MOVIMENTACAO { get; set; }
        //DS_NOME_REDUZ

        //“E” – indica movimentação de entrada
        //“S” – indica movimentação de saida
        //TP_ENTRADA_SAIDA
    }

    public class TpEstoqueMoviSinc
    {
        public Int64 ID_TP_ESTOQUE_MOVI { get; set; }
        public String CD_TP_MOVIMENTACAO { get; set; }
        public String DS_TP_MOVIMENTACAO { get; set; }
        public String DS_NOME_REDUZ { get; set; }
    }

}

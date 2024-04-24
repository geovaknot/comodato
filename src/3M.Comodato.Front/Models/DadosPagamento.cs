using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class DadosPagamento
    {
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public Int64 ID { get; set; }

        [StringLength(8, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_Cliente { get; set; }

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string NRAtivo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DT_Solicitacao { get; set; }

        public string DT_DATA_Solicitacao { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataEmissaoNF { get; set; }

        public string DT_DATA_Emissao { get; set; }

        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 NRSolicitacaoSESM { get; set; }

        public Int64? NR_NotaFiscal { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string SerieNF { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 NRLinhaNF { get; set; }
   
        public Int64 ID_DADOS_FATURAMENTO { get; set; }
        public string SituacaoBpcs { get; set; }
        public string DSStatus { get; set; }
    }
}
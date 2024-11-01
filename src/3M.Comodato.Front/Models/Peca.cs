using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class Peca : BaseModel
    {
        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Peca", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já importado do BPCS! Redirecionando...")]
        public string CD_PECA { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_PECA { get; set; }
        public string DESCRICAO_PECA { get; set; }

        [StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string TX_UNIDADE { get; set; }

        [StringLength(19, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression(@"^(0(?:,[0]{3})*|[1-9][0-9]{0,2}(?:,[0-9]{3})*)$", ErrorMessage = "Valor inválido!")]
        public string QTD_ESTOQUE { get; set; }

        public decimal QTD_ESTOQUE_GRID { get; set; }

        [StringLength(19, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression(@"^(0(?:,[0]{3})*|[1-9][0-9]{0,2}(?:,[0-9]{3})*)$", ErrorMessage = "Valor inválido!")]
        public string QTD_MINIMA { get; set; }

        [StringLength(17, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string VL_PECA { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string TP_PECA { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string FL_ATIVO_PECA { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public Dictionary<string, string> tiposPecas { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public string cdsTP_PECA { get; set; }

        public string cdsFL_ATIVO_PECA { get; set; }

        public string CD_PECA_RECUPERADA { get; set; }

        [StringLength(19, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [RegularExpression(@"^[1-9][0-9]{0,2}(?:,[0-9]{3})*$", ErrorMessage = "Valor inválido!")]
        public string QTD_PlanoZero { get; set; } = "1,000";
    }
}
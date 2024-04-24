using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Consumivel : BaseModel
    {
        private LinhaProdutoEntity _LinhaProdutoEntity = null;

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Consumivel", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já importado do BPCS! Redirecionando...")]
        public string CD_CONSUMIVEL { get; set; }

        [StringLength(85, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_CONSUMIVEL { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string TX_UNIDADE { get; set; }

        [StringLength(6, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_COMMODITY { get; set; }

        [StringLength(45, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string DS_COMMODITY { get; set; }

        [StringLength(9, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_MAJOR { get; set; }

        [StringLength(45, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string DS_MAJOR { get; set; }

        [StringLength(9, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_FAMILY { get; set; }

        [StringLength(45, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string DS_FAMILY { get; set; }

        [StringLength(13, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_SUB_FAMILY { get; set; }

        [StringLength(45, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string DS_SUB_FAMILY { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string TX_UNIDADE_CONV { get; set; }

        public string CUSTO { get; set; }

        public bool? BPCS { get; set; }

        public LinhaProdutoEntity linhaProduto
        {
            get
            {
                if (_LinhaProdutoEntity == null) _LinhaProdutoEntity = new LinhaProdutoEntity();
                return _LinhaProdutoEntity;
            }
            set
            {
                if (_LinhaProdutoEntity == null) _LinhaProdutoEntity = new LinhaProdutoEntity();
                _LinhaProdutoEntity = value;
            }
        }

        public List<LinhaProduto> linhasProdutos { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public bool ST_ATIVO { get; set; }

        public string cdsST_ATIVO { get; set; }
    }
}
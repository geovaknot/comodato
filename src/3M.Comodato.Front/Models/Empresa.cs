using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class Empresa : BaseModel
    {
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Somente valor positivo acima de 0(zero)!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Empresa", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já castrado!")]
        public Int64 CD_Empresa { get; set; }

        //[StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //public string IIComp { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_Empresa { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NR_Cnpj { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string EN_Endereco { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string EN_Bairro { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string EN_Cidade { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string EN_Estado { get; set; }

        [StringLength(10, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [RegularExpression("[0-9]{5}-[0-9]{3}", ErrorMessage = "Utilize o formato 00000-000.")]
        public string EN_Cep { get; set; }

        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_Telefone { get; set; }

        [StringLength(25, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)?(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 44 4444-4444 | 99 99999-9999.")]
        public string TX_Fax { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string TX_Email { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string NM_Contato { get; set; }

        [StringLength(3, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string FL_Tipo_Empresa { get; set; }

        public string cdsFL_Tipo_Empresa { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public Dictionary<string, string> TipoEmpresa { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Contato : BaseModel
    {
        private TipoContatoEntity _TipoContato = null;

        private EmpresaEntity _Empresa = null;

        private ClienteEntity _Cliente = null;

        public Int64 nidContato { get; set; }

        [StringLength(250, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cnmContato { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cnmApelido { get; set; }

        public TipoContatoEntity tipoContato
        {
            get
            {
                if (_TipoContato == null) _TipoContato = new TipoContatoEntity();
                return _TipoContato;
            }
            set
            {
                if (_TipoContato == null) _TipoContato = new TipoContatoEntity();
                _TipoContato = value;
            }
        }

        [StringLength(250, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string cdsEmail { get; set; }

        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        public EmpresaEntity empresa
        {
            get
            {
                if (_Empresa == null) _Empresa = new EmpresaEntity();
                return _Empresa;
            }
            set
            {
                if (_Empresa == null) _Empresa = new EmpresaEntity();
                _Empresa = value;
            }
        }

        [StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado! Utilize o formato 00.")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)$", ErrorMessage = "Utilize o formato 00.")]
        public string cdsDDDTelefoneCel { get; set; }

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression(@"^(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 99999-9999.")]
        public string cdsTelefoneCel { get; set; }

        [StringLength(2, ErrorMessage = "Limite de caracteres ultrapassado! Utilize o formato 00.")]
        //[RegularExpression(@"^(?<areaCode>[(]?\d{1,3}[)]?\s?)$", ErrorMessage = "Utilize o formato 00.")]
        public string cdsDDDTelefone2 { get; set; }

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [RegularExpression(@"^(?<numero>\d{3,5}[-]?\d{4})$", ErrorMessage = "Utilize o formato 4444-4444.")]
        public string cdsTelefone2 { get; set; }

        [DataType(DataType.MultilineText)]
        public string cdsObservacoes { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string cdsCargo { get; set; }

        public ClienteEntity cliente
        {
            get
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                return _Cliente;
            }
            set
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                _Cliente = value;
            }
        }

        public List<TipoContato> tiposContatos { get; set; }

        public List<Empresa> empresas { get; set; }

        public List<Cliente> clientes { get; set; }

    }
}
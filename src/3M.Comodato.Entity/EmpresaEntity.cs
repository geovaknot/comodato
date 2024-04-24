using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{

    public class EmpresaEntity : BaseEntity
    {
        public Int64 CD_Empresa { get; set; }
        //public string IIComp { get; set; }
        public string NM_Empresa { get; set; }
        public string NR_Cnpj { get; set; }
        public string EN_Endereco { get; set; }
        public string EN_Bairro { get; set; }
        public string EN_Cidade { get; set; }
        public string EN_Estado { get; set; }
        public string EN_Cep { get; set; }
        public string TX_Telefone { get; set; }
        public string TX_Fax { get; set; }
        public string NM_Contato { get; set; }
        public string TX_Email { get; set; }
        public string FL_Tipo_Empresa { get; set; }
    }
}

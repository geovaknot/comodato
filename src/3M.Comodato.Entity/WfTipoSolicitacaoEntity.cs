using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class WfTipoSolicitacaoEntity: BaseEntity
    {
        public Int64 ID_TIPO_SOLICITACAO { get; set; }
        public Int64 CD_TIPO_SOLICITACAO { get; set; }
        public string DS_TIPO_SOLICITACAO { get; set; }
        public string FL_ATIVO { get; set; }
    }
}

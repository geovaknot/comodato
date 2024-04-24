using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class ConsumivelEntity : BaseEntity
    {
        private LinhaProdutoEntity _LinhaProdutoEntity = null;

        public string CD_CONSUMIVEL { get; set; }
        public string DS_CONSUMIVEL { get; set; }
        public string TX_UNIDADE { get; set; }
        public string CD_COMMODITY { get; set; }
        public string DS_COMMODITY { get; set; }
        public string CD_MAJOR { get; set; }
        public string DS_MAJOR { get; set; }
        public string CD_FAMILY { get; set; }
        public string DS_FAMILY { get; set; }
        public string CD_SUB_FAMILY { get; set; }
        public string DS_SUB_FAMILY { get; set; }
        public string TX_UNIDADE_CONV { get; set; }
        public bool? BPCS { get; set; }
        public bool ST_ATIVO { get; set; }

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
    }
}

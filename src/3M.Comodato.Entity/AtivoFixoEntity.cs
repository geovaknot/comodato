using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class AtivoFixoEntity : BaseEntity
    {
        private ClienteEntity _ClienteEntity = null;
        private ModeloEntity _ModeloEntity = null;
        private StatusAtivoEntity _StatusAtivoEntity = null;
        private SituacaoAtivoEntity _SituacaoAtivoEntity = null;
        private LinhaProdutoEntity _LinhaProdutoEntity = null;
        private DepreciacaoEntity _DepreciacaoEntity = null;

        public string CD_ATIVO_FIXO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }

        public string DS_ATIVO_FIXO_COMPLETA { get; set; }
        public DateTime? DT_INCLUSAO { get; set; }
        public DateTime? DT_FIM_GARANTIA { get; set; }
        public DateTime? DT_MANUTENCAO { get; set; }
        public DateTime? DT_FIM_MANUTENCAO { get; set; }

        public string TX_ANO_MAQUINA { get; set; }
        public string DS_MOTIVO { get; set; }

        public DateTime? DT_INVENTARIO { get; set; }
        public string TX_TIPO { get; set; }
        public bool? FL_STATUS { get; set; }

        public ClienteEntity cliente
        {
            get
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                return _ClienteEntity;
            }
            set
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                _ClienteEntity = value;
            }
        }

        public ModeloEntity modelo
        {
            get
            {
                if (_ModeloEntity == null) _ModeloEntity = new ModeloEntity();
                return _ModeloEntity;
            }
            set
            {
                if (_ModeloEntity == null) _ModeloEntity = new ModeloEntity();
                _ModeloEntity = value;
            }
        }

        public StatusAtivoEntity statusAtivo
        {
            get
            {
                if (_StatusAtivoEntity == null) _StatusAtivoEntity = new StatusAtivoEntity();
                return _StatusAtivoEntity;
            }
            set
            {
                if (_StatusAtivoEntity == null) _StatusAtivoEntity = new StatusAtivoEntity();
                _StatusAtivoEntity = value;
            }
        }

        public SituacaoAtivoEntity situacaoAtivo
        {
            get
            {
                if (_SituacaoAtivoEntity == null) _SituacaoAtivoEntity = new SituacaoAtivoEntity();
                return _SituacaoAtivoEntity;
            }
            set
            {
                if (_SituacaoAtivoEntity == null) _SituacaoAtivoEntity = new SituacaoAtivoEntity();
                _SituacaoAtivoEntity = value;
            }
        }

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

        public DepreciacaoEntity depreciacao
        {
            get
            {
                if (_DepreciacaoEntity == null) _DepreciacaoEntity = new DepreciacaoEntity();
                return _DepreciacaoEntity;
            }
            set
            {
                if (_DepreciacaoEntity == null) _DepreciacaoEntity = new DepreciacaoEntity();
                _DepreciacaoEntity = value;
            }
        }
    }

    public class AtivoFixoSinc : BaseEntity
    {
        public String CD_ATIVO_FIXO { get; set; }
        public String CD_MODELO { get; set; }
        public DateTime DT_INCLUSAO { get; set; }
        public String TX_ANO_MAQUINA { get; set; }
        public DateTime DT_INVENTARIO { get; set; }
        public Int32 CD_STATUS_ATIVO { get; set; }
        public Int32 CD_SITUACAO_ATIVO { get; set; }
        public String TX_TIPO { get; set; }
        public Int32 CD_LINHA_PRODUTO { get; set; }
        public bool FL_STATUS { get; set; }
        public DateTime DT_FIM_GARANTIA { get; set; }
        public DateTime DT_MANUTENCAO { get; set; }
        public DateTime DT_FIM_MANUTENCAO { get; set; }

        public DateTime? Data_ultimaOS { get; set;}
        public DateTime? last_order { get; set; }
        
    }

}

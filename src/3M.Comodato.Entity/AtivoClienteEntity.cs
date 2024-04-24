using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class AtivoClienteEntity : BaseEntity
    {
        private ClienteEntity _ClienteEntity = null;
        private AtivoFixoEntity _AtivoFixoEntity = null;
        private MotivoDevolucaoEntity _MotivoDevolucaoEntity = null;
        private RazaoComodatoEntity _RazaoComodatoEntity = null;
        private TipoEntity _TipoEntity = null;

        public Int64 ID_ATIVO_CLIENTE { get; set; }
        public DateTime? DT_NOTAFISCAL { get; set; }
        public Int64 NR_NOTAFISCAL { get; set; }
        public DateTime? DT_DEVOLUCAO { get; set; }
        public string TX_OBS { get; set; }
        public decimal VL_ALUGUEL { get; set; }
        public string TX_TERMOPGTO { get; set; }
        public int QTD_MESES_LOCACAO { get; set; }
        public DateTime? DT_FIM_GARANTIA_REFORMA { get; set; }

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

        public AtivoFixoEntity ativoFixo
        {
            get
            {
                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                return _AtivoFixoEntity;
            }
            set
            {
                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                _AtivoFixoEntity = value;
            }
        }

        public MotivoDevolucaoEntity motivoDevolucao
        {
            get
            {
                if (_MotivoDevolucaoEntity == null) _MotivoDevolucaoEntity = new MotivoDevolucaoEntity();
                return _MotivoDevolucaoEntity;
            }
            set
            {
                if (_MotivoDevolucaoEntity == null) _MotivoDevolucaoEntity = new MotivoDevolucaoEntity();
                _MotivoDevolucaoEntity = value;
            }
        }

        public RazaoComodatoEntity razaoComodato
        {
            get
            {
                if (_RazaoComodatoEntity == null) _RazaoComodatoEntity = new RazaoComodatoEntity();
                return _RazaoComodatoEntity;
            }
            set
            {
                if (_RazaoComodatoEntity == null) _RazaoComodatoEntity = new RazaoComodatoEntity();
                _RazaoComodatoEntity = value;
            }
        }

        public TipoEntity tipo
        {
            get
            {
                if (_TipoEntity == null) _TipoEntity = new TipoEntity();
                return _TipoEntity;
            }
            set
            {
                if (_TipoEntity == null) _TipoEntity = new TipoEntity();
                _TipoEntity = value;
            }
        }

        public string DS_ARQUIVO_FOTO { get; set; }
        public string DS_ARQUIVO_FOTO2 { get; set; }
    }

    /// <summary>
    /// Modela o Objeto utilizado para sincronismo de dados com o Mobile
    /// </summary>
    public class AtivoClienteSinc : BaseEntity
    {
        public Int64 ID_ATIVO_CLIENTE { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string CD_ATIVO_FIXO { get; set; }
        public DateTime? DT_NOTAFISCAL { get; set; }
        public Int64 NR_NOTAFISCAL { get; set; }
        public DateTime? DT_DEVOLUCAO { get; set; }
        public DateTime? DT_ULTIMA_MANUTENCAO { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string TX_OBS { get; set; }
        public Int32 CD_RAZAO { get; set; }
        public Int32 CD_TIPO { get; set; }
        public string DS_TIPO { get; set; }
        public decimal VL_ALUGUEL { get; set; }
        public string TX_TERMOPGTO { get; set; }
        public DateTime? DT_FIM_GARANTIA_REFORMA { get; set; }

    }
}

using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class OsPadrao : BaseModel
    {
        private TpStatusOSPadraoEntity _tpStatusOSEntity = null;

        private TpOSPadraoEntity _tpOSEntity = null;

        private ClienteEntity _clienteEntity = null;

        private TecnicoEntity _tecnicoEntity = null;

        private RegiaoEntity _regiaoEntity = null;

        private AtivoFixoEntity _AtivoFixoEntity = null;

        public long ID_OS { get; set; }

        public long? CodigoCli { get; set; }

        public string ID_OS_Formatado { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public DateTime? DT_DATA_OS { get; set; }

        public string DATA_OS_FORMATADA { get; set; }
        public string GRUPO_MODELO_ATIVO { get; set; }

        public int QT_PERIODO { get; set; }

        public decimal QT_PERIODO_REALIZADO { get; set; }

        public string QT_PERIODO_REALIZADO_FORMATADO { get; set; }

        public string PERCENTUAL { get; set; }

        public string HR_INICIO { get; set; }

        public string HR_FIM { get; set; }

        public string EMAIL { get; set; }

        public string DS_RESPONSAVEL { get; set; }
        public string Origem { get; set; }

        public string NOME_LINHA { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_OBSERVACAO { get; set; }

        public string HR_TOTAL
        {
            get
            {
                try
                {
                    var startTime = new TimeSpan(Convert.ToInt32(HR_INICIO.Split(':')[0]), Convert.ToInt32(HR_INICIO.Split(':')[1]), 0);
                    var endTime = new TimeSpan(Convert.ToInt32(HR_FIM.Split(':')[0]), Convert.ToInt32(HR_FIM.Split(':')[1]), 0);
                    return endTime.Subtract(startTime).ToString("hhmm").Insert(2, ":");
                }
                catch
                {
                    return "00:00";
                }
            }
        }

        public string CodigoTecnico { get; set; }

        public TpStatusOSPadraoEntity TpStatusOS
        {
            get
            {
                if (_tpStatusOSEntity == null)
                    _tpStatusOSEntity = new TpStatusOSPadraoEntity();
                return _tpStatusOSEntity;
            }
            set
            {
                if (_tpStatusOSEntity == null)
                    _tpStatusOSEntity = new TpStatusOSPadraoEntity();
                _tpStatusOSEntity = value;
            }
        }

        public TpOSPadraoEntity TpOS
        {
            get
            {
                if (_tpOSEntity == null)
                    _tpOSEntity = new TpOSPadraoEntity();
                return _tpOSEntity;
            }
            set
            {
                if (_tpOSEntity == null)
                    _tpOSEntity = new TpOSPadraoEntity();
                _tpOSEntity = value;
            }
        }

        public ClienteEntity cliente
        {
            get
            {
                if (_clienteEntity == null)
                    _clienteEntity = new ClienteEntity();
                return _clienteEntity;
            }
            set
            {
                if (_clienteEntity == null)
                    _clienteEntity = new ClienteEntity();
                _clienteEntity = value;
            }
        }

        public TecnicoEntity tecnico
        {
            get
            {
                if (_tecnicoEntity == null)
                    _tecnicoEntity = new TecnicoEntity();
                return _tecnicoEntity;
            }
            set
            {
                if (_tecnicoEntity == null)
                    _tecnicoEntity = new TecnicoEntity();
                _tecnicoEntity = value;
            }
        }

        public RegiaoEntity regiao
        {
            get
            {
                if (_regiaoEntity == null) _regiaoEntity = new RegiaoEntity();
                return _regiaoEntity;
            }
            set
            {
                if (_regiaoEntity == null) _regiaoEntity = new RegiaoEntity();
                _regiaoEntity = value;
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

        public PendenciaOS pendenciaOS { get; set; }

        public PecaOS pecaOS { get; set; }

        public ReclamacaoOs reclamacaoOs { get; set; }

        public List<ListaAtivoCliente> ativos { get; set; }

        public List<TpStatusOSPadraoEntity> tiposStatusOsPadrao { get; set; }

        public List<TpOSPadraoEntity> tiposOsPadrao { get; set; }

        public List<Cliente> clientes { get; set; }

        public List<Tecnico> tecnicos { get; set; }

        public List<Regiao> regioes { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Models
{
    public class AtivoCliente : BaseModel
    {
        private ClienteEntity _ClienteEntity = null;
        private AtivoFixoEntity _AtivoFixoEntity = null;
        private MotivoDevolucaoEntity _MotivoDevolucaoEntity = null;
        private RazaoComodatoEntity _RazaoComodatoEntity = null;
        private TipoEntity _TipoEntity = null;

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public Int64 ID_ATIVO_CLIENTE { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_NOTAFISCAL { get; set; }

        [Range(0, Int64.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public Int64 NR_NOTAFISCAL { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DEVOLUCAO { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_FIM_GARANTIA_REFORMA { get; set; }

        public DateTime? DT_DEVOLUCAO_GRID { get; set; }

        [StringLength(255, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string TX_OBS { get; set; }

        [StringLength(21, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string VL_ALUGUEL { get; set; }

        [StringLength(5, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string TX_TERMOPGTO { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int QTD_MESES_LOCACAO { get; set; }

        public DadosFaturamento dadosFaturamento { get; set; }

        public DadosPagamento dadosPagamento { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
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

        public List<Cliente> clientes { get; set; }

        public List<Ativo> ativosFixos { get; set; }

        public List<MotivoDevolucao> motivosDevolucoes { get; set; }

        public List<RazaoComodato> razoesComodatos { get; set; }

        public List<Tipo> tipos { get; set; } = new List<Tipo>();

        public string TP_ACAO { get; set; }

        public string Mensagem { get; set; }

        public string DS_ARQUIVO_FOTO { get; set; }
        public string DS_ARQUIVO_FOTO2 { get; set; }
        public string DT_SUGESTAO { get; internal set; }

        //public string DT_FIM_GARANTIA_REFORMA { get; set; }
    }

    public class ListaAtivoCliente
    {
        public string cdsPrograma { get; set; }
        public string cdsTipo { get; set; }
        public DateTime? DT_INCLUSAO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }
        public string NR_NOTAFISCAL { get; set; }

        public DateTime? DT_NOTAFISCAL { get; set; }
        public DateTime? DT_DEVOLUCAO { get; set; }
        public DateTime? DT_ULTIMA_MANUTENCAO { get; set; }
        public DateTime? DT_FIM_GARANTIA_REFORMA { get; set; }

        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string DS_MOTIVO_DEVOLUCAO { get; set; }

        public string CD_ATIVO_FIXO { get; set; }
        //public string DS_MODELO{get;set;
    }
}
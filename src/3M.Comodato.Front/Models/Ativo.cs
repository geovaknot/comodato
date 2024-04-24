using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Models
{
    public class Ativo : BaseModel
    {
        private ModeloEntity _Modelo = null;
        private StatusAtivoEntity _StatusAtivo = null;
        private SituacaoAtivoEntity _SituacaoAtivo = null;
        private LinhaProdutoEntity _LinhaProdutoEntity = null;
        private Depreciacao _Depreciacao = null;
        private int _QTD_DIAS_GARANTIA = 0;
        private int _QTD_DIAS_MANUTENCAO = 0;

        [StringLength(6, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Ativo", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já castrado!")]
        public string CD_ATIVO_FIXO { get; set; }

        public ModeloEntity modelo
        {
            get
            {
                if (_Modelo == null) _Modelo = new ModeloEntity();
                return _Modelo;
            }
            set
            {
                if (_Modelo == null) _Modelo = new ModeloEntity();
                _Modelo = value;
            }
        }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_INCLUSAO { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(0, 2100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int TX_ANO_MAQUINA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_INVENTARIO { get; set; }

        public StatusAtivoEntity statusAtivo
        {
            get
            {
                if (_StatusAtivo == null) _StatusAtivo = new StatusAtivoEntity();
                return _StatusAtivo;
            }
            set
            {
                if (_StatusAtivo == null) _StatusAtivo = new StatusAtivoEntity();
                _StatusAtivo = value;
            }
        }

        public SituacaoAtivoEntity situacaoAtivo
        {
            get
            {
                if (_SituacaoAtivo == null) _SituacaoAtivo = new SituacaoAtivoEntity();
                return _SituacaoAtivo;
            }
            set
            {
                if (_SituacaoAtivo == null) _SituacaoAtivo = new SituacaoAtivoEntity();
                _SituacaoAtivo = value;
            }
        }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string TX_TIPO { get; set; }

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

        public Depreciacao depreciacao
        {
            get
            {
                if (_Depreciacao == null) _Depreciacao = new Depreciacao();
                return _Depreciacao;
            }
            set
            {
                if (_Depreciacao == null) _Depreciacao = new Depreciacao();
                _Depreciacao = value;
            }
        }

        public List<Modelo> modelos { get; set; }

        public List<StatusAtivo> statusAtivos { get; set; }

        public List<SituacaoAtivo> situacoesAtivos { get; set; }

        public List<LinhaProduto> linhasProdutos { get; set; }

        public bool CancelarVerificarCodigo { get; set; }

        public bool FL_STATUS { get; set; }

        public string cdsFL_STATUS { get; set; }

        public string DS_ATIVO_FIXO { get; set; }

        public string TP_ACAO { get; set; }

        public string DT_DEVOLUCAO { get; set; }

        public DateTime? DT_DEVOLUCAO_GRID { get; set; }

        public string NM_CLIENTE { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_MANUTENCAO { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_FIM_MANUTENCAO { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_GARANTIA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_FIM_GARANTIA { get; set; }

        public string DS_MOTIVO { get; set; }
              


        public int QTD_DIAS_GARANTIA { get; set; }
        //{
        //    get
        //    {
        //        if (_QTD_DIAS_GARANTIA == 0 && DT_FIM_GARANTIA != null && DT_INCLUSAO != null)
        //        { _QTD_DIAS_GARANTIA = (DateTime.Parse(DT_FIM_GARANTIA).Subtract(DateTime.Parse(DT_INCLUSAO))).Days; }
        //        else
        //        {   _QTD_DIAS_GARANTIA = System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasGarantia"));

        //            DateTime Var_Dias = Convert.ToDateTime(DT_INCLUSAO);
        //            Var_Dias = Var_Dias.AddDays(_QTD_DIAS_GARANTIA);

        //            //var X = DateTime.Now.AddDays(_QTD_DIAS_GARANTIA);

        //            DT_FIM_GARANTIA = Var_Dias.ToShortDateString();
        //        }
        //        return _QTD_DIAS_GARANTIA;
        //    }
        //    set
        //    {
        //        if (_QTD_DIAS_GARANTIA == 0) _QTD_DIAS_GARANTIA = System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasGarantia")); // (DateTime.Parse(DT_FIM_GARANTIA).Subtract(DateTime.Parse(DT_INCLUSAO))).Days;
        //        _QTD_DIAS_GARANTIA = value ;
        //    }
        //}
        public int QTD_DIAS_MANUTENCAO { get; set; }
        //{
        //    get
        //    {
        //        if (_QTD_DIAS_MANUTENCAO == 0 && DT_FIM_MANUTENCAO != null && DT_MANUTENCAO != null)
        //        { _QTD_DIAS_MANUTENCAO = (DateTime.Parse(DT_FIM_MANUTENCAO).Subtract(DateTime.Parse(DT_MANUTENCAO))).Days; }
        //        else
        //        {
        //            _QTD_DIAS_MANUTENCAO = System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasManutencao"));

        //            if (DT_MANUTENCAO != null)
        //            {
        //                DateTime Var_Dias = Convert.ToDateTime(DT_MANUTENCAO);
        //                Var_Dias = Var_Dias.AddDays(_QTD_DIAS_MANUTENCAO);
        //                DT_FIM_MANUTENCAO = Var_Dias.ToShortDateString();
        //            }
        //        }
        //        return _QTD_DIAS_MANUTENCAO;
        //    }
        //    set
        //    {
        //        if (QTD_DIAS_MANUTENCAO == 0) QTD_DIAS_MANUTENCAO = System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasManutencao")); // (DateTime.Parse(DT_FIM_GARANTIA).Subtract(DateTime.Parse(DT_INCLUSAO))).Days;
        //        _QTD_DIAS_MANUTENCAO = value;
        //    }
        //}
    }
}
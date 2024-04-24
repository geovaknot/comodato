using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Models
{
    public class WfEquipamentoItemDevolucao
    {
        public long CodigoEquipamento { get; set; }
        public string NumeroAtivo { get; set; }
        public string Envio { get; set; }
        public string Modelo { get; set; }
        public string CaixaLargura { get; set; }
        public string CaixaAltura { get; set; }
        public string CaixaComprimento { get; set; }
        public string Peso { get; set; }
        public string NotaFiscal { get; set; }
        public string DS_ARQUIVO { get; set; }
        public long ST_STATUS_PEDIDO { get; set; }

        //public bool ReadOnly =>
        //    this.ST_STATUS_PEDIDO != (long)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho &&
        //    this.ST_STATUS_PEDIDO != (long)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.PendenteAnexar;
    }
}
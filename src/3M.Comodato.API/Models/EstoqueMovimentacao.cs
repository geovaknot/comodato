using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class EstoqueMovimentacao : BaseModel
    {
        public string ID_ESTOQUE_MOVI { get; set; }

        public string DT_MOVIMENTACAO { get; set; }
        public string CD_PECA { get; set; }
        public decimal QT_PECA { get; set; }
        public string TP_ENTRADA_SAIDA { get; set; }
        //public Estoque EstoqueOrigem { get; set; } = new Estoque();
        //public Estoque EstoqueDestino { get; set; } = new Estoque();
        //public TpEstoqueMovi TpEstoqueMovi { get; set; } = new TpEstoqueMovi();
        //public Usuario usuario { get; set; } = new Usuario();
    }
}
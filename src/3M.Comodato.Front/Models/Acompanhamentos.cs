using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Acompanhamentos : BaseModel
    {
        private ClienteEntity _Cliente = null;

        public ClienteEntity cliente
        {
            get
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                return _Cliente;
            }
            set
            {
                if (_Cliente == null) _Cliente = new ClienteEntity();
                _Cliente = value;
            }
        }
        public List<Cliente> clientes { get; set; }

        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string CD_CONSUMIVEL { get; set; }
        public string DS_CONSUMIVEL { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public decimal PR_PRATICADO1 { get; set; }
        public decimal PR_PRATICADO2 { get; set; }
        public decimal PR_PRATICADO3 { get; set; }
        public decimal PR_PRATICADO4 { get; set; }
        public decimal PR_PRATICADO5 { get; set; }
        public decimal PR_PRATICADO6 { get; set; }
        public decimal PR_PRATICADO7 { get; set; }
        public decimal PR_PRATICADO8 { get; set; }
        public decimal PR_PRATICADO9 { get; set; }
        public decimal PR_PRATICADO10 { get; set; }
        public decimal PR_PRATICADO11 { get; set; }
        public decimal PR_PRATICADO12 { get; set; }
    }
}
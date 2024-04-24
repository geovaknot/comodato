using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Models
{
    public class TreinamentoEAD : BaseModel
    {
    
        public bool Visualizar{ get; set; }

        public bool Administracao  { get; set; }
        public bool Administracao2 { get; set; }
        public bool Administracao3 { get; set; }
        public bool ControleEstoque { get; set; }
        public bool Dashboard { get; set; }
        public bool WorkflowEnvio { get; set; }
        public bool WorkflowDevolucao { get; set; }
        public bool AppMobile { get; set; }
        public bool Clientes { get; set; }
        public bool UtilizacaoRelatorios { get; set; }
        public bool EquipeTecnica { get; set; }
        public bool Equipetecnica3M { get; set; }

        public string caminho { get; set; }
    }
}
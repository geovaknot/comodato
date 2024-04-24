using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class GeralManutencao : BaseModel
    {
        public string DT_INICIO { get; set; }
        public string DT_FIM { get; set; }

        public string filtroAtual { get; set; }

        public List<Cliente> clientes { get; set; }
        public List<Cliente> AllClientes { get; set; }
        public int[] ClientesSelecionados { get; set; }

        public List<Grupo> grupos { get; set; }
        public List<Grupo> AllGrupos { get; set; }
        public string[] GruposSelecionados { get; set; }

        public List<Modelo> modelos { get; set; }
        public List<Modelo> AllModelos { get; set; }
        public string[] ModelosSelecionados { get; set; }

        public List<Tecnico> tecnicos { get; set; }
        public List<Tecnico> AllTecnicos { get; set; }
        public string[] TecnicosSelecionados { get; set; }

        public List<Peca> pecas { get; set; }
        public List<Peca> AllPecas { get; set; }
        public string[] PecasSelecionados { get; set; }

        public List<Ativo> ativos { get; set; }
        public List<Ativo> AllAtivos { get; set; }
        public string[] AtivosSelecionados { get; set; }

    }
}
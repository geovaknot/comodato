using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class PlanoZero : BaseModel
    {
        public int nSequencia { get; set; }

        public long nidPlanoZero { get; set; }

        public string ccdPeca { get; set; }

        public Peca PecaModel { get; set; } = new Peca();

        public string ccdModelo { get; set; }

        public Modelo ModeloModel { get; set; } = new Modelo();

        public string ccdGrupoModelo { get; set; }

        public GrupoModelo GrupoModeloModel { get; set; } = new GrupoModelo();

        public string nqtPecaModelo { get; set; }

        public string nqtEstoqueMinimo { get; set; }

        public string ccdCriticidadeAbc { get; set; }

        public string nqtEstoqueF4 { get; set; }

        public string nqtEstoqueRec { get; set; }

        public string nPonderacao { get; set; }
    }
}
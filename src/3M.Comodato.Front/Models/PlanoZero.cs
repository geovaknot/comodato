using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
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
        private GrupoModeloEntity _GrupoModelo = null;

        public string nqtPecaModelo { get; set; }

        public string nqtEstoqueMinimo { get; set; }

        public string ccdCriticidadeAbc { get; set; }
        
        public string nqtEstoqueF4 { get; set; }

        public string nqtEstoqueRec { get; set; }

        public string qtPecaModelo { get; set; }

        public string nqtEstoqueATec { get; set; }

        public GrupoModeloEntity grupoModelo
        {
            get
            {
                if (_GrupoModelo == null) _GrupoModelo = new GrupoModeloEntity();
                return _GrupoModelo;
            }
            set
            {
                if (_GrupoModelo == null) _GrupoModelo = new GrupoModeloEntity();
                _GrupoModelo = value;
            }
        }
        public List<GrupoModelo> gruposModelos { get; set; }
    }
}
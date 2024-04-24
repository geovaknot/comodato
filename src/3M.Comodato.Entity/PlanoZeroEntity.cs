using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class PlanoZeroEntity : BaseEntity
    {
        public long nidPlanoZero { get; set; }

        public string ccdPeca {get;set;}

        public PecaEntity Peca { get; set; } = new PecaEntity();

        public string ccdModelo { get; set; }

        public ModeloEntity Modelo { get; set; } = new ModeloEntity();

        public string ccdGrupoModelo { get; set; }

        public GrupoModeloEntity grupoModelo { get; set; } = new GrupoModeloEntity();

        public Decimal nqtPecaModelo { get; set; }
        
        public Decimal nqtEstoqueMinimo { get; set; }

        public string ccdCriticidadeABC { get; set; }

        public Decimal nPonderacao { get; set; }
    }


    public class PlanoZeroSinc
    {
        public Int64 ID_PLANO_ZERO { get; set; }
        public String CD_PECA { get; set; }
        public String CD_MODELO { get; set; }
        public String CD_GRUPO_MODELO { get; set; }
        public Decimal QT_PECA_MODELO { get; set; }
        public Decimal NM_PERC_PONDERACAO { get; set; }
        public Decimal QT_ESTOQUE_MINIMO { get; set; }
        public Char CD_CRITICIDADE_ABC { get; set; }
        public Int64 ID_USU_RESPONSAVEL { get; set; }
        public DateTime DT_CRIACAO { get; set; }
    }

}

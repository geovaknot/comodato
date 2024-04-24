using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class ModeloEntity : BaseEntity
    {
        public string CD_MODELO { get; set; }
        public string DS_MODELO { get; set; }
        public string CD_MOD_NR12 { get; set; }
        public string CD_GRUPO_MODELO { get; set; }
        public string FL_ATIVO { get; set; }
        public string cdsFL_ATIVO { get; set; }

        public decimal VL_COMPLEXIDADE_EQUIP { get; set; }
        public string TP_EMPACOTAMENTO { get; set; }
        public decimal? VL_COMP_MIN { get; set; }
        public decimal? VL_COMP_MAX { get; set; }
        public decimal? VL_LARG_MIN { get; set; }
        public decimal? VL_LARG_MAX { get; set; }
        public decimal? VL_ALTUR_MIN { get; set; }
        public decimal? VL_ALTUR_MAX { get; set; }
        public decimal? VL_LARG_CAIXA { get; set; }
        public decimal? VL_ALTUR_CAIXA { get; set; }
        public decimal? VL_COMP_CAIXA { get; set; }
        public decimal? VL_PESO_CUBADO { get; set; }
        public decimal? VL_PROJETADO { get; set; }

        public CategoriaEntity CATEGORIA { get; set; } = new CategoriaEntity();
        public LinhaProdutoEntity LINHA_PRODUTO { get; set; } = new LinhaProdutoEntity();
        //public Int64 VL_COMPLEXIDADE_EQUIP { get; set; }
        //public Int64 VL_PROJETADO { get; set; }

    }

    public class ModeloSinc
    {
        public string CD_MODELO { get; set; }
        public string DS_MODELO { get; set; }
        public string CD_GRUPO_MODELO { get; set; }
        public string FL_ATIVO { get; set; }
        public Int64 VL_COMPLEXIDADE_EQUIP { get; set; }
        public Int64 VL_PROJETADO { get; set; }
    }

}

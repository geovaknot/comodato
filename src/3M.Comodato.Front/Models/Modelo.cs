using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Modelo : BaseModel
    {
        private GrupoModeloEntity _GrupoModeloEntity = null;

        [StringLength(15, ErrorMessage = "Limite de caracteres ultrapassado!")]
        //[Required(ErrorMessage = "Conteúdo obrigatório!")]
        [System.Web.Mvc.Remote("VerificarCodigo", "Modelo", AdditionalFields = "CancelarVerificarCodigo", ErrorMessage = "Código já castrado!")]
        public string CD_MODELO { get; set; }

        [StringLength(30, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DS_MODELO { get; set; }

        [StringLength(20, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string CD_MOD_NR12 { get; set; }        

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Conteúdo obrigatório!")]
        public Int64 VL_COMPLEXIDADE_EQUIP { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Conteúdo obrigatório!")]
        public Int64 VL_PROJETADO { get; set; }

        public List<GrupoModelo> gruposModelos { get; set; }

        [StringLength(1, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string FL_ATIVO { get; set; }

        public string cdsFL_ATIVO { get; set; }

        public Dictionary<string, string> SimNao { get; set; }

        public bool CancelarVerificarCodigo { get; set; }


        public string TP_EMPACOTAMENTO { get; set; }
        public Dictionary<string, string> TipoEmpacotamento { get; set; }
        public string VL_COMP_MIN { get; set; }
        public string VL_COMP_MAX { get; set; }
        public string VL_LARG_MIN { get; set; }
        public string VL_LARG_MAX { get; set; }
        public string VL_ALTUR_MIN { get; set; }
        public string VL_ALTUR_MAX { get; set; }
        public string VL_LARG_CAIXA { get; set; }
        public string VL_ALTUR_CAIXA { get; set; }
        public string VL_COMP_CAIXA { get; set; }
        public string VL_PESO_CUBADO { get; set; }

        public string ID_CATEGORIA { get; set; }
        public List<Categoria> categorias { get; set; }

        public string CD_LINHA_PRODUTO { get; set; }
        public List<LinhaProduto> produtos { get; set; }

        public GrupoModeloEntity grupoModelo
        {
            get
            {
                if (_GrupoModeloEntity == null) _GrupoModeloEntity = new GrupoModeloEntity();
                return _GrupoModeloEntity;
            }
            set
            {
                if (_GrupoModeloEntity == null) _GrupoModeloEntity = new GrupoModeloEntity();
                _GrupoModeloEntity = value;
            }
        }
    }

}
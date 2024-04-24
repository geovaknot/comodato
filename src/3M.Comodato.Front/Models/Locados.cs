using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Locados : BaseModel
    {
        private ClienteEntity _Cliente = null;
        private VendedorEntity _Vendedor = null;
        private GrupoEntity _Grupo = null;

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

        public VendedorEntity vendedor
        {
            get
            {
                if (_Vendedor == null) _Vendedor = new VendedorEntity();
                return _Vendedor;
            }
            set
            {
                if (_Vendedor == null) _Vendedor = new VendedorEntity();
                _Vendedor = value;
            }
        }

        public GrupoEntity grupo
        {
            get
            {
                if (_Grupo == null) _Grupo = new GrupoEntity();
                return _Grupo;
            }
            set
            {
                if (_Grupo == null) _Grupo = new GrupoEntity();
                _Grupo = value;
            }
        }

        public string CD_ATIVO_FIXO { get; set; }
        public string DS_MODELO { get; set; }
        public string DT_NOTAFISCAL { get; set; }
        //public string QTD_VENCIDOS { get; set; }
        public string NR_NOTAFISCAL { get; set; }
        public string DS_TIPO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string VL_ALUGUEL { get; set; }
        public string TX_TERMOPGTO { get; set; }
        //public string VENC_1 { get; set; }
        //public string VENC_2 { get; set; }
        //public string VENC_3 { get; set; }
        //public string VENC_4 { get; set; }
    }

    public class LocadosDetalhe : Locados
    {
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_INICIAL { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_FINAL { get; set; }

        public List<Cliente> clientes { get; set; }
        public List<Vendedor> vendedores { get; set; }
        public List<Grupo> grupos { get; set; }
    }
}
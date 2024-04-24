using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Inventarios : BaseModel
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

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DEVOLUCAO { get; set; }

        public string CD_ATIVO_FIXO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string DT_NOTAFISCAL { get; set; }
        public string NR_NOTAFISCAL { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string DS_MOTIVO_DEVOLUCAO { get; set; }
        public string ID_ATIVO_CLIENTE { get; set; }
        public string VL_RESIDUAL { get; set; }
        public Int64 CD_VENDEDOR { get; set; }
        public string NM_VENDEDOR { get; set; }
        public string CD_GRUPO { get; set; }
        public string DS_GRUPO { get; set; }
        public string DS_MODELO { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public string DS_SITUACAO_ATIVO { get; set; }
        public string DS_STATUS_ATIVO { get; set; }
    }

    public class InventariosDetalhe : Inventarios
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
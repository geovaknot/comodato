using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Pedidos : BaseModel
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
        public string DT_CRIACAO { get; set; }

        //public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        //public Int64 CD_VENDEDOR { get; set; }
        public string NM_VENDEDOR { get; set; }
        //public string CD_GRUPO { get; set; }
        public string DS_GRUPO { get; set; }
        public string NR_DOCUMENTO { get; set; }
        public string NM_TECNICO { get; set; }
        public string TX_OBS { get; set; }
        //public string CD_PECA { get; set; }
        public string DS_PECA { get; set; }
        public string DS_STATUS_PEDIDO { get; set; }
        public string QTD_SOLICITADA { get; set; }
        public string QTD_APROVADA { get; set; }
        public string TOTAL_ITEM { get; set; }
    }

    public class PedidosDetalhe : Pedidos
    {
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_INICIAL { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_FINAL { get; set; }

        public List<Cliente> clientes { get; set; }
        //public List<Vendedor> vendedores { get; set; }
        public List<Grupo> grupos { get; set; }

        private TecnicoEntity _TecnicoEntity = null;

        public TecnicoEntity tecnico
        {
            get
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                return _TecnicoEntity;
            }
            set
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                _TecnicoEntity = value;
            }
        }

        public List<Tecnico> tecnicos { get; set; }
    }
}
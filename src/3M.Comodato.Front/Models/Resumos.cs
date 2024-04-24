using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Resumos : BaseModel
    {
        private LinhaProdutoEntity _Produto = null;
        private ClienteEntity _Cliente = null;
        private VendedorEntity _Vendedor = null;
        private GrupoEntity _Grupo = null;
        private ExecutivoEntity _Executivo = null;
        private RegiaoEntity _Regiao = null;

        public LinhaProdutoEntity produto
        {
            get
            {
                if (_Produto == null) _Produto = new LinhaProdutoEntity();
                return _Produto;
            }
            set
            {
                if (_Produto == null) _Produto = new LinhaProdutoEntity();
                _Produto = value;
            }
        }

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

        public ExecutivoEntity executivo
        {
            get
            {
                if (_Executivo == null) _Executivo = new ExecutivoEntity();
                return _Executivo;
            }
            set
            {
                if (_Executivo == null) _Executivo = new ExecutivoEntity();
                _Executivo = value;
            }
        }

        public RegiaoEntity regiao
        {
            get
            {
                if (_Regiao == null) _Regiao = new RegiaoEntity();
                return _Regiao;
            }
            set
            {
                if (_Regiao == null) _Regiao = new RegiaoEntity();
                _Regiao = value;
            }
        }

        public Int64 CD_LINHA_PRODUTO { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public Int64 CD_VENDEDOR { get; set; }
        public string NM_VENDEDOR { get; set; }
        public string CD_GRUPO { get; set; }
        public string DS_GRUPO { get; set; }
        public Int64 CD_EXECUTIVO { get; set; }
        public string NM_EXECUTIVO { get; set; }
        public string CD_REGIAO { get; set; }
        public string DS_REGIAO { get; set; }
    }

    public class resumosDetalhe : Resumos
    {
        public List<LinhaProduto> produtos { get; set; }
        public List<Cliente> clientes { get; set; }
        public List<Vendedor> vendedores { get; set; }
        public List<Grupo> grupos { get; set; }
        public List<Regiao> regioes { get; set; }
        public List<Executivo> executivos { get; set; }
    }
}
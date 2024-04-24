using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Recolhidos : BaseModel
    {
        private ClienteEntity _Cliente = null;
        private AtivoFixoEntity _Ativo = null;

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

        public AtivoFixoEntity ativo
        {
            get
            {
                if (_Ativo == null) _Ativo = new AtivoFixoEntity();
                return _Ativo;
            }
            set
            {
                if (_Ativo == null) _Ativo = new AtivoFixoEntity();
                _Ativo = value;
            }
        }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_DEVOLUCAO { get; set; }

        public string CD_ATIVO_FIXO { get; set; }
        public string DS_ATIVO_FIXO { get; set; }
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public string CD_MOTIVO_DEVOLUCAO { get; set; }
        public string DS_MOTIVO_DEVOLUCAO { get; set; }
        public string ID_ATIVO_CLIENTE { get; set; }
    }

    public class RecolhidosDetalhe :Recolhidos
    {
        public string filtroAtual { get; set; }

        public List<Cliente> clientes { get; set; }
        public List<Cliente> AllClientes { get; set; }
        public int[] ClientesSelecionados { get; set; }

        public List<Modelo> modelos { get; set; }
        public List<Modelo> AllModelos { get; set; }
        public string[] ModelosSelecionados { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] //dd/MM/yyyy 00:00:00.000
        public string DT_DEV_INICIAL { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] //dd/MM/yyyy 23:59:59.000
        public string DT_DEV_FINAL { get; set; }
    }
}
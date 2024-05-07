using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class TecnicoXCliente : BaseModel
    {
        private ClienteEntity _Cliente = null;
        private TecnicoEntity _Tecnico = null;

        public List<Cliente> clientes { get; set; }
        public List<Tecnico> tecnicos { get; set; }

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

        public TecnicoEntity tecnico
        {
            get
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                return _Tecnico;
            }
            set
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                _Tecnico = value;
            }
        }
    }

    public class TecnicoXClienteDetalhe : TecnicoXCliente
    {
        //[Range(0, 9999, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public int? nvlQtdeTecnicos { get; set; }

        public int nvlQtdeEquipamentos { get; set; }

        //public List<ListaTecnicoXClienteEscala> listaTecnicoXClienteEscalas { get; set; }
    }

    public class ListaTecnicoXCliente : BaseModel
    {
        public Int64 CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }
        public Int64 nvlQtdeTecnicos { get; set; }
        public string NM_TECNICO_PRINCIPAL { get; set; }
        public Int64 QT_PERIODO { get; set; }
        public Int64 nvlCargaTecnica { get; set; }
    }

    public class ListaTecnicoXClienteEscala : BaseModel
    {
        private EmpresaEntity _Empresa = null;
        private TecnicoEntity _Tecnico = null;

        public int CD_ORDEM { get; set; }
        public string ID_ESCALA { get; set; }

        public EmpresaEntity empresa
        {
            get
            {
                if (_Empresa == null) _Empresa = new EmpresaEntity();
                return _Empresa;
            }
            set
            {
                if (_Empresa == null) _Empresa = new EmpresaEntity();
                _Empresa = value;
            }
        }

        public TecnicoEntity tecnico
        {
            get
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                return _Tecnico;
            }
            set
            {
                if (_Tecnico == null) _Tecnico = new TecnicoEntity();
                _Tecnico = value;
            }
        }

        public Int64 nvlCargaTecnica { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{

    public class ContatoEntity : BaseEntity
    {

        private TipoContatoEntity _TipoContatoEntity = null;
        private EmpresaEntity _EmpresaEntity = null;
        private ClienteEntity _ClienteEntity = null;

        public Int64 nidContato { get; set; }
        public string cnmContato { get; set; }
        public string cnmApelido { get; set; }
        public string cdsEmail { get; set; }
        public string cdsDDDTelefoneCel { get; set; }
        public string cdsTelefoneCel { get; set; }
        public string cdsDDDTelefone2 { get; set; }
        public string cdsTelefone2 { get; set; }
        public string cdsObservacoes { get; set; }
        public string cdsCargo { get; set; }

        public TipoContatoEntity tipoContato
        {
            get
            {
                if (_TipoContatoEntity == null) _TipoContatoEntity = new TipoContatoEntity();
                return _TipoContatoEntity;
            }
            set
            {
                if (_TipoContatoEntity == null) _TipoContatoEntity = new TipoContatoEntity();
                _TipoContatoEntity = value;
            }
        }

        public EmpresaEntity empresa
        {
            get
            {
                if (_EmpresaEntity == null) _EmpresaEntity = new EmpresaEntity();
                return _EmpresaEntity;
            }
            set
            {
                if (_EmpresaEntity == null) _EmpresaEntity = new EmpresaEntity();
                _EmpresaEntity = value;
            }
        }

        public ClienteEntity cliente
        {
            get
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                return _ClienteEntity;
            }
            set
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                _ClienteEntity = value;
            }
        }
    }
}

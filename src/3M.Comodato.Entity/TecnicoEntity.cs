using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class TecnicoEntity : BaseEntity
    {
        private UsuarioEntity _UsuarioCoordenadorEntity = null;
        private UsuarioEntity _UsuarioSupervisorEntity = null;
        private UsuarioEntity _UsuarioEntity = null;
        private EmpresaEntity _EmpresaEntity = null;

        public string CD_TECNICO { get; set; }
        public string NM_TECNICO { get; set; }
        public string EN_ENDERECO { get; set; }
        public string EN_BAIRRO { get; set; }
        public string EN_CIDADE { get; set; }
        public string EN_ESTADO { get; set; }
        public string EN_CEP { get; set; }
        public string TX_TELEFONE { get; set; }
        public string TX_FAX { get; set; }
        public string TX_EMAIL { get; set; }
        public string TP_TECNICO { get; set; }
        public decimal VL_CUSTO_HORA { get; set; }
        public string FL_ATIVO { get; set; }
        public string FL_FERIAS { get; set; }
        public string NM_REDUZIDO { get; set; }
        public string CD_BCPS { get; set; }

        public UsuarioEntity usuarioCoordenador
        {
            get
            {
                if (_UsuarioCoordenadorEntity == null) _UsuarioCoordenadorEntity = new UsuarioEntity();
                return _UsuarioCoordenadorEntity;
            }
            set
            {
                if (_UsuarioCoordenadorEntity == null) _UsuarioCoordenadorEntity = new UsuarioEntity();
                _UsuarioCoordenadorEntity = value;
            }
        }

        public UsuarioEntity usuarioSupervisorTecnico
        {
            get
            {
                if (_UsuarioSupervisorEntity == null) _UsuarioSupervisorEntity = new UsuarioEntity();
                return _UsuarioSupervisorEntity;
            }
            set
            {
                if (_UsuarioSupervisorEntity == null) _UsuarioSupervisorEntity = new UsuarioEntity();
                _UsuarioSupervisorEntity = value;
            }
        }

        public UsuarioEntity usuario
        {
            get
            {
                if (_UsuarioEntity == null) _UsuarioEntity = new UsuarioEntity();
                return _UsuarioEntity;
            }
            set
            {
                if (_UsuarioEntity == null) _UsuarioEntity = new UsuarioEntity();
                _UsuarioEntity = value;
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

    }


    public class TecnicoSinc
    {
        public String CD_TECNICO { get; set; }
        public String NM_TECNICO { get; set; }
        public String EN_ENDERECO { get; set; }
        public String EN_BAIRRO { get; set; }
        public String EN_CIDADE { get; set; }
        public String EN_ESTADO { get; set; }
        public String EN_CEP { get; set; }
        public String TX_TELEFONE { get; set; }
        public String TX_FAX { get; set; }
        public String TX_EMAIL { get; set; }
        public String TP_TECNICO { get; set; }
        public Int64 VL_CUSTO_HORA { get; set; }
        public String FL_ATIVO { get; set; }
        public Int64 ID_USUARIO_COORDENADOR { get; set; }
        public Int64 ID_USUARIO_SUPERVISOR { get; set; }
        public Int64 ID_USUARIO { get; set; }
        public Int64 CD_EMPRESA { get; set; }
    }
}

using System;

namespace _3M.Comodato.Entity
{
    public class UsuarioEntity : BaseEntity
    {
        public Int64 nidUsuario { get; set; }
        public string cnmNome { get; set; }
        public string cdsLogin { get; set; }
        public string cdsEmail { get; set; }
        public string cdsSenha { get; set; }
        public decimal cd_empresa { get; set; }
        public DateTime? dtmDataHoraTrocaLoginExterno { get; set; }
        public string ccdChaveAcessoTrocarSenha { get; set; }
    }

    public class UsuarioSinc
    {
        public Int64 nidUsuario { get; set; }
        public String cnmNome { get; set; }
        public String cdsLogin { get; set; }
        public String cdsEmail { get; set; }
        public String cdsSenha { get; set; }
        public bool bidAtivo { get; set; }
        public String ccdChaveAcessoTrocarSenha { get; set; }
        public DateTime? dtmDataHoraTrocaLoginExterno { get; set; }
        public Int64? nidUsuarioAtualizacao { get; set; }
        public DateTime? dtmDataHoraAtualizacao { get; set; }
        public Int64 cd_empresa { get; set; }
    }


}

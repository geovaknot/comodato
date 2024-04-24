using System;

namespace _3M.Comodato.Entity
{
    public abstract class BaseEntity
    {
        public Int64 nidUsuarioAtualizacao { get; set; }
        public DateTime dtmDataHoraAtualizacao { get; set; }
        public string action { get; set; }

        public bool? bidAtivo { get; set; }
    }
}

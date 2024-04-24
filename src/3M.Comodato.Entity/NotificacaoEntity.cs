using System;

namespace _3M.Comodato.Entity
{
    public class NotificacaoEntity
    {
        public long ID_NOTIFICACAO { get; set; }
        public string TITULO { get; set; }
        public string MENSAGEM { get; set; }
        public bool? LIDA { get; set; }
        public DateTime DATA_INCLUSAO { get; set; }
        public long ID_USUARIO { get; set; }
    }
}

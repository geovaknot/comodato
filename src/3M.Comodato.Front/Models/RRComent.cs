using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace _3M.Comodato.Front.Models
{
    public class RRComent : BaseModel
    {
        UsuarioEntity _UsuarioEntity = null;
        public Int64 ID_RR_COMMENT { get; set; }
        public Int64 ID_RELATORIO_RECLAMACAO { get; set; }

        [StringLength(255, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_COMENT { get; set; }
        public DateTime? DT_REGISTRO { get; set; }

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
    }
}
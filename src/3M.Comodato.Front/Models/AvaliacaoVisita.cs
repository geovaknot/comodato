using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class AvaliacaoVisita : BaseModel
    {
        private UsuarioEntity _UsuarioEntity = null;
        private VisitaEntity _VisitaEntity = null;

        public Int64 ID_AVALIACAO_VISITA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string DT_AVALIACAO_VISITA { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_AVALICACAO_VISITA { get; set; }

        public int NR_GRAU_AVALIACAO { get; set; }

        public int NR_GRAU_AVALIACAO_1 { get; set; }
        public int NR_GRAU_AVALIACAO_2 { get; set; }
        public int NR_GRAU_AVALIACAO_3 { get; set; }
        public int NR_GRAU_AVALIACAO_4 { get; set; }
        public int NR_GRAU_AVALIACAO_5 { get; set; }

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

        public VisitaEntity visita
        {
            get
            {
                if (_VisitaEntity == null) _VisitaEntity = new VisitaEntity();
                return _VisitaEntity;
            }
            set
            {
                if (_VisitaEntity == null) _VisitaEntity = new VisitaEntity();
                _VisitaEntity = value;
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class AvaliacaoVisitaEntity: BaseEntity
    {
        private UsuarioEntity _UsuarioEntity = null;
        private VisitaEntity _VisitaEntity = null;

        public Int64 ID_AVALIACAO_VISITA { get; set; }
        public DateTime? DT_AVALIACAO_VISITA { get; set; }
        public string DS_AVALIACAO_VISITA { get; set; }
        public decimal NR_GRAU_AVALIACAO { get; set; }

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

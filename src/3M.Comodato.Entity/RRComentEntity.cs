using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class RRComentEntity : BaseEntity
    {
        UsuarioEntity _UsuarioEntity = null;
        RelatorioReclamacaoEntity _RelatorioReclamacaoEntity = null;

        public long ID_RR_COMMENT { get; set; }
        //public long ID_WF_PEDIDO_EQUIP { get; set; }
        public string DS_COMENT { get; set; }

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

        public RelatorioReclamacaoEntity relatorioReclamacao
        {
            get
            {
                if (_RelatorioReclamacaoEntity == null) _RelatorioReclamacaoEntity = new RelatorioReclamacaoEntity();
                return _RelatorioReclamacaoEntity;
            }
            set
            {
                if (_RelatorioReclamacaoEntity == null) _RelatorioReclamacaoEntity = new RelatorioReclamacaoEntity();
                _RelatorioReclamacaoEntity = value;
            }
        }
    }

    public class RRComentSincEntity
    {
        public Int64 ID_RR_COMMENT { get; set; }
        public Int64 ID_RELATORIO_RECLAMACAO { get; set; }
        public String DS_COMENT { get; set; }
        public DateTime DT_REGISTRO { get; set; }
        public Decimal nidUsuarioAtualizacao { get; set; }

        public String IDENTIFICADOR_FK_ID_RR { get; set; }
    }
}

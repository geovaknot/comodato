using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Entity
{
    public class PedidosEntity
    {
        TecnicoEntity _TecnicoEntity = null;
        ClienteEntity _ClienteEntity = null;
        PecaEntity _PecaEntity = null;
        GrupoEntity _GrupoEntity = null;

        public DateTime? DT_INICIAL { get; set; }
        public DateTime? DT_FINAL { get; set; }
        public string DT_CRIACAO { get; set; }
        //public Int64 CD_CLIENTE { get; set; }
        //public string NM_CLIENTE { get; set; }
        //public Int64 CD_VENDEDOR { get; set; }
        //public string NM_VENDEDOR { get; set; }       
        //public string CD_GRUPO { get; set; }
        //public string DS_GRUPO { get; set; }
        public string NR_DOCUMENTO { get; set; }
        //public string NM_TECNICO { get; set; }
        public string TX_OBS { get; set; }
        //public string CD_PECA { get; set; }
        //public string DS_PECA { get; set; }
        public string DS_STATUS_PEDIDO { get; set; }
        public string QTD_SOLICITADA { get; set; }
        public string QTD_APROVADA { get; set; }
        public string TOTAL_ITEM { get; set; }

        public TecnicoEntity tecnico
        {
            get
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                return _TecnicoEntity;
            }
            set
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                _TecnicoEntity = value;
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

        public PecaEntity peca
        {
            get
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                return _PecaEntity;
            }
            set
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                _PecaEntity = value;
            }
        }

        public GrupoEntity grupo
        {
            get
            {
                if (_GrupoEntity == null) _GrupoEntity = new GrupoEntity();
                return _GrupoEntity;
            }
            set
            {
                if (_GrupoEntity == null) _GrupoEntity = new GrupoEntity();
                _GrupoEntity = value;
            }
        }
    }
}
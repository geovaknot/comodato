using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class VisitaPadrao : BaseModel
    {
        private TpStatusVisitaPadraoEntity _tpStatusVisitaEntity = null;
        private TpMotivoVisitaPadraoEntity _tpMotivoVisitaEntity = null;
        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;
        private RegiaoEntity _RegiaoEntity = null;

        public long ID_VISITA { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string DT_DATA_VISITA { get; set; }

        [StringLength(8000, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [DataType(DataType.MultilineText)]
        public string DS_OBSERVACAO { get; set; }

        public string HR_INICIO { get; set; }
        public string HR_FIM { get; set; }

        public string EMAIL { get; set; }
        public string DS_RESPONSAVEL { get; set; }
        public string Origem { get; set; }

        public List<Peca> listaPecas { get; set; }

        public string HR_TOTAL
        {
            get
            {
                try
                {
                    var startTime = new TimeSpan(Convert.ToInt32(HR_INICIO.Split(':')[0]), Convert.ToInt32(HR_INICIO.Split(':')[1]), 0);
                    var endTime = new TimeSpan(Convert.ToInt32(HR_FIM.Split(':')[0]), Convert.ToInt32(HR_FIM.Split(':')[1]), 0);
                    return endTime.Subtract(startTime).ToString("hhmm").Insert(2, ":");
                }
                catch
                {
                    return "00:00";
                }
            }
        }

        public string CodigoTecnico { get; set; }

        public TpStatusVisitaPadraoEntity TpStatusVisita
        {
            get
            {
                if (_tpStatusVisitaEntity == null) _tpStatusVisitaEntity = new TpStatusVisitaPadraoEntity();
                return _tpStatusVisitaEntity;
            }
            set
            {
                if (_tpStatusVisitaEntity == null) _tpStatusVisitaEntity = new TpStatusVisitaPadraoEntity();
                _tpStatusVisitaEntity = value;
            }
        }

        public TpMotivoVisitaPadraoEntity TpMotivoVisita
        {
            get
            {
                if (_tpMotivoVisitaEntity == null)
                    _tpMotivoVisitaEntity = new TpMotivoVisitaPadraoEntity();
                return _tpMotivoVisitaEntity;
            }
            set
            {
                if (_tpMotivoVisitaEntity == null)
                    _tpMotivoVisitaEntity = new TpMotivoVisitaPadraoEntity();
                _tpMotivoVisitaEntity = value;
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

        public RegiaoEntity regiao
        {
            get
            {
                if (_RegiaoEntity == null) _RegiaoEntity = new RegiaoEntity();
                return _RegiaoEntity;
            }
            set
            {
                if (_RegiaoEntity == null) _RegiaoEntity = new RegiaoEntity();
                _RegiaoEntity = value;
            }
        }

        public List<TpStatusVisitaPadraoEntity> tiposStatusVisitaPadrao { get; set; }
        public List<TpMotivoVisitaPadraoEntity> tiposMotivoVisitaPadrao { get; set; }
        public List<Cliente> clientes { get; set; }
        public List<Tecnico> tecnicos { get; set; }
        public List<Regiao> regioes { get; set; }

        public VisitaPadrao()
        {
   
            
        }
    }
}
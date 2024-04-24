using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class PecaOS: BaseModel
    {
        private OSEntity _OSEntity = null;
        private PecaEntity _PecaEntity = null;

        public Int64 ID_PECA_OS { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Somente valor positivo acima de 0(zero)!")]
        public string QT_PECA { get; set; }

        public decimal QTD_RECEBIDA_NAO_APROVADA { get; set; }

        public string CD_TP_ESTOQUE_CLI_TEC { get; set; }
        public string DS_TP_ESTOQUE_CLI_TEC { get; set; }

        public Dictionary<string, string> tiposEstoqueUtilizado { get; set; }

        public List<PecaEntity> listaPecas { get; set; }

        public string DS_OBSERVACAO { get; set; }

        public OSEntity OS
        {
            get
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                return _OSEntity;
            }
            set
            {
                if (_OSEntity == null) _OSEntity = new OSEntity();
                _OSEntity = value;
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
    }
}
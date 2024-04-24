using System;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class SoliticacaoAtendimento : BaseModel
    {
        public long CodigoSolicitacao { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string StatusSolicitacao { get; set; }

        public string TipoAtendimento { get; set; }
        public int QuantidadeEquipamento { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataVisita { get; set; }
        public long? ID_OS { get; set; }

        public string CD_ATIVO_FIXO { get; set; }

        public string UsuarioSolicitante { get; set; }

        [DataType(DataType.MultilineText)]
        public string Observacao { get; set; }

        public string NomeCliente { get; set; }
    }
}
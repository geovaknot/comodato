using System;
using System.ComponentModel.DataAnnotations;

namespace _3M.Comodato.Front.Models
{
    public class SatisfacaoResposta : BaseModel
    {
        public SatisfacaoPesquisa Pesquisa { get; set; } = new SatisfacaoPesquisa();
        public Visita Visita { get; set; } = new Visita();

        public string DataResposta { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public String NomeRespondedor { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public Decimal NotaPesquisa { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public String Justificativa { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa1 { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa2 { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa3 { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa4 { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string RespostaPesquisa5 { get; set; }

        public string NomeCliente
        {
            get
            {
                if (null != Visita)
                {
                    if (null != Visita.cliente)
                    {
                        return Visita.cliente.NM_CLIENTE;
                    }
                }
                return string.Empty;
            }
        }
    }
}
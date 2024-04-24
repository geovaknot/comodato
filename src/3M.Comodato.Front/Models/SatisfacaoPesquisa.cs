using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace _3M.Comodato.Front.Models
{
    public class SatisfacaoPesquisa : BaseModel
    {
        public long IdPesquisa { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string Titulo { get; set; }

        public string DataCriacao { get; set; }
        public string DataFinalizacao { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public int CodigoTipoPesquisa { get; set; }

        public string  TipoPesquisa => CodigoTipoPesquisa == 0|| !ControlesUtility.Dicionarios.TipoPesquisaSatisfacao().ContainsKey(this.CodigoTipoPesquisa)? "" : ControlesUtility.Dicionarios.TipoPesquisaSatisfacao()[this.CodigoTipoPesquisa];

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public Int64 nidUsuarioResponsavel { get; set; }
        public string UsuarioResponsavel { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public int StatusPesquisa { get; set; }

        public string StatusPesquisaDescricao => !ControlesUtility.Dicionarios.SituacaoPesquisa().ContainsKey(this.StatusPesquisa.ToString()) ? "" : ControlesUtility.Dicionarios.SituacaoPesquisa()[this.StatusPesquisa.ToString()];

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string DescricaoPesquisa { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [DataType(DataType.MultilineText)]
        public string PerguntaPesquisa1 { get; set; }

        [DataType(DataType.MultilineText)]
        public string PerguntaPesquisa2 { get; set; }

        [DataType(DataType.MultilineText)]
        public string PerguntaPesquisa3 { get; set; }

        [DataType(DataType.MultilineText)]
        public string PerguntaPesquisa4 { get; set; }

        [DataType(DataType.MultilineText)]
        public string PerguntaPesquisa5 { get; set; }

        public int QuantidadeVisitas { get; set; }

        public int QuantidadeRespostas => this.ListaRespostas.Count();
        public List<SatisfacaoResposta> ListaRespostas { get; set; } = new List<SatisfacaoResposta>();

        public decimal Percentual => (this.QuantidadeRespostas == 0 ? 0 : ((decimal)this.QuantidadeRespostas / (decimal)this.QuantidadeVisitas) * 100);
        public decimal IndiceSatisfacao => this.QuantidadeRespostas == 0 ? 0 : this.ListaRespostas.Sum(c => c.NotaPesquisa) / this.QuantidadeRespostas;
    }
}
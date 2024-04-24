using System;

namespace _3M.Comodato.Entity
{
    public class SatisfacaoPesquisaEntity : BaseEntity
    {
        public long ID_PESQUISA_SATISF { get; set; }
        public string DS_TITULO { get; set; }
        public int ST_STATUS_PESQUISA { get; set; }
        public DateTime DT_CRIACAO { get; set; }
        public DateTime DT_FINALIZACAO { get; set; }
        public int TP_PESQUISA { get; set; }
        public UsuarioEntity USUARIO_RESPONSAVEL { get; set; } = new UsuarioEntity();

        public string DS_DESCRICAO { get; set; }
        public string DS_PERGUNTA1 { get; set; }
        public string DS_PERGUNTA2 { get; set; }
        public string DS_PERGUNTA3 { get; set; }
        public string DS_PERGUNTA4 { get; set; }
        public string DS_PERGUNTA5 { get; set; }
    }

    public class SatisfacaoRespostaEntity : BaseEntity
    {
        public long ID_RESPOSTA_SATISF { get; set; }
        public SatisfacaoPesquisaEntity SatisfacaoPesquisa { get; set; } = new SatisfacaoPesquisaEntity();
        public VisitaEntity Visita { get; set; } = new VisitaEntity();

        public DateTime DT_DATA_RESPOSTA { get; set; }
        public decimal NM_NOTA_PESQ { get; set; }
        public string  DS_NOME_RESPONDEDOR { get; set; }
        public string DS_JUSTIFICATIVA { get; set; }
        public string DS_RESPOSTA1 { get; set; }
        public string DS_RESPOSTA2 { get; set; }
        public string DS_RESPOSTA3 { get; set; }
        public string DS_RESPOSTA4 { get; set; }
        public string DS_RESPOSTA5 { get; set; }
    }
}
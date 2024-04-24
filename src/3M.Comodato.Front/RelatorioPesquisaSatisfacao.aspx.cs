using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.ReportDataSources;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace _3M.Comodato.Front
{
    public partial class RelatorioPesquisaSatisfacao : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            long codigoPesquisa = long.Parse(idKey);
            Controllers.AnaliseSatisfacaoController analiseSatisfacao = new Controllers.AnaliseSatisfacaoController();

            ReportDataSourceResultadoPesquisa listaResultadoPesquisa = new ReportDataSourceResultadoPesquisa();
            SatisfacaoPesquisa model = analiseSatisfacao.ObterSatisfacaoPesquisa(codigoPesquisa);
            listaResultadoPesquisa.AddRange(model.ListaRespostas);

            if (listaResultadoPesquisa.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), @"Reports\AnaliseSatisfacao\ResultadoPesquisa.rdlc");
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("TituloPesquisa", model.Titulo));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("ResponsavelPesquisa", model.UsuarioResponsavel));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("DataCriacaoPesquisa", model.DataCriacao));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("StatusPesquisa", model.StatusPesquisaDescricao));
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsResultadoPesquisa", listaResultadoPesquisa));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }
    }
}
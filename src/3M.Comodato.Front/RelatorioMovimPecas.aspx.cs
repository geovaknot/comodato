using _3M.Comodato.Front.ReportDataSources;
using Microsoft.Reporting.WebForms;
using System.IO;
namespace _3M.Comodato.Front
{
    public partial class RelatorioMovimPecas : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            long codigoEstoque = long.Parse(idKey);
            Controllers.MovimPecasController movimPecas = new Controllers.MovimPecasController();

            ReportDataSourceMovimPecas listaMovimPecas = new ReportDataSourceMovimPecas();
            listaMovimPecas.AddRange(movimPecas.ConsultarEstoquePorCodigo(codigoEstoque, UsuarioAutenticado));

            if (listaMovimPecas.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), @"Reports\MovimPecas\MovimPecas.rdlc");
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsMovimPecas", listaMovimPecas));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }
    }
}

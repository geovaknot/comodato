using _3M.Comodato.Front.Reports.EstoqueIntermediario;
using _3M.Comodato.Front.Reports.EstoqueIntermediario.dsRptEstoqueIntermediarioTableAdapters;
using _3M.Comodato.Front.Reports.EstoqueIntermediario.dsRptEstoqueIntermediarioPecaTableAdapters;
using _3M.Comodato.Front.Reports.EstoqueIntermediario.dsRptEstoqueIntermediarioTecnicoTableAdapters;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.IO;
using _3M.Comodato.Data;

namespace _3M.Comodato.Front
{
    public partial class RelatorioEstoqueIntermediario : ReportBasePage
    {
        private enum Parametros
        {
            TipoRelatorio = 0,
            CodigoEstoque = 1,
            CodigoPeca = 2,
            EstoqueAtivo = 3,
            PecaAtiva = 4,
        }

        private DataSet dsConsultaEstoqueIntermediario = null;

        protected override void ExibirRelatorio()
        {
            var param = this.idKey.Split('|');

            string codigoEstoque = null;
            if (!string.IsNullOrEmpty(param[Parametros.CodigoEstoque.ToInt()]))
                codigoEstoque = param[Parametros.CodigoEstoque.ToInt()];

            string codigoPeca = null;
            if (!string.IsNullOrEmpty(param[Parametros.CodigoPeca.ToInt()]))
                codigoPeca = param[Parametros.CodigoPeca.ToInt()];

            string estoquesAtivos = null;
            if (!string.IsNullOrEmpty(param[Parametros.EstoqueAtivo.ToInt()]))
                estoquesAtivos = param[Parametros.EstoqueAtivo.ToInt()];

            string pecasAtivas = null;
            if (!string.IsNullOrEmpty(param[Parametros.PecaAtiva.ToInt()]))
                pecasAtivas = param[Parametros.PecaAtiva.ToInt()];

            //DataTable dtRelatorio = null;
            //string dataSet = string.Empty;

            if (param[Parametros.TipoRelatorio.ToInt()] == "P")
            {
                dsConsultaEstoqueIntermediario = new EstoqueData().ObterRelatorioEstoqueIntermediarioPorPeca(codigoPeca, pecasAtivas, codigoEstoque, estoquesAtivos);

                //Ordenação por data devolução
                //DataView Dw = new DataView(dsPedido.Tables[0]);
                //Dw.Sort = "DT_DEVOLUCAO";
                //dsPedido.Tables.Clear();
                //dsPedido.Tables.Add(Dw.Table);

                var reportDS = new ReportDataSource();
                reportDS.Name = "dsRptEstoqueIntermediarioPeca";
                reportDS.Value = dsConsultaEstoqueIntermediario.Tables[0];

                if (dsConsultaEstoqueIntermediario.Tables[0].Rows.Count == 0)
                {
                    pnlMensagem.Visible = true;
                    return;
                }
                else
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;

                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EstoqueIntermediario\EstoqueIntermediarioPorPeca.rdlc";
                //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();

                return;

                //dataSet = "dsRptEstoqueIntermediarioPeca";

                //dsRptEstoqueIntermediarioPeca.prcRptEstoqueIntermediarioPorPecaDataTable dtEstoquePorPeca = new dsRptEstoqueIntermediarioPeca.prcRptEstoqueIntermediarioPorPecaDataTable();
                //using (var adapter = new prcRptEstoqueIntermediarioPorPecaTableAdapter())
                //{
                //    adapter.Fill(dtEstoquePorPeca, codigoPeca, pecasAtivas, codigoEstoque, estoquesAtivos);
                //}
                //dtRelatorio = dtEstoquePorPeca as DataTable;

                //ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EstoqueIntermediario\EstoqueIntermediarioPorPeca.rdlc";
            }
            else
            {
                dsConsultaEstoqueIntermediario = new EstoqueData().ObterRelatorioEstoqueIntermediario(codigoPeca, pecasAtivas, codigoEstoque, estoquesAtivos);

                //Ordenação por data devolução
                //DataView Dw = new DataView(dsPedido.Tables[0]);
                //Dw.Sort = "DT_DEVOLUCAO";
                //dsPedido.Tables.Clear();
                //dsPedido.Tables.Add(Dw.Table);

                var reportDS = new ReportDataSource();
                reportDS.Name = "dsRptEstoqueIntermediario";
                reportDS.Value = dsConsultaEstoqueIntermediario.Tables[0];

                if (dsConsultaEstoqueIntermediario.Tables[0].Rows.Count == 0)
                {
                    pnlMensagem.Visible = true;
                    return;
                }
                else
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;

                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EstoqueIntermediario\EstoqueIntermediario.rdlc";
                //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();

                return;

                ////dataSet = "dsRptEstoqueIntermediarioTecnico";

                ////dsRptEstoqueIntermediarioTecnico.prcRptEstoqueIntermediarioPorTecnicoDataTable dtEstoquePorTecnico = new dsRptEstoqueIntermediarioTecnico.prcRptEstoqueIntermediarioPorTecnicoDataTable();
                ////using (var adapter = new prcRptEstoqueIntermediarioPorTecnicoTableAdapter())
                ////{
                ////    adapter.Fill(dtEstoquePorTecnico, codigoPeca, pecasAtivas, codigoTecnico, tecnicosAtivos);
                ////}
                ////dtRelatorio = dtEstoquePorTecnico as DataTable;

                ////ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EstoqueIntermediario\EstoqueIntermediarioPorTecnico.rdlc";

                //dataSet = "dsRptEstoqueIntermediario";

                //dsRptEstoqueIntermediario.prcRptEstoqueIntermediarioDataTable dtEstoque = new dsRptEstoqueIntermediario.prcRptEstoqueIntermediarioDataTable();
                //using (var adapter = new prcRptEstoqueIntermediarioTableAdapter())
                //{
                //    adapter.Fill(dtEstoque, codigoPeca, pecasAtivas, codigoEstoque, estoquesAtivos);
                //}
                //dtRelatorio = dtEstoque as DataTable;

                //ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EstoqueIntermediario\EstoqueIntermediario.rdlc";
            }

            //if (dtRelatorio.Rows.Count > 0)
            //{
            //    //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), $@"\Reports\EstoqueIntermediario\{(param[Parametros.TipoRelatorio.ToInt()] == "P" ? "EstoqueIntermediarioPorPeca" : "EstoqueIntermediarioPorTecnico")}.rdlc");
            //    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(dataSet, dtRelatorio));
            //    //parametrosRelatorio.ForEach(p => ReportViewer1.LocalReport.SetParameters(p));
            //    ReportViewer1.LocalReport.Refresh();
            //}
            //else
            //{
            //    pnlMensagem.Visible = true;
            //}

        }
    }
}
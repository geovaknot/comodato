using _3M.Comodato.Front.Reports.AnaliseConsumo.dsRptConsumoTableAdapters;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.IO;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace _3M.Comodato.Front
{
    public partial class RelatorioGeralManutencao : ReportBasePage
    {
        private DataSet dsGeralManutencao = null;

        protected override void ExibirRelatorio()
        {

            if (string.IsNullOrEmpty(idKey) == true)
            {
                pnlMensagem.Visible = true;
                return;
            }

            string arrayFiltros = parametros[0];
            DateTime? dtInicial = null;
            DateTime? dtFinal = null;
            string tipoRelatorio = parametros[3];

            string DataInicialParam = "";
            string DataFinalParam = "";
            if (!string.IsNullOrEmpty(parametros[1]))
            {
                dtInicial = Convert.ToDateTime(parametros[1]);
                DataInicialParam = parametros[1];
            }
            if (!string.IsNullOrEmpty(parametros[2]))
            {
                dtFinal = Convert.ToDateTime(parametros[2] + " 23:59:59");
                DataFinalParam = parametros[2];
            }

            if (string.IsNullOrEmpty(arrayFiltros))
            {
                pnlMensagem.Visible = true;
                return;
            }

            if (tipoRelatorio == "Tecnico")
                tipoRelatorio = "Técnico";
            else if (tipoRelatorio == "Peca")
                tipoRelatorio = "Peça";

            if (arrayFiltros == "Todos")
                arrayFiltros = string.Empty;

            var reportDS = new ReportDataSource();

            dsGeralManutencao = new GeralManutencaoData().ObterRelatorio(arrayFiltros, dtInicial, dtFinal, tipoRelatorio);

            reportDS.Name = "dtReportGeralManutencao";
            reportDS.Value = dsGeralManutencao.Tables[0];

            if (dsGeralManutencao.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
                return;
            }
            else
                ReportViewer1.ProcessingMode = ProcessingMode.Local;

            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Manutencao\ReportGeralManutencao.rdlc";
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.SetParameters(new ReportParameter("pTipoAgrupamento", tipoRelatorio));
            ReportViewer1.LocalReport.Refresh();

        }

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        string idKey = Request.QueryString["idKey"];
        //        GerarRelatorio(idKey);
        //    }
        //}

        //private void GerarRelatorio(string idKey)
        //{
        //    if (string.IsNullOrEmpty(idKey) == true)
        //    {
        //        pnlMensagem.Visible = true;
        //        return;
        //    }

        //    var param = ControlesUtility.Criptografia.Descriptografar(idKey).Split('|');
        //    //var param = idKey.Split('|');

        //    string arrayFiltros = param[0];
        //    DateTime? dtInicial = null;
        //    DateTime? dtFinal = null;
        //    string tipoRelatorio = param[3];

        //    if (!string.IsNullOrEmpty(param[1]))
        //        dtInicial = Convert.ToDateTime(param[1]);
        //    if (!string.IsNullOrEmpty(param[2]))
        //        dtFinal = Convert.ToDateTime(param[2] + " 23:59:59");

        //    if (string.IsNullOrEmpty(arrayFiltros))
        //    {
        //        pnlMensagem.Visible = true;
        //        return;
        //    }

        //    if (tipoRelatorio == "Tecnico")
        //        tipoRelatorio = "Técnico";
        //    else if (tipoRelatorio == "Peca")
        //        tipoRelatorio = "Peça";

        //    if (arrayFiltros == "Todos")
        //        arrayFiltros = string.Empty;

        //    dsGeralManutencao = new GeralManutencaoData().ObterRelatorio(arrayFiltros, dtInicial, dtFinal, tipoRelatorio);

        //    var reportDS = new ReportDataSource();
        //    reportDS.Name = "dtReportGeralManutencao";
        //    reportDS.Value = dsGeralManutencao.Tables[0];

        //    if (dsGeralManutencao.Tables[0].Rows.Count == 0)
        //    {
        //        pnlMensagem.Visible = true;
        //        return;
        //    }
        //    else
        //        ReportViewer1.ProcessingMode = ProcessingMode.Local;

        //    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Manutencao\ReportGeralManutencao.rdlc";
        //    //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
        //    ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //    ReportViewer1.LocalReport.SetParameters(new ReportParameter("pTipoAgrupamento", tipoRelatorio));
        //    //ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubrptProcessingEventHandler);
        //    ReportViewer1.LocalReport.Refresh();

        //    //DataTable dataTableRelatorio;
        //    //string rdlc = string.Empty;
        //    //string dataSetName = string.Empty;

        //    //if (tipoRelatorio == "Mensal")
        //    //{
        //    //    rdlc = "AnaliseConsumoMensal";
        //    //    using (var tableAdapter = new prcRptConsumoMensalTableAdapter())
        //    //    {
        //    //        using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
        //    //        {
        //    //            tableAdapter.Fill(dsRptConsumo.prcRptConsumoMensal, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo);
        //    //            dataTableRelatorio = dsRptConsumo.prcRptConsumoMensal as DataTable;
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    rdlc = "AnaliseConsumoCliente";
        //    //    using (var tableAdapter = new prcRptConsumoClienteTableAdapter())
        //    //    {
        //    //        using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
        //    //        {
        //    //            tableAdapter.Fill(dsRptConsumo.prcRptConsumoCliente, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo);
        //    //            dataTableRelatorio = dsRptConsumo.prcRptConsumoCliente as DataTable;
        //    //        }
        //    //    }
        //    //}

        //    //if (dataTableRelatorio.Rows.Count > 0)
        //    //{
        //    //    ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), $@"\Reports\AnaliseConsumo\{rdlc}.rdlc");
        //    //    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ConsumoMensal", dataTableRelatorio));
        //    //    ReportViewer1.LocalReport.SetParameters(new ReportParameter("VisaoRelatorio", visaoRelatorio));
        //    //}
        //    //else
        //    //{
        //    //    pnlMensagem.Visible = true;
        //    //}
        //}

        //void SubrptProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        //{
        //    ReportDataSource ds = new ReportDataSource("dtReportSolicitacaoPecasPedidoItem", dsPedido.Tables[2]);
        //    e.DataSources.Add(ds);

        //    ReportDataSource ds1 = new ReportDataSource("dtReportSolicitacaoPecasMensagem", dsPedido.Tables[1]);
        //    e.DataSources.Add(ds1);
        //}

        private void ReportViewer1_Load(object sender, EventArgs e)
        {
            SetVisibleExportOption("PDF, Excel");
        }

        /// <summary>
        /// Atribuir visibilidade false para opção de exportar
        /// </summary>
        /// <param name="exportOption">Excel, EXCELOPENXML, IMAGE, PDF, WORD ou WORDOPENXML</param>
        private void SetVisibleExportOption(string exportOption)
        {
            Action<RenderingExtension, bool> actionVisible = new Action<RenderingExtension, bool>((extension, visible) =>
            {
                if (extension != null)
                {
                    System.Reflection.FieldInfo fieldInfo = extension.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    fieldInfo.SetValue(extension, visible);
                }
            });

            var listExtensions = ReportViewer1.LocalReport.ListRenderingExtensions().ToList();
            listExtensions.ForEach(e => actionVisible(e, false));

            RenderingExtension extensionToShow = listExtensions.Find(x => x.Name.Equals(exportOption, StringComparison.CurrentCultureIgnoreCase));
            actionVisible(extensionToShow, true);
        }

    }
}
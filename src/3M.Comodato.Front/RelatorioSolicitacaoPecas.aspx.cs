using System;
using System.Data;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.Controllers;
using _3M.Comodato.Utility;
using static _3M.Comodato.Utility.ControlesUtility.Enumeradores;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _3M.Comodato.Front
{
    public partial class RelatorioSolicitacaoPecas : ReportBasePage
    {
        //private string idKey => Request.QueryString["IdKey"];
        private DataSet dsPedido = null;

        protected override void ExibirRelatorio()
        {
            Int64 ID_PEDIDO = Convert.ToInt64(parametros[0]); //Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));

            Int64 NR_LOTE;
            if (parametros[1] == "" || parametros[1] == null || parametros[1] == "0")
                NR_LOTE = 0;
            else
                NR_LOTE = Convert.ToInt64(parametros[1]);

            dsPedido = new PedidoData().ObterRelatorio(ID_PEDIDO, NR_LOTE);

            //Ordenação por data devolução
            //DataView Dw = new DataView(dsPedido.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //dsPedido.Tables.Clear();
            //dsPedido.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "dtReportSolicitacaoPecasPedido";
            reportDS.Value = dsPedido.Tables[0];

            if (dsPedido.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
                return;
            }
            else
                ReportViewer1.ProcessingMode = ProcessingMode.Local;

            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\SolicitacaoPecas\ReportSolicitacaoPecas.rdlc";
            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubrptProcessingEventHandler);
            ReportViewer1.LocalReport.Refresh();

        }

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        Int64 ID_PEDIDO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));

        //        dsPedido = new PedidoData().ObterRelatorio(ID_PEDIDO);

        //        //Ordenação por data devolução
        //        //DataView Dw = new DataView(dsPedido.Tables[0]);
        //        //Dw.Sort = "DT_DEVOLUCAO";
        //        //dsPedido.Tables.Clear();
        //        //dsPedido.Tables.Add(Dw.Table);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "dtReportSolicitacaoPecasPedido";
        //        reportDS.Value = dsPedido.Tables[0];

        //        if (dsPedido.Tables[0].Rows.Count == 0)
        //        {
        //            pnlMensagem.Visible = true;
        //            return;
        //        }
        //        else
        //            ReportViewer1.ProcessingMode = ProcessingMode.Local;

        //        ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\SolicitacaoPecas\ReportSolicitacaoPecas.rdlc";
        //        //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
        //        ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //        ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubrptProcessingEventHandler);
        //        ReportViewer1.LocalReport.Refresh();
        //    }
        //}

        void SubrptProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            ReportDataSource ds = new ReportDataSource("dtReportSolicitacaoPecasPedidoItem", dsPedido.Tables[2]);
            e.DataSources.Add(ds);

            ReportDataSource ds1 = new ReportDataSource("dtReportSolicitacaoPecasMensagem", dsPedido.Tables[1]);
            e.DataSources.Add(ds1);
        }


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
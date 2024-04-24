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
    public partial class RelatorioKAT : ReportBasePage
    {
        //private string idKey => Request.QueryString["IdKey"];
        private DataSet dsConsultaKAT = null;

        protected override void ExibirRelatorio()
        {
            dsConsultaKAT = new ClienteData().ObterRelatorioKAT();

            //Ordenação por data devolução
            //DataView Dw = new DataView(dsPedido.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //dsPedido.Tables.Clear();
            //dsPedido.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = dsConsultaKAT.Tables[0];

            if (dsConsultaKAT.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
                return;
            }
            else
                ReportViewer1.ProcessingMode = ProcessingMode.Local;

            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\KAT\KAT.rdlc";
            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

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
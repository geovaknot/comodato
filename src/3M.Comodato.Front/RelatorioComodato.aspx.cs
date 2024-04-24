using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Extensions.ExtContrato;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;

namespace _3M.Comodato.Front
{
    public partial class RelatorioComodato : System.Web.UI.Page
    {
        private string IdKey => ControlesUtility.Criptografia.Descriptografar(Request.QueryString["IdKey"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.Load += ReportViewer1_Load;
            if (!IsPostBack)
            {
                ContratoData contratoData = new ContratoData();
                var tableContrato = contratoData.ObterLista(new ContratoEntity { ID_CONTRATO = Convert.ToInt64(IdKey) });
                if (tableContrato.Rows.Count > 0)
                {
                    var contratoInfo = tableContrato.Rows[0].ToModel();
                    ConfigurarRelatorioContrato(contratoInfo);
                }

                ReportViewer1.LocalReport.Refresh();
            }
        }

        private void ReportViewer1_Load(object sender, EventArgs e)
        {
            SetVisibleExportOption("PDF,WORDOPENXML");
        }

        /// <summary>
        /// Atribuir visibilidade false para opção de exportar
        /// </summary>
        /// <param name="exportOptions">Excel, EXCELOPENXML, IMAGE, PDF, WORD ou WORDOPENXML</param>
        private void SetVisibleExportOption(string exportOptions)
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

            string[] exportOption = { exportOptions };
            if (exportOptions.Contains(","))
            {
                exportOption = exportOptions.Split(',');
            }

            List<RenderingExtension> extensionsToShow = listExtensions.Where(x => exportOption.Contains(x.Name)).ToList();
            extensionsToShow.ForEach(e=> actionVisible(e, true));
        }

        private void ConfigurarRelatorioContrato(Contrato contrato)
        {
            string nomeDataSet = string.Empty, relatorioArquivo = string.Empty;
            DataTable tableRelatorio = new DataTable();
            List<ReportParameter> parametros = new List<ReportParameter>();

            var contratoDataSet = new Reports.Contrato.dsRelatorioContrato();

            switch (contrato.cdsContratoTipo)
            {
                case "ADITIVO":
                    relatorioArquivo = "Aditivo";
                    nomeDataSet = "dsContratoAditivoNormal";

                    using (var adapter = new Reports.Contrato.dsRelatorioContratoTableAdapters.prcContratoAditivoNormalSelectTableAdapter())
                        adapter.Fill(contratoDataSet.prcContratoAditivoNormalSelect, contrato.nidCliente);
                    tableRelatorio = contratoDataSet.prcContratoAditivoNormalSelect as DataTable;

                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Contrato\Aditivo.rdlc";
                    break;

                //case "DENUNCIA":
                //    relatorioArquivo = "Denuncia";
                //    nomeDataSet = "dsContratoDenuncia";

                //    using (var adapter = new Reports.Contrato.dsRelatorioContratoTableAdapters.prcContratoDenunciaSelectTableAdapter())
                //        adapter.Fill(contratoDataSet.prcContratoDenunciaSelect, contrato.nidCliente);
                //    tableRelatorio = contratoDataSet.prcContratoDenunciaSelect as DataTable;

                //    parametros.Add(new ReportParameter("pCodigoCliente", contrato.nidCliente.ToString()));

                //    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Contrato\Denuncia.rdlc";
                //    break;

                //case "DISTRATO":
                //    relatorioArquivo = "Distrato";
                //    nomeDataSet = "dsContratoDistrato";

                //    using (var adapter = new Reports.Contrato.dsRelatorioContratoTableAdapters.prcContratoDistratoSelectTableAdapter())
                //        adapter.Fill(contratoDataSet.prcContratoDistratoSelect, contrato.nidCliente);
                //    tableRelatorio = contratoDataSet.prcContratoDistratoSelect as DataTable;

                //    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Contrato\Distrato.rdlc";
                //    break;

                default:
                    relatorioArquivo = "Contrato";
                    nomeDataSet = "dsContrato";
                    using (var adapter = new Reports.Contrato.dsRelatorioContratoTableAdapters.prcContratoAditivoNormalSelectTableAdapter())
                        adapter.Fill(contratoDataSet.prcContratoAditivoNormalSelect, contrato.nidCliente);
                    tableRelatorio = contratoDataSet.prcContratoAditivoNormalSelect as DataTable;

                    if (!string.IsNullOrEmpty(contrato.cdsContratoTipo))
                    {
                        parametros.Add(new ReportParameter("pObjeto", contrato.cdsContratoTipo));
                    }

                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Contrato\Contrato.rdlc";
                    break;
            }
            if (!string.IsNullOrEmpty(contrato.cdsClausulas))
                parametros.Add(new ReportParameter("pClausula", contrato.cdsClausulas.Trim()));

            if (tableRelatorio.Rows.Count > 0)
            {
                //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), $@"\Reports\Contrato\{relatorioArquivo}.rdlc");
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(nomeDataSet, tableRelatorio));
                parametros.ForEach(p => ReportViewer1.LocalReport.SetParameters(p));

            }
            else
                pnlMensagem.Visible = true;


        }
    }
}
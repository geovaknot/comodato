using _3M.Comodato.Utility;
using _3M.Comodato.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace _3M.Comodato.Front
{
    public partial class RelatorioPecas : ReportBasePage
    {
        private enum ParametroPecas
        {
            
            ModeloRelatorio = 8
        }

        protected override void ExibirRelatorio()
        {

            var param = this.idKey.Split('|');
            
            string ModeloRelatorio = param[0];
            string StatusRelatorio = param[1];
            if (StatusRelatorio == "0")
            {
                StatusRelatorio = "S";
            }
            else if (StatusRelatorio == "1")
            {
                StatusRelatorio = "N";
            }
            else
            {
                StatusRelatorio = null;
            }
            
            RelatorioPecasData pecasData = new RelatorioPecasData();

            DataSet pecas = pecasData.ObterRelatorio(StatusRelatorio);

            var reportDS = new ReportDataSource();
            reportDS.Name = "dsRptRelatorioPecas";
            reportDS.Value = pecas.Tables[0];

            if (pecas.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Pecas\PecasTab.rdlc";
                if (ModeloRelatorio == "1") // Modelo de Tabela Simplificado (Excel)
                {
                    reportDS.Name = "DataSet1";
                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Pecas\PecasTab.rdlc";

                }
                if (ModeloRelatorio == "2") // Modelo de Tabela Completo
                {
                    reportDS.Name = "DataSet1";
                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Pecas\PecasTab.rdlc";

                }
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();
            }

        }

    }
}
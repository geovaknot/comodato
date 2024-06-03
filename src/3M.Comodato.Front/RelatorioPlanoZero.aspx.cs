using _3M.Comodato.Data;
using Microsoft.Reporting.WebForms;
using System.Data;

namespace _3M.Comodato.Front
{
    public partial class RelatorioPlanoZero : ReportBasePage
    {
        private enum ParametroPecas
        {
            
            ModeloRelatorio = 8
        }

        protected override void ExibirRelatorio()
        {

            var param = this.idKey.Split('|');
            
            string ModeloRelatorio = param[0];
            string PeriodoRelatorio = param[1];

            RelatorioPlanoZeroData data = new RelatorioPlanoZeroData();

            DataSet planoZeroData = data.ObterRelatorio(PeriodoRelatorio);

            var reportDS = new ReportDataSource();
            reportDS.Name = "dsRptRelatorioPlanoZero";
            reportDS.Value = planoZeroData.Tables[0];

            if (planoZeroData.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                
                if (ModeloRelatorio == "1") // Modelo de Tabela Simplificado (Excel)
                {
                   ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PlanoZero\ReportPlanoZero.rdlc";
                }

                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();
            }

        }

    }
}
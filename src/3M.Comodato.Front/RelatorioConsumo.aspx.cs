using _3M.Comodato.Front.Reports.AnaliseConsumo.dsRptConsumoTableAdapters;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.IO;

namespace _3M.Comodato.Front
{
    public partial class RelatorioConsumo : ReportBasePage
    {

        protected override void ExibirRelatorio()
        {
            var param = idKey.Split('|');

            string tipoRelatorio = param[0];

            int codigoVisao = int.Parse(param[1]);
            string visaoRelatorio = param[2];
            
            string codigoGrupo = null;
            if (!string.IsNullOrEmpty(param[3]))
                codigoGrupo = param[3];

            decimal? codigoCliente = null;
            if (!string.IsNullOrEmpty(param[4]))
                codigoCliente = Convert.ToDecimal(param[4]);

            decimal? codigoExecutivo = null;
            if (!string.IsNullOrEmpty(param[5]))
                codigoExecutivo = Convert.ToDecimal(param[5]);

            decimal? codigoVendedor = null;
            if (!string.IsNullOrEmpty(param[6]))
                codigoVendedor = Convert.ToDecimal(param[6]);

            int? codigoLinhaProduto = null;
            if (!string.IsNullOrEmpty(param[7]))
                codigoLinhaProduto = Convert.ToInt32(param[7]);

            DataTable dataTableRelatorio;
            string rdlc = string.Empty;
            string dataSetName = string.Empty;

            if (tipoRelatorio == "Mensal")
            {
                rdlc = "AnaliseConsumoMensal";

                using (var tableAdapter = new prcRptConsumoMensalTableAdapter())
                {
                    using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
                    {
                        tableAdapter.Fill(dsRptConsumo.prcRptConsumoMensal, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo, codigoLinhaProduto);
                        dataTableRelatorio = dsRptConsumo.prcRptConsumoMensal as DataTable;
                    }
                }
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AnaliseConsumo\AnaliseConsumoMensal.rdlc";
            }
            else
            {
                rdlc = "AnaliseConsumoCliente";
                using (var tableAdapter = new prcRptConsumoClienteTableAdapter())
                {
                    using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
                    {
                        tableAdapter.Fill(dsRptConsumo.prcRptConsumoCliente, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo, codigoLinhaProduto);
                        dataTableRelatorio = dsRptConsumo.prcRptConsumoCliente as DataTable;
                    }
                }
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AnaliseConsumo\AnaliseConsumoCliente.rdlc";
            }

            if (dataTableRelatorio.Rows.Count > 0)
            {
                //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), $@"\Reports\AnaliseConsumo\{rdlc}.rdlc");
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ConsumoMensal", dataTableRelatorio));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("VisaoRelatorio", visaoRelatorio));
            }
            else
            {
                pnlMensagem.Visible = true;
            }

        }
        //private string IdKey => ControlesUtility.Criptografia.Descriptografar(Request.QueryString["IdKey"]);

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        ConfigurarRelatorio();
        //        ReportViewer1.LocalReport.Refresh();
        //    }
        //}

        //private void ConfigurarRelatorio()
        //{
        //    var param = this.IdKey.Split('|');

        //    string tipoRelatorio = param[0];

        //    int codigoVisao = int.Parse(param[1]);
        //    string visaoRelatorio = param[2];
        //    //
        //    string codigoGrupo = null;
        //    if (!string.IsNullOrEmpty(param[3]))
        //        codigoGrupo = param[3];

        //    decimal? codigoCliente = null;
        //    if (!string.IsNullOrEmpty(param[4]))
        //        codigoCliente = Convert.ToDecimal(param[4]);

        //    decimal? codigoExecutivo = null;
        //    if (!string.IsNullOrEmpty(param[5]))
        //        codigoExecutivo = Convert.ToDecimal(param[5]);

        //    decimal? codigoVendedor = null;
        //    if (!string.IsNullOrEmpty(param[6]))
        //        codigoVendedor = Convert.ToDecimal(param[6]);

        //    int? codigoLinhaProduto = null;
        //    if (!string.IsNullOrEmpty(param[7]))
        //        codigoLinhaProduto = Convert.ToInt32(param[7]);

        //    DataTable dataTableRelatorio;
        //    string rdlc = string.Empty;
        //    string dataSetName = string.Empty;

        //    if (tipoRelatorio == "Mensal")
        //    {
        //        rdlc = "AnaliseConsumoMensal";
        //        using (var tableAdapter = new prcRptConsumoMensalTableAdapter())
        //        {
        //            using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
        //            {
        //                tableAdapter.Fill(dsRptConsumo.prcRptConsumoMensal, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo, codigoLinhaProduto);
        //                dataTableRelatorio = dsRptConsumo.prcRptConsumoMensal as DataTable;
        //            }
        //        }
        //        ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AnaliseConsumo\AnaliseConsumoMensal.rdlc";
        //    }
        //    else
        //    {
        //        rdlc = "AnaliseConsumoCliente";
        //        using (var tableAdapter = new prcRptConsumoClienteTableAdapter())
        //        {
        //            using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
        //            {
        //                tableAdapter.Fill(dsRptConsumo.prcRptConsumoCliente, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo, codigoLinhaProduto);
        //                dataTableRelatorio = dsRptConsumo.prcRptConsumoCliente as DataTable;
        //            }
        //        }
        //        ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AnaliseConsumo\AnaliseConsumoCliente.rdlc";
        //    }

        //    if (dataTableRelatorio.Rows.Count > 0)
        //    {
        //        //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), $@"\Reports\AnaliseConsumo\{rdlc}.rdlc");
        //        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ConsumoMensal", dataTableRelatorio));
        //        ReportViewer1.LocalReport.SetParameters(new ReportParameter("VisaoRelatorio", visaoRelatorio));
        //    }
        //    else
        //    {
        //        pnlMensagem.Visible = true;
        //    }
        //}
    }
}
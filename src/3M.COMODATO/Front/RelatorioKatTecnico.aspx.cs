using _3M.Comodato.Utility;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace _3M.Comodato.Front
{
    public partial class RelatorioKatTecnico : ReportBasePage
    {

        private enum ParametroKatTecnico
        {
            DataInicial = 0,
            DataFinal = 1,
            CodigoTecnicos = 2,
            CodigoClientes = 3,
            ExibirClientes = 4,
            AnoOuMes = 5
        }

        protected override void ExibirRelatorio()
        {

            string DataInicialParam = "";
            string DataFinalParam = "";

            var param = this.idKey.Split('|');
            DateTime? dataInicial = null;
            if (!string.IsNullOrEmpty(param[ParametroKatTecnico.DataInicial.ToInt()]))
            {
                dataInicial = Convert.ToDateTime(param[ParametroKatTecnico.DataInicial.ToInt()]);
                DataInicialParam = parametros[ParametroKatTecnico.DataInicial.ToInt()];
            }

            DateTime? dataFinal = null;
            if (!string.IsNullOrEmpty(param[ParametroKatTecnico.DataInicial.ToInt()]))
            {
                dataFinal = Convert.ToDateTime(param[ParametroKatTecnico.DataFinal.ToInt()] + " 23:59:59");
                DataFinalParam = parametros[ParametroKatTecnico.DataFinal.ToInt()];
            }

            string codigoTecnicos = null;
            if (!string.IsNullOrEmpty(param[ParametroKatTecnico.CodigoTecnicos.ToInt()]))
                codigoTecnicos = param[ParametroKatTecnico.CodigoTecnicos.ToInt()];

            string codigoClientes = null;
            if (!string.IsNullOrEmpty(param[ParametroKatTecnico.CodigoClientes.ToInt()]))
                codigoClientes = param[ParametroKatTecnico.CodigoClientes.ToInt()];


            string ExibirClientes = null;
            if (!string.IsNullOrEmpty(param[ParametroKatTecnico.ExibirClientes.ToInt()]))
                ExibirClientes = param[ParametroKatTecnico.ExibirClientes.ToInt()];

            string AnoOuMes = null;
            if (!string.IsNullOrEmpty(param[ParametroKatTecnico.AnoOuMes.ToInt()]))
                AnoOuMes = param[ParametroKatTecnico.AnoOuMes.ToInt()];


            DataSet dsRptKatPorTecnico = new KatTecnicoData().ObterRelatorio(dataInicial, dataFinal, codigoTecnicos, codigoClientes);

            var reportDS = new ReportDataSource();
            reportDS.Name = "dsRptKatPorTecnico";
            reportDS.Value = dsRptKatPorTecnico.Tables[0];

            if (dsRptKatPorTecnico.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
                return;
            }
            else
                ReportViewer1.ProcessingMode = ProcessingMode.Local;

            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\KatTecnico\RptKatPorTecnico.rdlc";
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", "Período de " + DataInicialParam + " até " + DataFinalParam));
            ReportViewer1.LocalReport.SetParameters(new ReportParameter("ExibirClientes", ExibirClientes));
            ReportViewer1.LocalReport.SetParameters(new ReportParameter("AnoOuMes", AnoOuMes));
            ReportViewer1.LocalReport.Refresh();

        }
    }
}
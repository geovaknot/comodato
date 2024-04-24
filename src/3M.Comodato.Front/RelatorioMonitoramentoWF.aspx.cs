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
    public partial class RelatorioMonitoramentoWF : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            MonitoramentoWFData monData = new MonitoramentoWFData();

            string[] parameters = parametros;
            string dtInicial = parameters[0];
            string dtFinal = parameters[1];

            if (string.IsNullOrEmpty(dtInicial))
                dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
            if (string.IsNullOrEmpty(dtFinal))
                dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            MonitoramentoWFEntity MonEntity = new MonitoramentoWFEntity();
            MonEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
            MonEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
            MonEntity.TIPO = parameters[2].ToString();

            DataTable monitoramentos = monData.ObterLista(MonEntity);

            //Ordenação por data devolução
            //DataView Dw = new DataView(devolvidos.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //devolvidos.Tables.Clear();
            //devolvidos.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "dsMonitoramento";
            reportDS.Value = monitoramentos;

            if (monitoramentos.Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\MonitoramentoWF\ReportMonitoramentoWF.rdlc";
            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

        }
    }
}
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
    public partial class RelatorioSumarizados : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            SumarizadosData sumarizadosData = new SumarizadosData();

            //string[] parametros = idKey.Split('|');
            string dtInicial = parametros[0];
            string dtFinal = parametros[1];

            if (string.IsNullOrEmpty(dtInicial))
                dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
            if (string.IsNullOrEmpty(dtFinal))
                dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            SumarizadosEntity sumarizadosEntity = new SumarizadosEntity();
            sumarizadosEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
            sumarizadosEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59"); 

            DataSet sumarizados = sumarizadosData.ObterLista(sumarizadosEntity);

            //Ordenação por data devolução
            //DataView Dw = new DataView(devolvidos.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //devolvidos.Tables.Clear();
            //devolvidos.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = sumarizados.Tables[0];

            if (sumarizados.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Sumarizados\ReportSumarizados.rdlc";
            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

        }
        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        SumarizadosData sumarizadosData = new SumarizadosData();

        //        string[] parametros = IdKey.Split('|');
        //        string dtInicial = parametros[0];
        //        string dtFinal = parametros[1];

        //        if (string.IsNullOrEmpty(dtInicial))
        //            dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //        if (string.IsNullOrEmpty(dtFinal))
        //            dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        //        SumarizadosEntity sumarizadosEntity = new SumarizadosEntity();
        //        sumarizadosEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
        //        sumarizadosEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");;

        //        DataSet sumarizados = sumarizadosData.ObterLista(sumarizadosEntity);

        //        //Ordenação por data devolução
        //        //DataView Dw = new DataView(devolvidos.Tables[0]);
        //        //Dw.Sort = "DT_DEVOLUCAO";
        //        //devolvidos.Tables.Clear();
        //        //devolvidos.Tables.Add(Dw.Table);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = sumarizados.Tables[0];

        //        if (sumarizados.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Sumarizados\ReportSumarizados.rdlc";
        //            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}
    }
}
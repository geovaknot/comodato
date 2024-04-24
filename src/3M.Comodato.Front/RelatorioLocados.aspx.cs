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
    public partial class RelatorioLocados : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            LocadosData LocadoData = new LocadosData();

            string[] parameters = parametros;
            string dtInicial = parameters[0];
            string dtFinal = parameters[1];
            if (parameters[2] == "" || parameters[2] == " " || parameters[2] == null)
                parameters[2] = "0";
            Int64 cdCliente = Convert.ToInt32(parameters[2]);
            if (parameters[3] == "" || parameters[3] == " " || parameters[3] == null)
                parameters[3] = "0";
            Int64 cdVendedor = Convert.ToInt32(parameters[3]);
            if (parameters[4] == "" || parameters[4] == " " || parameters[4] == null)
                parameters[4] = "0";
            string cdGrupo = parameters[4];

            if (string.IsNullOrEmpty(dtInicial))
                dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
            if (string.IsNullOrEmpty(dtFinal))
                dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            LocadosEntity locadosEntity = new LocadosEntity();
            locadosEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
            locadosEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
            if (cdCliente > 0)
                locadosEntity.CD_CLIENTE = cdCliente;
            if (cdVendedor > 0)
                locadosEntity.CD_VENDEDOR = cdVendedor;
            if (Convert.ToInt64(cdGrupo) > 0)
                locadosEntity.CD_GRUPO = cdGrupo;

            DataSet Locados = LocadoData.ObterLista(locadosEntity);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = Locados.Tables[0];

            if (Locados.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Locados\ReportLocados.rdlc";
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

        }

        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        LocadosData LocadoData = new LocadosData();

        //        string[] parametros = IdKey.Split('|');
        //        string dtInicial = parametros[0];
        //        string dtFinal = parametros[1];
        //        if (parametros[2] == "" || parametros[2] == null)
        //            parametros[2] = "0";
        //        Int64 cdCliente = Convert.ToInt32(parametros[2]);
        //        if (parametros[3] == "" || parametros[3] == null)
        //            parametros[3] = "0";
        //        string cdVendedor = parametros[3];
        //        string cdGrupo = parametros[4];

        //        if (string.IsNullOrEmpty(dtInicial))
        //            dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //        if (string.IsNullOrEmpty(dtFinal))
        //            dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        //        LocadosEntity locadosEntity = new LocadosEntity();
        //        locadosEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
        //        locadosEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
        //        locadosEntity.CD_CLIENTE = cdCliente;
        //        locadosEntity.CD_VENDEDOR = Convert.ToInt64(cdVendedor);
        //        locadosEntity.CD_GRUPO = cdGrupo;

        //        DataSet Locados = LocadoData.ObterLista(locadosEntity);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = Locados.Tables[0];

        //        if (Locados.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Locados\ReportLocados.rdlc";
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}
    }
}
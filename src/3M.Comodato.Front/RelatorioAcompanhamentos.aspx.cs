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
    public partial class RelatorioAcompanhamentos : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            AcompanhamentosData acompanhamentosData = new AcompanhamentosData();

            string cliente = parametros[0].ToString();
            List<String> listaClientes = new List<string>();
            if (cliente.Length > 0)
            {
                listaClientes = cliente.Split(',').ToList();
            }

            DataSet acompanhamentos = acompanhamentosData.ObterLista(listaClientes);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = acompanhamentos.Tables[0];

            if (acompanhamentos.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Acompanhamentos\ReportAcompanhamentos.rdlc";
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

        }

        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        AcompanhamentosData acompanhamentosData = new AcompanhamentosData();

        //        string[] parametros = IdKey.Split('|');

        //        if (parametros[0] == "" || parametros[0] == null)
        //            parametros[0] = "0";
        //        Int64 cdCliente = Convert.ToInt32(parametros[0]);

        //        DataSet acompanhamentos = acompanhamentosData.ObterLista(cdCliente);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = acompanhamentos.Tables[0];

        //        if (acompanhamentos.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Acompanhamentos\ReportAcompanhamentos.rdlc";
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}


    }
}
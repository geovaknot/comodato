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
    public partial class RelatorioInventarios : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            //SL00033191
            InventariosData inventarioData = new InventariosData();

            //string[] parametros = idKey.Split('|');
            string dtInicial = parametros[0];
            string dtFinal = parametros[1];
            //SL00033191
            string cdClientes = parametros[2];
            List<String> listacdClientes = new List<string>();
            if (cdClientes.Length > 0)
            {
                listacdClientes = cdClientes.Split(',').ToList();
            }


            Int64 CD_VENDEDOR = Convert.ToInt64("0" + parametros[3]);
            string CD_GRUPO = parametros[4];
            string CD_TECNICO = parametros[5];
            int CD_LINHA_PRODUTO = Convert.ToInt32("0" + parametros[6]);
            string ModeloTabela = parametros[7];
            int sitUltManutencao = Convert.ToInt32("0" + parametros[8]);

            //if (string.IsNullOrEmpty(dtInicial))
            //    dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
            //if (string.IsNullOrEmpty(dtFinal))
            //    dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            DateTime? DT_INICIAL = null;
            DateTime? DT_FINAL = null;

            InventariosEntity inventariosEntity = new InventariosEntity();
            if (!string.IsNullOrEmpty(dtInicial))
                DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");

            if (!string.IsNullOrEmpty(dtFinal))
                DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");



            DataSet inventarios = inventarioData.ObterRptLista(DT_INICIAL, DT_FINAL, listacdClientes, CD_VENDEDOR, CD_GRUPO, CD_TECNICO, CD_LINHA_PRODUTO, sitUltManutencao);

            //Ordenação por data devolução
            //DataView Dw = new DataView(devolvidos.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //devolvidos.Tables.Clear();
            //devolvidos.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = inventarios.Tables[0];

            if (inventarios.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                string ReportArq = @"\Reports\Inventarios\ReportInventarios.rdlc";
                if (ModeloTabela == "S")
                {
                    ReportArq = @"\Reports\Inventarios\ReportInventariosTab.rdlc";
                }
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + ReportArq;
                //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();
            }

        }

        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        InventariosData inventarioData = new InventariosData();

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

        //        InventariosEntity inventariosEntity = new InventariosEntity();
        //        inventariosEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
        //        inventariosEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
        //        inventariosEntity.CD_CLIENTE = cdCliente;
        //        inventariosEntity.CD_VENDEDOR = Convert.ToInt64(cdVendedor);
        //        inventariosEntity.CD_GRUPO = cdGrupo;

        //        DataSet inventarios = inventarioData.ObterLista(inventariosEntity);

        //        //Ordenação por data devolução
        //        //DataView Dw = new DataView(devolvidos.Tables[0]);
        //        //Dw.Sort = "DT_DEVOLUCAO";
        //        //devolvidos.Tables.Clear();
        //        //devolvidos.Tables.Add(Dw.Table);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = inventarios.Tables[0];

        //        if (inventarios.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Inventarios\ReportInventarios.rdlc";
        //            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}


    }
}
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
    public partial class RelatorioResumos : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            ResumosData resumoData = new ResumosData();
            ResumosEntity resumosEntity = new ResumosEntity();

            //string[] parametros = idKey.Split('|');
            string filtrosUtilizados = "Filtros: ";

            if (parametros[0] != "" && parametros[0].ToString() != "" && parametros[0] != null && parametros[0] != "0")
            {
                resumosEntity.CD_CLIENTE = Convert.ToInt64(parametros[0]);
                filtrosUtilizados = filtrosUtilizados + " Cliente: " + parametros[0].ToString();
            }

            if (parametros[1] != "" && parametros[1].ToString() != "" && parametros[1] != null && parametros[1] != "0")
            {
                resumosEntity.CD_VENDEDOR = Convert.ToInt64(parametros[1]);
                filtrosUtilizados = filtrosUtilizados + " Vendedor: " + parametros[1].ToString();
            }

            if (parametros[2].ToString() != "" && parametros[2] != null)
            {
                resumosEntity.CD_REGIAO = parametros[2];
                filtrosUtilizados = filtrosUtilizados + " Região: " + parametros[2].ToString();
            }

            if (parametros[3].ToString() != "" && parametros[3] != null)
            {
                resumosEntity.CD_GRUPO = parametros[3];
                filtrosUtilizados = filtrosUtilizados + " Grupo: " + parametros[3].ToString();
            }

            if (parametros[4] != "" && parametros[4].ToString() != "" && parametros[4] != null && parametros[4] != "0")
            {
                resumosEntity.CD_EXECUTIVO = Convert.ToInt64(parametros[4]);
                filtrosUtilizados = filtrosUtilizados + " Executivo: " + parametros[4].ToString();
            }

            if (parametros[5] != "" && parametros[5].ToString() != "" && parametros[5] != null && parametros[5] != "0")
            {
                resumosEntity.CD_LINHA_PRODUTO = Convert.ToInt64(parametros[5]);
                filtrosUtilizados = filtrosUtilizados + " Linha de Produto: " + parametros[5].ToString();
            }

            DataSet resumos = resumoData.ObterLista(resumosEntity);
            //ReportParameter Filtros = new ReportParameter("Filtros", filtrosUtilazados);

            //Ordenação por data devolução
            //DataView Dw = new DataView(devolvidos.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //devolvidos.Tables.Clear();
            //devolvidos.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = resumos.Tables[0];

            if (resumos.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
                //ReportViewer1.LocalReport.SetParameters(Filtros);
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Resumos\ReportResumos.rdlc";
            ReportViewer1.LocalReport.SetParameters(new ReportParameter("Filtros", filtrosUtilizados));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

        }
        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        ResumosData resumoData = new ResumosData();
        //        ResumosEntity resumosEntity = new ResumosEntity();

        //        string[] parametros = IdKey.Split('|');
        //        string filtrosUtilizados = "Filtros: ";

        //        if (parametros[0] != "" && parametros[0].ToString() != "" && parametros[0] != null && parametros[0] != "0")
        //        {
        //            resumosEntity.CD_CLIENTE = Convert.ToInt64(parametros[0]);
        //            filtrosUtilizados = filtrosUtilizados + " Cliente: " + parametros[0].ToString();
        //        }   

        //        if (parametros[1] != "" && parametros[1].ToString() != "" && parametros[1] != null && parametros[1] != "0")
        //        {
        //            resumosEntity.CD_VENDEDOR = Convert.ToInt64(parametros[1]);
        //            filtrosUtilizados = filtrosUtilizados + " Vendedor: " + parametros[1].ToString();
        //        }

        //        if (parametros[2].ToString() != "" && parametros[2] != null)
        //        {
        //            resumosEntity.CD_REGIAO = parametros[2];
        //            filtrosUtilizados = filtrosUtilizados + " Região: " + parametros[2].ToString();
        //        }

        //        if (parametros[3].ToString() != "" && parametros[3] != null)
        //        {
        //            resumosEntity.CD_GRUPO = parametros[3];
        //            filtrosUtilizados = filtrosUtilizados + " Grupo: " + parametros[3].ToString();
        //        }                

        //        if (parametros[4] != "" &&  parametros[4].ToString() != "" && parametros[4] != null && parametros[4] != "0")
        //        {
        //            resumosEntity.CD_EXECUTIVO = Convert.ToInt64(parametros[4]);
        //            filtrosUtilizados = filtrosUtilizados + " Executivo: " + parametros[4].ToString();
        //        }

        //        if (parametros[5] != "" && parametros[5].ToString() != "" && parametros[5] != null && parametros[5] != "0")
        //        {
        //            resumosEntity.CD_LINHA_PRODUTO = Convert.ToInt64(parametros[5]);
        //            filtrosUtilizados = filtrosUtilizados + " Linha de Produto: " + parametros[5].ToString();
        //        }

        //        DataSet resumos = resumoData.ObterLista(resumosEntity);
        //        //ReportParameter Filtros = new ReportParameter("Filtros", filtrosUtilazados);

        //        //Ordenação por data devolução
        //        //DataView Dw = new DataView(devolvidos.Tables[0]);
        //        //Dw.Sort = "DT_DEVOLUCAO";
        //        //devolvidos.Tables.Clear();
        //        //devolvidos.Tables.Add(Dw.Table);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = resumos.Tables[0];

        //        if (resumos.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            //ReportViewer1.LocalReport.SetParameters(Filtros);
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Resumos\ReportResumos.rdlc";
        //            ReportViewer1.LocalReport.SetParameters(new ReportParameter("Filtros", filtrosUtilizados));
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}


    }
}
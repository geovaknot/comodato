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
    public partial class RelatorioDevolvidos : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            DevolvidosData devolvidoData = new DevolvidosData();

            string[] parameters = parametros;
            string dtInicial = parameters[0];
            string dtFinal = parameters[1];
            if (parameters[2] == "" || parameters[2] == null)
                parameters[2] = "0";
            Int64 cdCliente = Convert.ToInt32(parameters[2]);
            if (parameters[3] == "" || parameters[3] == null)
                parameters[3] = "0";
            Int64 cdVendedor = Convert.ToInt32(parameters[3]);
            if (parameters[4] == "" || parameters[4] == null)
                parameters[4] = "0";
            string cdGrupo = parameters[4];
            if (parameters[5] == "" || parameters[5] == null)
                parameters[5] = "0";
            string cdMotivo = parameters[5];

            //BEGIN Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021
            string ModeloTabela = parameters[6];
            //END Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

            if (string.IsNullOrEmpty(dtInicial))
                dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            if (string.IsNullOrEmpty(dtFinal))
                dtFinal = DateTime.Now.ToString("dd/MM/yyyy");

            DevolvidosEntity devolvidosEntity = new DevolvidosEntity();
            devolvidosEntity.DT_DEV_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
            devolvidosEntity.DT_DEV_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
            if (cdCliente > 0)
                devolvidosEntity.CD_CLIENTE = cdCliente;
            if(cdVendedor > 0)
                devolvidosEntity.CD_VENDEDOR = cdVendedor;
            //Código Original
            //if (Convert.ToInt64(cdGrupo) > 0)
            //    devolvidosEntity.CD_GRUPO = cdGrupo;
            //if (Convert.ToInt64(cdMotivo) > 0)
            //    devolvidosEntity.CD_MOTIVO_DEVOLUCAO = cdMotivo;

            //Código alterado chamado SL00033250
            if (cdGrupo != "0")
                devolvidosEntity.CD_GRUPO = cdGrupo;
            if (cdMotivo != "0")
                devolvidosEntity.CD_MOTIVO_DEVOLUCAO = cdMotivo;

            DataSet devolvidos = devolvidoData.ObterLista(devolvidosEntity);

            //Ordenação por data devolução
            //DataView Dw = new DataView(devolvidos.Tables[0]);
            //Dw.Sort = "DT_DEVOLUCAO";
            //devolvidos.Tables.Clear();
            //devolvidos.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = devolvidos.Tables[0];

            //BEGIN Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

            /* Era antes da melhoria
            if (devolvidos.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Devolvidos\ReportDevolvidos.rdlc";
            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();
            */

            if (devolvidos.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                string ReportArq = @"\Reports\Devolvidos\ReportDevolvidos.rdlc";
                if (ModeloTabela == "S")
                {
                    ReportArq = @"\Reports\Devolvidos\ReportDevolvidosTab.rdlc";
                }
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + ReportArq;
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();
            }

            //END Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

        }
        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        DevolvidosData devolvidoData = new DevolvidosData();

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
        //        string cdMotivo = parametros[5];

        //        if (string.IsNullOrEmpty(dtInicial))
        //            dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //        if (string.IsNullOrEmpty(dtFinal))
        //            dtFinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        //        DevolvidosEntity devolvidosEntity = new DevolvidosEntity();
        //        devolvidosEntity.DT_DEV_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
        //        devolvidosEntity.DT_DEV_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
        //        devolvidosEntity.CD_CLIENTE = cdCliente;
        //        devolvidosEntity.CD_VENDEDOR = Convert.ToInt64(cdVendedor);
        //        devolvidosEntity.CD_GRUPO = cdGrupo;
        //        devolvidosEntity.CD_MOTIVO_DEVOLUCAO = cdMotivo;

        //        DataSet devolvidos = devolvidoData.ObterLista(devolvidosEntity);

        //        //Ordenação por data devolução
        //        //DataView Dw = new DataView(devolvidos.Tables[0]);
        //        //Dw.Sort = "DT_DEVOLUCAO";
        //        //devolvidos.Tables.Clear();
        //        //devolvidos.Tables.Add(Dw.Table);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = devolvidos.Tables[0];

        //        if (devolvidos.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Devolvidos\ReportDevolvidos.rdlc";
        //            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}

        private void ReportViewer1_Load(object sender, EventArgs e)
        {
            //SetVisibleExportOption("PDF, Excel");
        }

        /// <summary>
        /// Atribuir visibilidade false para opção de exportar
        /// </summary>
        /// <param name="exportOption">Excel, EXCELOPENXML, IMAGE, PDF, WORD ou WORDOPENXML</param>
        private void SetVisibleExportOption(string exportOption)
        {
            Action<RenderingExtension, bool> actionVisible = new Action<RenderingExtension, bool>((extension, visible) =>
            {
                if (extension != null)
                {
                    System.Reflection.FieldInfo fieldInfo = extension.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    fieldInfo.SetValue(extension, visible);
                }
            });

            var listExtensions = ReportViewer1.LocalReport.ListRenderingExtensions().ToList();
            listExtensions.ForEach(e => actionVisible(e, false));

            RenderingExtension extensionToShow = listExtensions.Find(x => x.Name.Equals(exportOption, StringComparison.CurrentCultureIgnoreCase));
            actionVisible(extensionToShow, true);
        }
    }
}
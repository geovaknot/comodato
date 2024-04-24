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
    public partial class RelatorioLote : ReportBasePage
    {
        //private string idKey => Request.QueryString["IdKey"];
        private DataSet dsLote = null;
        private Int64 ID_PEDIDO = 0;

        protected override void ExibirRelatorio()
        {
            ID_PEDIDO = Convert.ToInt64(parametros[0]); //Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));

            Int64 NR_LOTE;
            if (parametros[1] == "" || parametros[1] == null || parametros[1] == "0")
                NR_LOTE = 0;
            else
                NR_LOTE = Convert.ToInt64(parametros[1]);

            dsLote = new LotesData().ObterLista(ID_PEDIDO, NR_LOTE);

            var reportDS = new ReportDataSource();
            reportDS.Name = "dsLote";
            reportDS.Value = dsLote.Tables[0];

            if (dsLote.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
                return;
            }
            else
            {
                //ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Lotes\Lote.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubrptProcessingEventHandler);
                ReportViewer1.LocalReport.Refresh();
            }            
        }

        private void SubrptProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            MensagemData msgData = new MensagemData();
            MensagemEntity mensagensEntity = new MensagemEntity();
            if (int.Parse(e.Parameters[0].Values[0]) > 0)
            {
                mensagensEntity.pedido.ID_PEDIDO = int.Parse(e.Parameters[0].Values[0].ToString());
            }

            //mensagensEntity.pedido.ID_PEDIDO = ID_PEDIDO;

            DataTable dtSub = msgData.ObterLista(mensagensEntity);
            ReportDataSource ds = new ReportDataSource("DataSet1", dtSub);
            e.DataSources.Add(ds);
        }
    }
}
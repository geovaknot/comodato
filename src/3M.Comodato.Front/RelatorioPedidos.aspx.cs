using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;

namespace _3M.Comodato.Front
{
    public partial class RelatorioPedidos : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            PedidosData pedidoData = new PedidosData();

            string dtInicial = parametros[0];
            string dtFinal = parametros[1];

            Int64 cdCliente;
            Int32 ModeloRelatorio = 0;
            if (!long.TryParse(parametros[2], out cdCliente))
            {
                cdCliente = 0;
            }

            string cdTecnico = parametros[3];
            string cdGrupo = parametros[4];
            //string ModeloRelatorio = null;
            if (!string.IsNullOrEmpty(parametros[5]))
                ModeloRelatorio = Convert.ToInt32(parametros[5]);

            if (string.IsNullOrEmpty(dtInicial))
            {
                dtInicial = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            }

            if (string.IsNullOrEmpty(dtFinal))
            {
                dtFinal = DateTime.Now.ToString("dd/MM/yyyy");
            }

            if (cdGrupo == " ")
                cdGrupo = null;

            if (cdTecnico == " ")
                cdTecnico = null;

            PedidosEntity pedidosEntity = new PedidosEntity();
            pedidosEntity.DT_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
            pedidosEntity.DT_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
            pedidosEntity.cliente.CD_CLIENTE = cdCliente;
            pedidosEntity.tecnico.CD_TECNICO = cdTecnico;
            pedidosEntity.grupo.CD_GRUPO = cdGrupo;

            DataTable pedidos = pedidoData.ObterLista(pedidosEntity);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = pedidos;

            if (pedidos.Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
            }
            else
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Pedidos\ReportPedidos.rdlc";
                if (ModeloRelatorio == 1) // Modelo de Tabela Simplificado (Excel)
                {
                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Pedidos\ReportPedidosSimplificado.rdlc";
                }
                else
                {
                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Pedidos\ReportPedidos.rdlc";
                }

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
                mensagensEntity.pedido.ID_PEDIDO = int.Parse(e.Parameters[0].Values[0]);//int.Parse(e.Parameters["ID_PEDIDO"].Values[0].ToString());
            }

            DataTable dtSub = msgData.ObterLista(mensagensEntity);
            ReportDataSource ds = new ReportDataSource("DataSet1", dtSub);
            e.DataSources.Add(ds);

            //MensagemData msgData = new MensagemData();
            //MensagemEntity mensagensEntity = new MensagemEntity();
            //if (int.Parse(e.Parameters[0].Values[0]) > 0)
            //    mensagensEntity.pedido.ID_PEDIDO = int.Parse(e.Parameters[0].Values[0]);//int.Parse(e.Parameters["ID_PEDIDO"].Values[0].ToString());
            //DataTable dtSub = msgData.ObterLista(mensagensEntity);
            //if (dtSub.Rows.Count > 1)
            //{
            //    ReportDataSource ds = new ReportDataSource("DataSet1", dtSub);
            //    e.DataSources.Add(ds);
            //}
            //else
            //{
            //    ReportDataSource ds = null;
            //    e.DataSources.Add(ds);
            //}
        }
    }
}
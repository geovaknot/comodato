using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Controllers;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _3M.Comodato.Front
{
    public partial class RelatorioWorkflow : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            DateTime? dataInicio = null;
            if (!string.IsNullOrEmpty(parametros[0]))
                dataInicio = Convert.ToDateTime(parametros[0]);

            DateTime? dataFim = null;
            if (!string.IsNullOrEmpty(parametros[1]))
                dataFim = Convert.ToDateTime(parametros[1]).AddDays(1).AddMilliseconds(-1);

            WfPedidoEquipEntity entityFiltro = new WfPedidoEquipEntity();
            if (!string.IsNullOrEmpty(parametros[2]))
                entityFiltro.CD_WF_PEDIDO_EQUIP = parametros[2];

            if (!string.IsNullOrEmpty(parametros[3]))
                entityFiltro.ID_USU_SOLICITANTE= Convert.ToInt64(parametros[3]);

            if (!string.IsNullOrEmpty(parametros[4]))
                entityFiltro.TP_PEDIDO = parametros[4];

            if (!string.IsNullOrEmpty(parametros[5]))
                entityFiltro.ST_STATUS_PEDIDO = Convert.ToInt32(parametros[5]);

            WfPedidoEquipData data = new WfPedidoEquipData();

            var listaPedidos = (from d in data.ObterListaEntity(entityFiltro, dataInicio, dataFim)
                                select ConverterParaPedidoEquipamentoPesquisa(d)).ToList();

            if (listaPedidos.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Workflow\ReportPedidoEquipamento.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsPedidoEquipamento", listaPedidos));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }

        private WfPedidoEquipamentoItem ConverterParaPedidoEquipamentoPesquisa(WfPedidoEquipEntity entity)
        {
            WfPedidoEquipamentoItem model = new WfPedidoEquipamentoItem();

            model.idKey = ControlesUtility.Criptografia.Criptografar(entity.ID_WF_PEDIDO_EQUIP.ToString());
            model.CodigoPedido = entity.CD_WF_PEDIDO_EQUIP;
            model.TituloPedido = entity.DS_TITULO;
            model.DataCadastro = entity.DT_PEDIDO;

            model.TipoPedido = ControlesUtility.Dicionarios.TipoPedidoWorkflow().Where(c => c.Value == entity.TP_PEDIDO).Select(c => c.Key).FirstOrDefault();
            model.ActionTipoPedido = entity.TP_PEDIDO == "E" ? "PedidoEnvio" : "PedidoDevolucao";

            model.CodigoStatusPedido = entity.ST_STATUS_PEDIDO;
            model.Status = entity.DS_STATUS_PEDIDO;
            model.Solicitante = entity.NM_USU_SOLICITANTE;
            if (!string.IsNullOrEmpty(entity.NM_CLIENTE))
                model.Cliente = entity.NM_CLIENTE + "(" + entity.CD_CLIENTE.ToString() + ")" ?? "";//model.Cliente = entity.NM_CLIENTE ?? "";
            else
                model.Cliente = "";
            model.Modelo = entity.DS_MODELO ?? "";


            return model;
        }
    }
}
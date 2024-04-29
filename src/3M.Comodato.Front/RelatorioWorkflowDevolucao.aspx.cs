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
    public partial class RelatorioWorkflowDevolucao : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            DateTime? dataInicio = null;
            if (!string.IsNullOrEmpty(parametros[0]))
                dataInicio = Convert.ToDateTime(parametros[0]);

            DateTime? dataFim = null;
            if (!string.IsNullOrEmpty(parametros[1]))
                dataFim = Convert.ToDateTime(parametros[1]).AddDays(1).AddMilliseconds(-1);

            WfPedidoEquipDevolucaoEntity entityFiltro = new WfPedidoEquipDevolucaoEntity();
            if (!string.IsNullOrEmpty(parametros[2]))
                entityFiltro.CD_WF_PEDIDO_EQUIP = parametros[2];

            if (!string.IsNullOrEmpty(parametros[3]))
                entityFiltro.ID_USU_SOLICITANTE= Convert.ToInt64(parametros[3]);

            if (!string.IsNullOrEmpty(parametros[4]))
                entityFiltro.TP_PEDIDO = parametros[4];

            if (!string.IsNullOrEmpty(parametros[5]))
                entityFiltro.ST_STATUS_PEDIDO = Convert.ToInt32(parametros[5]);

            WfPedidoEquipData data = new WfPedidoEquipData();

            var listaPedidos = (from d in data.ObterListaDevolucaoEntity(entityFiltro, dataInicio, dataFim)
                                select ConverterParaPedidoEquipamentoPesquisaDevolucao(d)).ToList();

            if (listaPedidos.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\WorkflowDevolucao\ReportPedidoEquipamentoDevolucao.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsPedidoEquipamentoDevolucao", listaPedidos));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }

        private WfPedidoEquipamentoItemDevolucao ConverterParaPedidoEquipamentoPesquisaDevolucao(WfPedidoEquipDevolucaoEntity entity)
        {
            WfPedidoEquipamentoItemDevolucao model = new WfPedidoEquipamentoItemDevolucao();

            model.idKey = ControlesUtility.Criptografia.Criptografar(entity.ID_WF_PEDIDO_EQUIP.ToString());
            model.CD_WF_PEDIDO_EQUIP = entity.CD_WF_PEDIDO_EQUIP;
            model.DT_PEDIDO = entity.DT_PEDIDO;
            model.ST_STATUS_PEDIDO = entity.ST_STATUS_PEDIDO;
            model.TP_PEDIDO = ControlesUtility.Dicionarios.TipoPedidoWorkflow().Where(c => c.Value == entity.TP_PEDIDO).Select(c => c.Key).FirstOrDefault(); ;


            model.UsuarioSolicitante = entity.NM_USU_SOLICITANTE;
            if (!string.IsNullOrEmpty(entity.NM_CLIENTE))
                model.Cliente = entity.NM_CLIENTE + "(" + entity.CD_CLIENTE.ToString() + ")" ?? "";//model.Cliente = entity.NM_CLIENTE ?? "";
            else
                model.Cliente = "";
            model.Modelo = entity.DS_MODELO ?? "";

            model.CD_TROCA = entity.CD_TROCA?? "N";
            model.CD_ATIVO_FIXO_TROCA = entity.CD_ATIVO_FIXO_TROCA;
            model.DS_CONTATO_NOME = entity.DS_CONTATO_NOME;
            model.DS_CONTATO_EMAIL = entity.DS_CONTATO_EMAIL;
            model.DS_CONTATO_TEL_NUM = entity.DS_CONTATO_TEL_NUM;
            model.CD_LINHA = entity.CD_LINHA;
            model.CD_UNIDADE_MEDIDA = entity.CD_UNIDADE_MEDIDA;
            model.QT_EQUIPAMENTO = entity.QT_EQUIPAMENTO;
            model.VL_ALTURA_MAX = entity.VL_ALTURA_MAX;
            model.VL_LARGURA_MAX = entity.VL_LARGURA_MAX;
            model.VL_COMPRIM_MAX = entity.VL_COMPRIM_MAX;
            model.VL_PESO_MAXIMO = entity.VL_PESO_MAXIMO;
            model.DS_TITULO = entity.DS_TITULO;
            model.CD_MOTIVO_DEVOLUCAO = entity.CD_MOTIVO_DEVOLUCAO;
            model.VL_NOTA_FISCAL_3M = entity.VL_NOTA_FISCAL_3M;
            model.Transportadora = entity.NM_EMPRESA;
            model.DT_RETIRADA_AGENDADA = entity.DT_RETIRADA_AGENDADA;
            model.DT_RETIRADA_REALIZADA = entity.DT_RETIRADA_REALIZADA;
            model.DT_PROGRAMADA_TMS = entity.DT_PROGRAMADA_TMS;
            model.DT_DEVOLUCAO_3M = entity.DT_DEVOLUCAO_3M;
            model.DT_DEVOLUCAO_PLANEJAMENTO = entity.DT_DEVOLUCAO_PLANEJAMENTO;
            model.DS_OBSERVACAO = entity.DS_OBSERVACAO;

            //model.ActionTipoPedido = entity.TP_PEDIDO == "E" ? "PedidoEnvio" : "PedidoDevolucao";

            //model.CodigoStatusPedido = entity.ST_STATUS_PEDIDO;
            //model.Status = entity.DS_STATUS_PEDIDO;


            return model;
        }
    }
}
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
    public partial class RelatorioReclamacao : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            DateTime? dataInicio = null;
            if (!string.IsNullOrEmpty(parametros[0]))
                dataInicio = Convert.ToDateTime(parametros[0]);

            DateTime? dataFim = null;
            if (!string.IsNullOrEmpty(parametros[1]))
                dataFim = Convert.ToDateTime(parametros[1]).AddDays(1).AddMilliseconds(-1);

            RelatorioReclamacaoEntity entityFiltro = new RelatorioReclamacaoEntity();
            if (!string.IsNullOrEmpty(parametros[2]))
                entityFiltro.clienteEntity.CD_CLIENTE = Convert.ToInt64(parametros[2]);

            if (!string.IsNullOrEmpty(parametros[3]))
                entityFiltro.tecnicoEntity.CD_TECNICO = parametros[3].ToString();
      

            if (!string.IsNullOrEmpty(parametros[4]))
                entityFiltro.TipoAtendimento = Convert.ToInt32(parametros[4]);

            if (!string.IsNullOrEmpty(parametros[5]))
                entityFiltro.TipoReclamacaoRR = Convert.ToInt32(parametros[5]);


            if (!string.IsNullOrEmpty(parametros[6]))
                entityFiltro.rrStatusEntity.ST_STATUS_RR = Convert.ToInt32(parametros[6]);


            if (!string.IsNullOrEmpty(parametros[7]))
                entityFiltro.pecaEntity.CD_PECA = parametros[7].ToString();

            if (!string.IsNullOrEmpty(parametros[8]))
                entityFiltro.ativoFixoEntity.CD_ATIVO_FIXO = parametros[8].ToString();
          
            RelatorioReclamacaoData data = new RelatorioReclamacaoData();

            var listaReclamacao = (from d in data.ObterListaEntity(entityFiltro, dataInicio, dataFim)
                                select ConverterParaPedidoEquipamentoPesquisa(d)).ToList();

            if (listaReclamacao.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\RelatorioReclamacao\ReportRelatorioReclamacao.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsRelatorioReclamacao", listaReclamacao));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }

        private RelatorioReclamacaoItem ConverterParaPedidoEquipamentoPesquisa(RelatorioReclamacaoEntity entity)
        {
            RelatorioReclamacaoItem model = new RelatorioReclamacaoItem();
            model.ID_RELATORIO_RECLAMACAO = entity.ID_RELATORIO_RECLAMACAO;
            model.idKey = ControlesUtility.Criptografia.Criptografar(entity.ID_RELATORIO_RECLAMACAO.ToString());
            model.DataCadastro = entity.DataCadastro.ToShortDateString();

            model.TipoAtendimento = ControlesUtility.Dicionarios.TipoAtendimento().Where(c => c.Key == entity.TipoAtendimento).Select(c => c.Value).FirstOrDefault();

            model.TipoReclamacaoRR = ControlesUtility.Dicionarios.TipoReclamacaoRR().Where(c => c.Key == entity.TipoReclamacaoRR).Select(c => c.Value).FirstOrDefault();

            model.DS_MOTIVO = entity.DS_MOTIVO;
            model.TecSolicitante = entity.TecnicoSolicitante;
            // model.TecRegional = entity.TecnicoRegional;
            model.Cliente = entity.clienteEntity.NM_CLIENTE;
            model.Status = entity.rrStatusEntity.DS_STATUS_NOME_REDUZ;
            model.ST_STATUS_RR = entity.ST_STATUS_RR;
            model.Ativo = entity.ativoFixoEntity.CD_ATIVO_FIXO;
            model.Peca = entity.pecaEntity.DS_PECA;
            model.TEMPO_ATENDIMENTO = entity.TEMPO_ATENDIMENTO;
            model.CD_GRUPO_RESPONS = entity.CD_GRUPO_RESPONS;
            model.ID_OS = entity.osPadraoEntity.ID_OS;
            //  model.ID_USUARIO_RESPONS = entity.ID_USUARIO_RESPONS;
            model.NM_Fornecedor = entity.NM_Fornecedor;

            model.VL_Hora_Atendimento = TimeSpan.FromHours(Convert.ToInt32((model.TEMPO_ATENDIMENTO / 60).ToString())).ToString();
            model.VL_Minuto_Atendimento = CalcularMinuto(model.TEMPO_ATENDIMENTO, model.VL_Hora_Atendimento);

            model.VL_Hora_Atendimento = model.VL_Hora_Atendimento.Substring(0, 2) + ":" + model.VL_Minuto_Atendimento;

            return model;
        }



        private string CalcularMinuto(int Vl_Tempo_Atendimento, string Vl_Hora)
        {

            decimal hora;
            string minuto = Vl_Tempo_Atendimento.ToString();
            if (Convert.ToInt32(Vl_Hora.Substring(0, 2)) > 0)
            {
                hora = Math.Round(Convert.ToDecimal((Vl_Tempo_Atendimento / (double)60)), 2);


                if (hora.ToString().Length > 1)
                {
                    Int64 horaInteira = Convert.ToInt64(Math.Floor(Convert.ToDecimal((Vl_Tempo_Atendimento / (double)60))));
                    //string minuto = Math.Floor(Convert.ToDouble(hora)) ;
                    string minutoconvertido = Math.Round((hora - horaInteira) * 60).ToString();
                    minuto = minutoconvertido.PadRight(2, '0');
                    ////minuto = hora.ToString().Substring(2, 2);
                }
                else
                    minuto = "0";
            }
            return minuto.ToString();
        }
    }
}
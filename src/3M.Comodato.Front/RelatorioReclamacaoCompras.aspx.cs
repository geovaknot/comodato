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
    public partial class RelatorioReclamacaoCompras : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
         

            RelatorioReclamacaoEntity entityFiltro = new RelatorioReclamacaoEntity();
            if (!string.IsNullOrEmpty(parametros[0]))
                entityFiltro.rrStatusEntity.ST_STATUS_RR = Convert.ToInt32(parametros[0]);

            if (!string.IsNullOrEmpty(parametros[1]))
                entityFiltro.ID_RELATORIO_RECLAMACAO = Convert.ToInt32(parametros[1]);
            RelatorioReclamacaoData data = new RelatorioReclamacaoData();

            var listaReclamacao = (from d in data.ObterListaEntity(entityFiltro)
                                select ConverterParaPedidoEquipamentoPesquisa(d)).ToList();

            if (listaReclamacao.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\RelatorioReclamacao\ReportRelatorioReclamacaoCompras.rdlc";
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
            model.DS_ARQUIVO_FOTO = entity.DS_ARQUIVO_FOTO;
            model.Custo_Peca = entity.Custo_Peca;
            model.DS_MOTIVO = entity.DS_MOTIVO;
            model.DS_Descricao = entity.DS_DESCRICAO;
            model.NM_Fornecedor = entity.NM_Fornecedor;
            model.Vl_Mao_Obra = entity.ValorMaoDeObra;
            if (!string.IsNullOrEmpty(model.Vl_Mao_Obra.ToString()))
            {
                //O valor mão de Obra mais o custo da Peça
                model.Custo_Total = model.Vl_Mao_Obra + model.Custo_Peca;
            }

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
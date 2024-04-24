using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class SolicitacaoAtendimentoController : BaseController
    {
        private void ValidarPermissoes()
        {
            ViewBag.VisaoAdmin = CurrentUser.perfil.nidPerfil.Equals(ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt());
            ViewBag.VisaoTecnico = CurrentUser.perfil.nidPerfil.Equals(ControlesUtility.Enumeradores.Perfil.Tecnico3M.ToInt()) ||
                CurrentUser.perfil.nidPerfil.Equals(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira.ToInt());
            ViewBag.VisaoCliente = CurrentUser.perfil.nidPerfil.Equals(ControlesUtility.Enumeradores.Perfil.Cliente.ToInt());
        }

        [_3MAuthentication]
        public ActionResult Index()
        {
            ValidarPermissoes();
            PopularTipoAtendimento();
            PopularStatusAtendimento();
            return View();
        }

        private void PopularTipoAtendimento()
        {
            TipoAtendimentoData data = new TipoAtendimentoData();
            var listaTipoAtendimento = data.ObterLista();
            ViewBag.ListaTipoAtendimento = new SelectList(listaTipoAtendimento.ToList(), "ID_TIPO_ATENDIMENTO", "DS_TIPO_ATENDIMENTO");
        }
        private void PopularStatusAtendimento()
        {
            StatusAtendimentoData data = new StatusAtendimentoData();
            var listaStatusAtendimento = data.ObterLista();
            ViewBag.ListaStatusAtendimento = new SelectList(listaStatusAtendimento.ToList(), "ID_STATUS_ATENDIMENTO", "DS_STATUS_ATENDIMENTO");
        }

        [_3MAuthentication]
        public JsonResult ObterSolicitacoes(SolicitacaoAtendimentoEntity filtro)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            IEnumerable<SoliticacaoAtendimento> listaSolicitacao = null;
            try
            {
                Func<SolicitacaoAtendimentoEntity, SoliticacaoAtendimento> converter = new Func<SolicitacaoAtendimentoEntity, SoliticacaoAtendimento>((e) =>
                {
                    SoliticacaoAtendimento model = new SoliticacaoAtendimento();
                    model.CodigoSolicitacao = e.ID_SOLICITA_ATENDIMENTO;
                    model.DataSolicitacao = e.DT_DATA_SOLICITACAO;
                    model.StatusSolicitacao = e.StatusAtendimento.DS_STATUS_ATENDIMENTO;
                    model.Observacao = e.DS_OBSERVACAO;
                    model.TipoAtendimento = e.TipoAtendimento.DS_TIPO_ATENDIMENTO;

                    model.DataVisita = null;
                    if (e.OS.visita.DT_DATA_VISITA != DateTime.MinValue)
                    {
                        model.DataVisita = e.OS.visita.DT_DATA_VISITA;
                    }

                    model.NomeCliente = e.CLIENTE.NM_CLIENTE;
                    model.QuantidadeEquipamento = e.QT_EQUIPAMENTO;
                    model.CD_ATIVO_FIXO = e.AtivoFixo.CD_ATIVO_FIXO;
                    model.ID_OS = e.OS.ID_OS;

                    return model;
                });

                if (null == filtro)
                {
                    filtro = new SolicitacaoAtendimentoEntity();
                }

                ValidarPermissoes();

                SolicitacaoAtendimentoData data = new SolicitacaoAtendimentoData();
                DateTime PeriodoInicioFiltro = DateTime.Now.AddDays(-30);
                var listaEntity = data.ObterListaEntity(filtro, PeriodoInicioFiltro);

                listaSolicitacao = (from s in listaEntity
                                    select converter(s)).ToList();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoAtendimento/_gridMVCListaSolicitacao.cshtml", listaSolicitacao));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;

        }
    }
}
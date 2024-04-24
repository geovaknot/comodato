using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Extensions.ExPesquisaSatisfacao;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class AnaliseSatisfacaoController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            try
            {
                PopularComboAcao();
                SatisfacaoPesquisaData pesquisaData = new SatisfacaoPesquisaData();
                List<SatisfacaoPesquisa> listaPesquisa = new List<SatisfacaoPesquisa>();

                DataTable dtPesquisaSatisfacao = pesquisaData.ObterLista(new SatisfacaoPesquisaEntity());
                listaPesquisa = (from row in dtPesquisaSatisfacao.Rows.Cast<DataRow>()
                                 select row.ToPesquisaModel(ActionListarRespostas)).ToList();

                return View(listaPesquisa);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        private void PopularComboAcao()
        {
            List<SelectListItem> listaAcao = new List<SelectListItem>();
            listaAcao.Add(new SelectListItem() { Text = "Enviar pesquisa de satisfação", Value = "1" });
            listaAcao.Add(new SelectListItem() { Text = "Somente enviar pesquisa específica", Value = "2" });
            listaAcao.Add(new SelectListItem() { Text = "Nunca enviar pesquisa de satisfação", Value = "3" });
            ViewBag.ListaAcao = listaAcao;
        }

        private void PopularCombos()
        {
            ViewBag.ListaUsuarios = new SelectList(base.ObterListaUsuario(), "nidUsuario", "cnmNome") as IEnumerable<SelectListItem>;
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            PopularCombos();
            return View(new SatisfacaoPesquisa());
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(SatisfacaoPesquisa model)
        {
            try
            {
                PopularCombos();

                if (ModelState.IsValid)
                {
                    SatisfacaoPesquisaEntity entity = model.ToPesquisaEntity();
                    entity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();
                    if (data.Inserir(ref entity))
                    {
                        model.JavaScriptToRun = "MensagemSucesso()";
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(model);
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            PopularCombos();
            SatisfacaoPesquisa model = null;
            try
            {
                SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();

                SatisfacaoPesquisaEntity entity = new SatisfacaoPesquisaEntity();
                entity.ID_PESQUISA_SATISF = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTable dt = data.ObterLista(entity);
                model = (from r in dt.Rows.Cast<DataRow>()
                         select r.ToPesquisaModel()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(SatisfacaoPesquisa model)
        {
            try
            {
                PopularCombos();
                if (ModelState.IsValid)
                {
                    SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();
                    SatisfacaoPesquisaEntity entity = model.ToPesquisaEntity();
                    entity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    if (data.Alterar(entity))
                    {
                        model.JavaScriptToRun = "MensagemSucesso()";
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(model);
        }

        [_3MAuthentication]
        public ActionResult ConsultarResultadoPesquisa(string idKey)
        {
            long codigoPesquisa = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));

            SatisfacaoPesquisa model = ObterSatisfacaoPesquisa(codigoPesquisa);
            return View(model);
        }

        internal SatisfacaoPesquisa ObterSatisfacaoPesquisa(long codigoPesquisa)
        {
            SatisfacaoPesquisa model = new SatisfacaoPesquisa();
            try
            {
                SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();

                SatisfacaoPesquisaEntity entity = new SatisfacaoPesquisaEntity();
                entity.ID_PESQUISA_SATISF = codigoPesquisa;
                DataTable dt = data.ObterLista(entity);
                model = (from r in dt.Rows.Cast<DataRow>()
                         select r.ToPesquisaModel(ActionListarRespostas)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return model;
        }

        [_3MAuthentication]
        public PartialViewResult PesquisaSatisfacao(long idPesquisaSatisfacao, long idVisita)
        {
            SatisfacaoResposta model = new SatisfacaoResposta() { DataResposta = DateTime.Now.DataString() };
            try
            {
                SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();
                SatisfacaoPesquisaEntity entity = new SatisfacaoPesquisaEntity();
                entity.ID_PESQUISA_SATISF = idPesquisaSatisfacao;

                model.Pesquisa = (from p in data.ObterLista(entity).Rows.Cast<DataRow>()
                                  select p.ToPesquisaModel()).FirstOrDefault();



                VisitaData visitaData = new VisitaData();
                model.Visita = (from v in visitaData.ObterLista(new VisitaEntity { ID_VISITA = idVisita }).Rows.Cast<DataRow>()
                                select new Visita
                                {
                                    ID_VISITA = Convert.ToInt64(v["ID_VISITA"]),
                                    DT_DATA_VISITA = v["DT_DATA_VISITA"].DataString(),
                                    tecnico = new TecnicoEntity() { NM_TECNICO = v["NM_TECNICO"].ToString() },
                                    DS_NOME_RESPONSAVEL = v["DS_NOME_RESPONSAVEL"] is DBNull ? String.Empty : v["DS_NOME_RESPONSAVEL"].ToString()
                                }).FirstOrDefault();

                if (null == model.Pesquisa)
                {
                    model.Pesquisa = new SatisfacaoPesquisa();
                }
                if (null == model.Visita)
                {
                    model.Visita = new Visita();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return PartialView("_modalPesquisaSatisfacao", model);
        }

        [_3MAuthentication]
        public ActionResult Imprimir(string idKey)
        {
            string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
            return Redirect(URLSite + $"/RelatorioPesquisaSatisfacao.aspx?IdKey={idKey}");
        }

        private Func<long, List<SatisfacaoResposta>> ActionListarRespostas = new Func<long, List<SatisfacaoResposta>>((c) =>
        {
            List<SatisfacaoResposta> listaRespostas = null;
            try
            {
                SatisfacaoRespostaEntity entityFilter = new SatisfacaoRespostaEntity();
                entityFilter.SatisfacaoPesquisa.ID_PESQUISA_SATISF = c;

                SatisfacaoRespostaData data = new SatisfacaoRespostaData();
                DataTable dtRespostas = data.ObterLista(entityFilter);
                listaRespostas = (from r in dtRespostas.Rows.Cast<DataRow>()
                                  select r.ToRespostaModel()).ToList();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return listaRespostas;
        });
    }
}

namespace _3M.Comodato.Front.Extensions.ExPesquisaSatisfacao
{
    internal static class PesquisaExtensions
    {
        public static SatisfacaoPesquisa ToPesquisaModel(this DataRow r)
        {
            return ToPesquisaModel(r, null);
        }

        public static SatisfacaoPesquisa ToPesquisaModel(this DataRow r, Func<long, List<SatisfacaoResposta>> actionRespostas)
        {
            SatisfacaoPesquisa satisfacaoPesquisa = new SatisfacaoPesquisa();

            long codigoPesquisa = Convert.ToInt64(r["ID_PESQUISA_SATISF"]);
            satisfacaoPesquisa.IdPesquisa = codigoPesquisa;
            satisfacaoPesquisa.idKey = ControlesUtility.Criptografia.Criptografar(codigoPesquisa.ToString());
            satisfacaoPesquisa.Titulo = r["DS_TITULO"].ToString();
            satisfacaoPesquisa.DataCriacao = Convert.ToDateTime(r["DT_CRIACAO"]).ToString();
            satisfacaoPesquisa.CodigoTipoPesquisa = Convert.ToInt32(r["TP_PESQUISA"]);

            if (r["DT_FINALIZACAO"] != DBNull.Value)
            {
                satisfacaoPesquisa.DataFinalizacao = Convert.ToDateTime(r["DT_FINALIZACAO"]).ToString();
            }

            satisfacaoPesquisa.StatusPesquisa = Convert.ToInt32(r["ST_STATUS_PESQUISA"]);
            satisfacaoPesquisa.cdsAtivo = satisfacaoPesquisa.StatusPesquisa == 1 ? "Ativo" : "Finalizado";

            if (null != actionRespostas)
            {
                satisfacaoPesquisa.ListaRespostas = actionRespostas.Invoke(codigoPesquisa);
            }

            satisfacaoPesquisa.nidUsuarioResponsavel = Convert.ToInt64(r["ID_USUARIO_RESPONSAVEL"]);
            if (r.Table.Columns.Contains("NM_USUARIO_RESPONSAVEL"))
            {
                satisfacaoPesquisa.UsuarioResponsavel = r["NM_USUARIO_RESPONSAVEL"].ToString();
            }

            satisfacaoPesquisa.DescricaoPesquisa = r["DS_DESCRICAO"].ToString();
            satisfacaoPesquisa.PerguntaPesquisa1 = r["DS_PERGUNTA1"].ToString();
            satisfacaoPesquisa.PerguntaPesquisa2 = r["DS_PERGUNTA2"].ToString();
            satisfacaoPesquisa.PerguntaPesquisa3 = r["DS_PERGUNTA3"].ToString();
            satisfacaoPesquisa.PerguntaPesquisa4 = r["DS_PERGUNTA4"].ToString();
            satisfacaoPesquisa.PerguntaPesquisa5 = r["DS_PERGUNTA5"].ToString();
            satisfacaoPesquisa.QuantidadeVisitas = Convert.ToInt32(r["QT_VISITAS"]);

            return satisfacaoPesquisa;
        }

        public static SatisfacaoPesquisaEntity ToPesquisaEntity(this SatisfacaoPesquisa satisfacaoPesquisa)
        {
            SatisfacaoPesquisaEntity satisfacaoPesquisaEntity = new SatisfacaoPesquisaEntity();

            if (!string.IsNullOrEmpty(satisfacaoPesquisa.idKey))
            {
                satisfacaoPesquisaEntity.ID_PESQUISA_SATISF = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(satisfacaoPesquisa.idKey));
            }

            satisfacaoPesquisaEntity.TP_PESQUISA = satisfacaoPesquisa.CodigoTipoPesquisa;
            satisfacaoPesquisaEntity.ST_STATUS_PESQUISA = satisfacaoPesquisa.StatusPesquisa;
            satisfacaoPesquisaEntity.DS_TITULO = satisfacaoPesquisa.Titulo;
            //satisfacaoPesquisaEntity.DT_CRIACAO = satisfacaoPesquisa.DataCriacao;

            satisfacaoPesquisaEntity.USUARIO_RESPONSAVEL.nidUsuario = satisfacaoPesquisa.nidUsuarioResponsavel;
            satisfacaoPesquisaEntity.DS_DESCRICAO = satisfacaoPesquisa.DescricaoPesquisa;
            satisfacaoPesquisaEntity.DS_PERGUNTA1 = satisfacaoPesquisa.PerguntaPesquisa1;
            satisfacaoPesquisaEntity.DS_PERGUNTA2 = satisfacaoPesquisa.PerguntaPesquisa2;
            satisfacaoPesquisaEntity.DS_PERGUNTA3 = satisfacaoPesquisa.PerguntaPesquisa3;
            satisfacaoPesquisaEntity.DS_PERGUNTA4 = satisfacaoPesquisa.PerguntaPesquisa4;
            satisfacaoPesquisaEntity.DS_PERGUNTA5 = satisfacaoPesquisa.PerguntaPesquisa5;

            return satisfacaoPesquisaEntity;
        }

        public static SatisfacaoResposta ToRespostaModel(this DataRow r)
        {
            SatisfacaoResposta model = new SatisfacaoResposta();
            model.idKey = ControlesUtility.Criptografia.Criptografar(r["ID_RESPOSTA_SATISF"].ToString());
            model.Pesquisa.idKey = ControlesUtility.Criptografia.Criptografar(r["ID_PESQUISA_SATISF"].ToString());
            model.Visita.ID_VISITA = Convert.ToInt64(r["ID_VISITA"]);

            DateTime? dataResposta = r["DT_DATA_RESPOSTA"].Data();
            if (dataResposta.HasValue)
            {
                model.DataResposta = r["DT_DATA_RESPOSTA"].Data().Value.DataString();
            }

            model.NomeRespondedor = r["DS_NOME_RESPONDEDOR"].ToString();
            model.NotaPesquisa = Convert.ToDecimal(r["NM_NOTA_PESQ"]);
            model.Justificativa = r["DS_JUSTIFICATIVA"].ToString();
            model.RespostaPesquisa1 = r["DS_RESPOSTA1"].ToString();
            model.RespostaPesquisa2 = r["DS_RESPOSTA2"].ToString();
            model.RespostaPesquisa3 = r["DS_RESPOSTA3"].ToString();
            model.RespostaPesquisa4 = r["DS_RESPOSTA4"].ToString();
            model.RespostaPesquisa5 = r["DS_RESPOSTA5"].ToString();

            if (null == model.Visita.cliente)
            {
                model.Visita.cliente = new ClienteEntity();
            }

            if (r.Table.Columns.Contains("NM_CLIENTE"))
            {
                model.Visita.cliente.NM_CLIENTE = r["NM_CLIENTE"].ToString();
            }

            return model;
        }
    }
}
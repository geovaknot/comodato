using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class MovimPecasController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<SelectListItem> listaEstoque = ObterListaEstoque().ToList();
            listaEstoque.Insert(0, new SelectListItem() { Value = "", Text = "Selecione...", Selected = true });

            List<SelectListItem> listaAcao = ObterListaTipoMovimentacao().ToList();
            listaAcao.Insert(0, new SelectListItem() { Value = "", Text = "Selecione...", Selected = true });

            ViewBag.ListaEstoque = listaEstoque;
            ViewBag.ListaAcao = listaAcao;


            return View();
        }

        private IEnumerable<SelectListItem> ObterListaEstoque()
        {
            EstoqueEntity estoqueFiltro = new EstoqueEntity();
            estoqueFiltro.ID_USU_RESPONSAVEL = CurrentUser.usuario.nidUsuario;
            estoqueFiltro.FL_ATIVO = "S";

            EstoqueData data = new EstoqueData();
            var lista = data.ObterListaUsuario(estoqueFiltro);

            var listaEstoque = (from dr in lista.Rows.Cast<DataRow>()
                                select new SelectListItem()
                                {
                                    Text = dr["CD_ESTOQUE"].ToString() + " " +dr["DS_ESTOQUE"].ToString(),
                                    Value = dr["ID_ESTOQUE"].ToString()
                                }).ToList();
            return listaEstoque;
        }

        private IEnumerable<SelectListItem> ObterListaTipoMovimentacao()
        {
            List<SelectListItem> listaTipoMovimentacao = null;
            EstoqueMoviData estoqueMoviData = new EstoqueMoviData();
            using (DataTable dtTpMovto = estoqueMoviData.ObterListaTipoMovimento())
            {
                List<int> listaTiposValidos = new List<int>();
                if (CurrentUser.perfil.ccdPerfil == ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt()
                    || CurrentUser.perfil.ccdPerfil == ControlesUtility.Enumeradores.Perfil.ControleEstoque.ToInt())
                {
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.MovimentacaoEstoque.ToInt());
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.MovimentacaoPecas.ToInt());
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.AjusteEntrada.ToInt());
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.AjusteSaida.ToInt());
                }
                else if (CurrentUser.perfil.ccdPerfil == ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M.ToInt())
                {
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.MovimentacaoEstoque.ToInt());
                }
                else if (CurrentUser.perfil.ccdPerfil == ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica.ToInt())
                {
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.MovimentacaoEstoque.ToInt());
                    listaTiposValidos.Add(ControlesUtility.Enumeradores.TpMovimentacao.MovimentacaoPecas.ToInt());
                }

                listaTipoMovimentacao = (from r in dtTpMovto.Rows.Cast<DataRow>()
                                         where listaTiposValidos.Contains(Convert.ToInt32(r["cd_tp_movimentacao"].ToString()))
                                         orderby r["CD_TP_MOVIMENTACAO"]
                                         select new SelectListItem()
                                         {
                                             Value = r["CD_TP_MOVIMENTACAO"].ToString(),
                                             Text = String.Concat(r["CD_TP_MOVIMENTACAO"].ToString(), "-", r["DS_TP_MOVIMENTACAO"].ToString())
                                         }).ToList();
            }

            return listaTipoMovimentacao;
        }


        /// <summary>
        /// Tela "Consulta de Movimentação de Estoque"
        /// </summary>
        /// <returns></returns>
        [_3MAuthentication]
        public ActionResult ConsultarMovimentacao()
        {
            PesquisaMovimentacaoModel model = new PesquisaMovimentacaoModel();

            EstoqueEntity estoqueFiltro = new EstoqueEntity();
            estoqueFiltro.ID_USU_RESPONSAVEL = CurrentUser.usuario.nidUsuario;

            using (DataTable dtEstoque = new EstoqueData().ObterListaUsuario(estoqueFiltro))
            {
                List<SelectListItem> listaEstoque = (from r in dtEstoque.Rows.Cast<DataRow>()
                                                     select new SelectListItem()
                                                     {
                                                         Value = r["ID_ESTOQUE"].ToString(),
                                                         Text = r["CD_ESTOQUE"].ToString()
                                                     }).ToList();

                listaEstoque.Insert(0, new SelectListItem() { Value = "", Text = "Selecione...", Selected = true });
                model.ListaEstoque = listaEstoque;
            }

            using (DataTable dtPeca = new PecaData().ObterListaNew(new PecaEntity()))
            {
                List<SelectListItem> listaPecas = (from r in dtPeca.Rows.Cast<DataRow>()
                                                   select new SelectListItem()
                                                   {
                                                       Value = r["CD_PECA"].ToString(),
                                                       Text = r["DS_PECA"].ToString()
                                                   }).ToList();
                listaPecas.Insert(0, new SelectListItem() { Value = "", Text = "TODAS", Selected = true });
                model.ListaPecas = listaPecas;
            }

            List<SelectListItem> listaTipoMovimentacao = ObterListaTipoMovimentacao().ToList();
            listaTipoMovimentacao.Insert(0, new SelectListItem() { Value = "", Text = "TODAS", Selected = true });
            model.ListaTipoMovimentacao = listaTipoMovimentacao;


            model.PeriodoDe = DateTime.Today.AddDays(-30).ToShortDateString();
            model.PeriodoAte = DateTime.Today.ToShortDateString();

            return View(model);
        }


        /// <summary>
        /// Grid "Consulta de Movimentação de Estoque" da Página "Consulta de Movimentação de Estoque"
        /// </summary>
        /// <param name="codigoEstoque"></param>
        /// <returns></returns>
        [_3MAuthentication]
        public JsonResult PesquisarMovimentacao(PesquisaMovimentacaoModel filtroMovimentacao)
        {

            Func<DataRow, EstoqueMovimentacao> modelConverter = new Func<DataRow, EstoqueMovimentacao>(dr =>
            {
                EstoqueMovimentacao model = new EstoqueMovimentacao();

                model.DT_MOVIMENTACAO = dr["DT_MOVIMENTACAO"].ToString();
                model.CD_PECA = dr["DS_PECA"].ToString();
                model.TpEstoqueMovi.DS_NOME_REDUZ = dr["DS_NOME_REDUZ"].ToString();
                model.TP_ENTRADA_SAIDA = ControlesUtility.Dicionarios.TipoEntradaSaida()[dr["TP_ENTRADA_SAIDA"].ToString()];
                model.QT_PECA = Convert.ToDecimal(dr["QT_PECA"].ToString());
                model.usuario.cnmNome = dr["NM_USU_MOVI"].ToString();
                model.EstoqueOrigem.cdsEstoque = dr["CD_ESTOQUE_ORIGEM"].ToString();

                return model;
            });
            try
            {
                EstoqueMoviEntity filtroEstoque = new EstoqueMoviEntity();

                filtroEstoque.ESTOQUE_ORIGEM.ID_ESTOQUE = Convert.ToInt64(filtroMovimentacao.CodigoEstoque);
                filtroEstoque.ESTOQUE_ORIGEM.ID_USU_RESPONSAVEL = CurrentUser.usuario.nidUsuario;

                if (!string.IsNullOrEmpty(filtroMovimentacao.CodigoPeca))
                {
                    filtroEstoque.Peca.CD_PECA = filtroMovimentacao.CodigoPeca;
                }

                if (!string.IsNullOrEmpty(filtroMovimentacao.TipoMovimentacao))
                {
                    filtroEstoque.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO = filtroMovimentacao.TipoMovimentacao;
                }

                EstoqueMoviData data = new EstoqueMoviData();
                DataTable result = data.ObterListaMovimento(filtroEstoque, Convert.ToDateTime(filtroMovimentacao.PeriodoDe), Convert.ToDateTime(filtroMovimentacao.PeriodoAte));
                var lista = from dr in result.Rows.Cast<DataRow>()
                            select modelConverter(dr);

                Dictionary<string, object> jsonResult = new Dictionary<string, object>();
                jsonResult.Add("Html", RenderRazorViewToString("~/Views/MovimPecas/_gridMovimentacao.cshtml", lista.ToList()));
                jsonResult.Add("Status", "Success");

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

        }

        protected internal List<EstoquePeca> ConsultarEstoquePorCodigo(long codigoEstoque, UsuarioPerfilEntity usuario)
        {
            Func<DataRow, EstoquePeca> modelConverter = new Func<DataRow, EstoquePeca>((d) =>
            {
                EstoquePeca estoquePeca = new EstoquePeca();
                estoquePeca.Peca.CD_PECA = d["CD_PECA"].ToString();
                estoquePeca.Peca.DS_PECA = d["DS_PECA"].ToString();
                estoquePeca.QT_PECA_ATUAL = Convert.ToDecimal(d["QT_PECA_ATUAL"]).ToString();
                estoquePeca.DT_ULT_MOVIM = d["DT_ULT_MOVIM"].ToString();
                return estoquePeca;
            });

            EstoquePecaData estoquePecaData = new EstoquePecaData();
            DataTable dtEstoque = estoquePecaData.ObterListaPorEstoque(codigoEstoque, string.Empty, usuario.usuario.nidUsuario);
            var listaEstoque = from dr in dtEstoque.Rows.Cast<DataRow>()
                               select modelConverter(dr);
            return listaEstoque.ToList();

        }

        /// <summary>
        /// Grid "Consulta de Estoque da Página Principal"
        /// </summary>
        /// <param name="codigoEstoque"></param>
        /// <returns></returns>
        [_3MAuthentication]
        public JsonResult ConsultarEstoque(long codigoEstoque)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            try
            {
                var listaEstoque = this.ConsultarEstoquePorCodigo(codigoEstoque, CurrentUser);
                jsonResult.Add("Html", RenderRazorViewToString("~/Views/MovimPecas/_gridEstoque.cshtml", listaEstoque));
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

        [_3MAuthentication]
        public ActionResult Imprimir(string idKey)
        {
            string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
            return Redirect(URLSite + $"/RelatorioMovimPecas.aspx?IdKey={ControlesUtility.Criptografia.Criptografar(idKey)}");
        }

    }

    public class PesquisaMovimentacaoModel
    {
        public IEnumerable<SelectListItem> ListaEstoque { get; set; }
        public IEnumerable<SelectListItem> ListaPecas { get; set; }
        public IEnumerable<SelectListItem> ListaTipoMovimentacao { get; set; }


        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string CodigoEstoque { get; set; }
        public string PeriodoDe { get; set; }
        public string PeriodoAte { get; set; }

        public string CodigoPeca { get; set; }
        public string TipoMovimentacao { get; set; }

    }
}
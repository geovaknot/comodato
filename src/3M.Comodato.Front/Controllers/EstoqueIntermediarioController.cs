using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class EstoqueIntermediarioController : BaseController
    {
        // GET: EstoqueIntermediario
        [_3MAuthentication]
        public ActionResult Index()
        {
            //return View(new List<EstoquePeca>());
            return View();
        }

        [_3MAuthentication]
        public JsonResult ConsultarEstoquePecas(EstoquePeca estoquePeca)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            try
            {
                Func<DataRow, EstoqueIntermediario> modelConverter = new Func<DataRow, EstoqueIntermediario>((d) =>
                {
                    EstoqueIntermediario info = new EstoqueIntermediario();
                    info.Peca.DS_PECA = d["DS_PECA"].ToString();
                    info.Peca.TX_UNIDADE = d["TX_UNIDADE"].ToString();
                    info.Peca.VL_PECA = d["VL_PECA"].ToString();
                    info.DT_ULT_MOVIM = Convert.ToDateTime(d["DT_ULT_MOVIM"]).ToShortDateString();
                    info.QT_PECA_ATUAL = Convert.ToDecimal(d["QT_PECA_ATUAL"]).ToString();
                    info.QTD_RECEBIDA_NAO_APROVADA = Convert.ToDecimal(d["QTD_REC_NAO_APROV"].ToString());

                    if (null == d["FL_ATIVO_PECA"].ToString())
                        info.cdsAtivo = ControlesUtility.Dicionarios.SimNao()[d["FL_ATIVO_PECA"].ToString()];

                    info.Estoque.cdsEstoque = d["ESTOQUE"].ToString();

                    return info;
                });

                EstoquePecaEntity estoqueEntity = new EstoquePecaEntity()
                {
                    estoque = new EstoqueEntity() { ID_ESTOQUE = estoquePeca.Estoque.nidEstoque, ID_USU_RESPONSAVEL = CurrentUser.usuario.nidUsuario },
                    peca = new PecaEntity() { CD_PECA = estoquePeca.Peca.CD_PECA, FL_ATIVO_PECA = estoquePeca.Peca.FL_ATIVO_PECA }

                };

                EstoquePecaData estoquePecaData = new EstoquePecaData();
                DataTable dtEstoque = estoquePecaData.ObterListaIntermediario(estoqueEntity);

                IEnumerable<EstoqueIntermediario> listaEstoque = (from dr in dtEstoque.Rows.Cast<DataRow>()
                                                                  select modelConverter(dr)).ToList();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/EstoqueIntermediario/_gridEstoquePecas.cshtml", listaEstoque.ToList().OrderBy(x => x.Peca.DS_PECA)));
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
        public ActionResult Relatorio()
        {
            return View();
        }

        //[_3MAuthentication]
        //public ActionResult Imprimir(string idKey)
        //{
        //    idKey = ControlesUtility.Criptografia.Criptografar(idKey);
        //    return Redirect($"~/RelatorioEstoqueIntermediario.aspx?IdKey={idKey}");
        //}

    }
}
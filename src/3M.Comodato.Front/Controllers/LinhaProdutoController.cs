using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;

namespace _3M.Comodato.Front.Controllers
{
    public class LinhaProdutoController : BaseController
    {
        // GET: LinhaProduto
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.LinhaProduto> linhaProdutos = new List<Models.LinhaProduto>();

            try
            {
                LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity();
                DataTableReader dataTableReader = new LinhaProdutoData().ObterLista(linhaProdutoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.LinhaProduto linha = new Models.LinhaProduto
                        {
                            CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_LINHA_PRODUTO"].ToString()),
                            DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                            VL_EXPECTATIVA_PADRAO = Convert.ToDecimal(dataTableReader["VL_EXPECTATIVA_PADRAO"]).ToString("N2")
                        };
                        linhaProdutos.Add(linha);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.LinhaProduto> iLinhaProdutos = linhaProdutos;
            return View(iLinhaProdutos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.LinhaProduto linha = new Models.LinhaProduto();
            return View(linha);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.LinhaProduto linha)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity
                    {
                        DS_LINHA_PRODUTO = linha.DS_LINHA_PRODUTO,
                        VL_EXPECTATIVA_PADRAO = Convert.ToDecimal(linha.VL_EXPECTATIVA_PADRAO),
                        nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario
                    };

                    new LinhaProdutoData().Inserir(ref linhaProdutoEntity);

                    linha.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(linha); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.LinhaProduto linhaProduto = null;

            try
            {
                LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity
                {
                    CD_LINHA_PRODUTO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey))
                };
                DataTableReader dataTableReader = new LinhaProdutoData().ObterLista(linhaProdutoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        linhaProduto = new Models.LinhaProduto
                        {
                            CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]),
                            DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                            VL_EXPECTATIVA_PADRAO = Convert.ToDecimal(dataTableReader["VL_EXPECTATIVA_PADRAO"]).ToString("N2")
                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (linhaProduto == null)
                return HttpNotFound();
            else
                return View(linhaProduto);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.LinhaProduto linhaProduto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity
                    {
                        CD_LINHA_PRODUTO = linhaProduto.CD_LINHA_PRODUTO,
                        DS_LINHA_PRODUTO = linhaProduto.DS_LINHA_PRODUTO,
                        VL_EXPECTATIVA_PADRAO = Convert.ToDecimal(linhaProduto.VL_EXPECTATIVA_PADRAO),
                        nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario
                    };

                    new LinhaProdutoData().Alterar(linhaProdutoEntity);

                    linhaProduto.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(linhaProduto); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.LinhaProduto linhaProduto = null;

            try
            {
                LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity
                {
                    CD_LINHA_PRODUTO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey))
                };
                DataTableReader dataTableReader = new LinhaProdutoData().ObterLista(linhaProdutoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        linhaProduto = new Models.LinhaProduto
                        {
                            CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]),
                            DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                            VL_EXPECTATIVA_PADRAO = Convert.ToDecimal(dataTableReader["VL_EXPECTATIVA_PADRAO"]).ToString("N2")
                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (linhaProduto == null)
                return HttpNotFound();
            else
                return View(linhaProduto);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.LinhaProduto linha = new Models.LinhaProduto();
            try
            {
                if (ModelState.IsValid)
                {
                    LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity
                    {
                        CD_LINHA_PRODUTO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey)),
                        nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario
                    };

                    new LinhaProdutoData().Excluir(linhaProdutoEntity);

                    linha.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(linha);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}
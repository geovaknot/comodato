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
    public class TBFuncaoController : BaseController
    {
        // GET: TBFuncao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.TBFuncao> tbFuncoes = new List<Models.TBFuncao>();

            try
            {
                TBFuncaoEntity tbFuncaoEntity = new TBFuncaoEntity();
                DataTableReader dataTableReader = new TBFuncaoData().ObterLista(tbFuncaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.TBFuncao tbFuncao = new Models.TBFuncao
                        {
                            CD_FUNCAO = Convert.ToInt32(dataTableReader["CD_FUNCAO"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_FUNCAO"].ToString()),
                            DS_FUNCAO = dataTableReader["DS_FUNCAO"].ToString()
                        };
                        tbFuncoes.Add(tbFuncao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.TBFuncao> iTBFuncoes = tbFuncoes;
            return View(iTBFuncoes);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.TBFuncao funcao = new Models.TBFuncao();
            return View(funcao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.TBFuncao tbFuncao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TBFuncaoEntity tbFuncaoEntity = new TBFuncaoEntity();

                    tbFuncaoEntity.DS_FUNCAO = tbFuncao.DS_FUNCAO;
                    tbFuncaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TBFuncaoData().Inserir(ref tbFuncaoEntity);

                    tbFuncao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(tbFuncao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.TBFuncao tbFuncao = null;

            try
            {
                TBFuncaoEntity tbFuncaoEntity = new TBFuncaoEntity();

                tbFuncaoEntity.CD_FUNCAO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new TBFuncaoData().ObterLista(tbFuncaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tbFuncao = new Models.TBFuncao
                        {
                            CD_FUNCAO = Convert.ToInt32(dataTableReader["CD_FUNCAO"]),
                            DS_FUNCAO = dataTableReader["DS_FUNCAO"].ToString()
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

            if (tbFuncao == null)
                return HttpNotFound();
            else
                return View(tbFuncao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.TBFuncao tbFuncao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TBFuncaoEntity tbFuncaoEntity = new TBFuncaoEntity();

                    tbFuncaoEntity.CD_FUNCAO = tbFuncao.CD_FUNCAO;
                    tbFuncaoEntity.DS_FUNCAO = tbFuncao.DS_FUNCAO;
                    tbFuncaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TBFuncaoData().Alterar(tbFuncaoEntity);

                    tbFuncao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(tbFuncao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.TBFuncao tbFuncao = null;

            try
            {
                TBFuncaoEntity tbFuncaoEntity = new TBFuncaoEntity();

                tbFuncaoEntity.CD_FUNCAO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new TBFuncaoData().ObterLista(tbFuncaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tbFuncao = new Models.TBFuncao
                        {
                            CD_FUNCAO = Convert.ToInt32(dataTableReader["CD_FUNCAO"]),
                            DS_FUNCAO = dataTableReader["DS_FUNCAO"].ToString()
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

            if (tbFuncao == null)
                return HttpNotFound();
            else
                return View(tbFuncao);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.TBFuncao funcao = new Models.TBFuncao();
            try
            {
                if (ModelState.IsValid)
                {
                    TBFuncaoEntity TBFuncaoEntity = new TBFuncaoEntity();

                    TBFuncaoEntity.CD_FUNCAO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                    TBFuncaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TBFuncaoData().Excluir(TBFuncaoEntity);

                    funcao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(funcao);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

    }
}
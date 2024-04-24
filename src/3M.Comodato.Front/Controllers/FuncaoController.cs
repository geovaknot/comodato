using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Controllers
{
    public class FuncaoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Funcao> funcoes = new List<Models.Funcao>();

            try
            {
                FuncaoEntity funcaoEntity = new FuncaoEntity();
                DataTableReader dataTableReader = new FuncaoData().ObterLista(funcaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Funcao Funcao = new Models.Funcao
                        {
                            nidFuncao = Convert.ToInt64(dataTableReader["nidFuncao"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidFuncao"].ToString()),
                            ccdFuncao = dataTableReader["ccdFuncao"].ToString(),
                            cdsFuncao = dataTableReader["cdsFuncao"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Sim" : "Não")
                        };
                        funcoes.Add(Funcao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Funcao> iFuncoes = funcoes;
            return View(iFuncoes);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Funcao funcao = new Models.Funcao();
            return View(funcao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Funcao funcao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FuncaoEntity funcaoEntity = new FuncaoEntity();

                    funcaoEntity.ccdFuncao = funcao.ccdFuncao;
                    funcaoEntity.cdsFuncao = funcao.cdsFuncao;
                    funcaoEntity.bidAtivo = funcao.bidAtivo;
                    funcaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new FuncaoData().Inserir(ref funcaoEntity);

                    funcao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(funcao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Funcao funcao = null;

            try
            {
                FuncaoEntity funcaoEntity = new FuncaoEntity();

                funcaoEntity.nidFuncao = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new FuncaoData().ObterLista(funcaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        funcao = new Models.Funcao
                        {
                            nidFuncao = Convert.ToInt64(dataTableReader["nidFuncao"]),
                            ccdFuncao = dataTableReader["ccdFuncao"].ToString(),
                            cdsFuncao = dataTableReader["cdsFuncao"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"])
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

            if (funcao == null)
                return HttpNotFound();
            else
                return View(funcao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Funcao funcao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FuncaoEntity funcaoEntity = new FuncaoEntity();

                    funcaoEntity.nidFuncao = funcao.nidFuncao;
                    funcaoEntity.ccdFuncao = funcao.ccdFuncao;
                    funcaoEntity.cdsFuncao = funcao.cdsFuncao;
                    funcaoEntity.bidAtivo = funcao.bidAtivo;
                    funcaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new FuncaoData().Alterar(funcaoEntity);

                    funcao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(funcao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Funcao funcao = null;

            try
            {
                FuncaoEntity funcaoEntity = new FuncaoEntity();

                funcaoEntity.nidFuncao = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new FuncaoData().ObterLista(funcaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        funcao = new Models.Funcao
                        {
                            nidFuncao = Convert.ToInt64(dataTableReader["nidFuncao"]),
                            ccdFuncao = dataTableReader["ccdFuncao"].ToString(),
                            cdsFuncao = dataTableReader["cdsFuncao"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"])
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

            if (funcao == null)
                return HttpNotFound();
            else
                return View(funcao);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Funcao funcao = new Models.Funcao();
            try
            {
                if (ModelState.IsValid)
                {
                    FuncaoEntity funcaoEntity = new FuncaoEntity();

                    funcaoEntity.nidFuncao = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    funcaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new FuncaoData().Excluir(funcaoEntity);
                                        
                    funcao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(funcao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

    }
}
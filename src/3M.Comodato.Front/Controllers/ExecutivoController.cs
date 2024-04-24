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
    public class ExecutivoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Executivo> executivos = new List<Models.Executivo>();

            try
            {
                ExecutivoEntity executivoEntity = new ExecutivoEntity();
                DataTableReader dataTableReader = new ExecutivoData().ObterLista(executivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Executivo Executivo = new Models.Executivo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_EXECUTIVO"].ToString()),
                            CD_EXECUTIVO = Convert.ToInt64(dataTableReader["CD_EXECUTIVO"].ToString()),
                            NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString(),

                        };
                        executivos.Add(Executivo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Executivo> iExecutivos = executivos;
            return View(iExecutivos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Executivo executivo = new Models.Executivo();
            return View(executivo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Executivo executivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ExecutivoEntity executivoEntity = new ExecutivoEntity();

                    // razaoComodatoEntity.CD_RAZAO_COMODATO = razaoComodato.CD_RAZAO_COMODATO;
                    executivoEntity.NM_EXECUTIVO = executivo.NM_EXECUTIVO;
                    executivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ExecutivoData().Inserir(ref executivoEntity);

                    executivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(executivo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Executivo executivo = null;

            try
            {
                ExecutivoEntity executivoEntity = new ExecutivoEntity();

                executivoEntity.CD_EXECUTIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ExecutivoData().ObterLista(executivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        executivo = new Models.Executivo
                        {
                            CD_EXECUTIVO = Convert.ToInt64(dataTableReader["CD_EXECUTIVO"].ToString()),
                            NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString()
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

            if (executivo == null)
                return HttpNotFound();
            else
                return View(executivo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Executivo executivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ExecutivoEntity executivoEntity = new ExecutivoEntity();

                    executivoEntity.CD_EXECUTIVO = executivo.CD_EXECUTIVO;
                    executivoEntity.NM_EXECUTIVO = executivo.NM_EXECUTIVO;
                    executivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ExecutivoData().Alterar(executivoEntity);

                    executivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(executivo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Executivo executivo = null;

            try
            {
                ExecutivoEntity executivoEntity = new ExecutivoEntity();

                executivoEntity.CD_EXECUTIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ExecutivoData().ObterLista(executivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        executivo = new Models.Executivo
                        {
                            CD_EXECUTIVO = Convert.ToInt64(dataTableReader["CD_EXECUTIVO"].ToString()),
                            NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString()
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

            if (executivo == null)
                return HttpNotFound();
            else
                return View(executivo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Executivo executivo = new Models.Executivo();
            try
            {
                if (ModelState.IsValid)
                {
                    ExecutivoEntity executivoEntity = new ExecutivoEntity();

                    executivoEntity.CD_EXECUTIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    executivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ExecutivoData().Excluir(executivoEntity);

                    executivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(executivo);

            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}
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
    public class StatusAtivoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.StatusAtivo> statusAtivos = new List<Models.StatusAtivo>();

            try
            {
                StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();
                DataTableReader dataTableReader = new StatusAtivoData().ObterLista(statusAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.StatusAtivo StatusAtivo = new Models.StatusAtivo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_STATUS_ATIVO"].ToString()),
                            CD_STATUS_ATIVO = Convert.ToInt64(dataTableReader["CD_STATUS_ATIVO"].ToString()),
                            DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString(),

                        };
                        statusAtivos.Add(StatusAtivo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.StatusAtivo> iStatusAtivos = statusAtivos;
            return View(iStatusAtivos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.StatusAtivo statusAtivo = new Models.StatusAtivo();
            return View(statusAtivo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.StatusAtivo statusAtivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();

                    statusAtivoEntity.CD_STATUS_ATIVO = statusAtivo.CD_STATUS_ATIVO;
                    statusAtivoEntity.DS_STATUS_ATIVO = statusAtivo.DS_STATUS_ATIVO;
                    statusAtivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new StatusAtivoData().Inserir(ref statusAtivoEntity);

                    statusAtivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(statusAtivo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.StatusAtivo statusAtivo = null;

            try
            {
                StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();

                statusAtivoEntity.CD_STATUS_ATIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new StatusAtivoData().ObterLista(statusAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        statusAtivo = new Models.StatusAtivo
                        {
                            CD_STATUS_ATIVO = Convert.ToInt64(dataTableReader["CD_STATUS_ATIVO"].ToString()),
                            DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString()
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

            if (statusAtivo == null)
                return HttpNotFound();
            else
                return View(statusAtivo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.StatusAtivo statusAtivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();

                    statusAtivoEntity.CD_STATUS_ATIVO = statusAtivo.CD_STATUS_ATIVO;
                    statusAtivoEntity.DS_STATUS_ATIVO = statusAtivo.DS_STATUS_ATIVO;
                    statusAtivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new StatusAtivoData().Alterar(statusAtivoEntity);

                    statusAtivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(statusAtivo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.StatusAtivo statusAtivo = null;

            try
            {
                StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();

                statusAtivoEntity.CD_STATUS_ATIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new StatusAtivoData().ObterLista(statusAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        statusAtivo = new Models.StatusAtivo
                        {
                            CD_STATUS_ATIVO = Convert.ToInt64(dataTableReader["CD_STATUS_ATIVO"].ToString()),
                            DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString()
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

            if (statusAtivo == null)
                return HttpNotFound();
            else
                return View(statusAtivo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.StatusAtivo status = new Models.StatusAtivo();
            try
            {
                if (ModelState.IsValid)
                {
                    StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();

                    statusAtivoEntity.CD_STATUS_ATIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    statusAtivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new StatusAtivoData().Excluir(statusAtivoEntity);

                    status.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(status);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}
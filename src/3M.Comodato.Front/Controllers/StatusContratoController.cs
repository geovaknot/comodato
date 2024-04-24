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
    public class StatusContratoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.StatusContrato> statusContratos = new List<Models.StatusContrato>();

            try
            {
                StatusContratoEntity statusContratoEntity = new StatusContratoEntity();
                DataTableReader dataTableReader = new StatusContratoData().ObterLista(statusContratoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.StatusContrato StatusContrato = new Models.StatusContrato
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_STATUS_CONTRATO"].ToString()),
                            CD_STATUS_CONTRATO = dataTableReader["CD_STATUS_CONTRATO"].ToString(),
                            DS_STATUS_CONTRATO = dataTableReader["DS_STATUS_CONTRATO"].ToString()
                        };
                        statusContratos.Add(StatusContrato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.StatusContrato> iStatusContratos = statusContratos;
            return View(iStatusContratos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.StatusContrato statusContrato = new Models.StatusContrato();
            statusContrato.CancelarVerificarCodigo = false;

            return View(statusContrato);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.StatusContrato statusContrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool Gravar = true;
                    StatusContratoEntity statusContratoEntity = new StatusContratoEntity();

                    //verifica se já existe esse registro no banco
                    statusContratoEntity.CD_STATUS_CONTRATO = statusContrato.CD_STATUS_CONTRATO;
                    DataTableReader dataTableReader = new StatusContratoData().ObterLista(statusContratoEntity).CreateDataReader();
                    if (dataTableReader.HasRows)//possui registro, então não inclui
                    {
                        if (dataTableReader.Read())
                        {
                            ViewBag.Mensagem = "Código já castrado!";
                            Gravar = false;
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    if (Gravar == true)
                    {
                        statusContratoEntity.CD_STATUS_CONTRATO = statusContrato.CD_STATUS_CONTRATO;
                        statusContratoEntity.DS_STATUS_CONTRATO = statusContrato.DS_STATUS_CONTRATO;
                        statusContratoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                        new StatusContratoData().Inserir(ref statusContratoEntity);

                        statusContrato.JavaScriptToRun = "MensagemSucesso()";
                        //return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(statusContrato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.StatusContrato statusContrato = null;

            try
            {
                StatusContratoEntity statusContratoEntity = new StatusContratoEntity();

                statusContratoEntity.CD_STATUS_CONTRATO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new StatusContratoData().ObterLista(statusContratoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        statusContrato = new Models.StatusContrato
                        {
                            CD_STATUS_CONTRATO = dataTableReader["CD_STATUS_CONTRATO"].ToString(),
                            DS_STATUS_CONTRATO = dataTableReader["DS_STATUS_CONTRATO"].ToString(),
                            CancelarVerificarCodigo = true
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

            if (statusContrato == null)
                return HttpNotFound();
            else
                return View(statusContrato);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.StatusContrato statusContrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StatusContratoEntity statusContratoEntity = new StatusContratoEntity();

                    statusContratoEntity.CD_STATUS_CONTRATO = statusContrato.CD_STATUS_CONTRATO;
                    statusContratoEntity.DS_STATUS_CONTRATO = statusContrato.DS_STATUS_CONTRATO;
                    statusContratoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new StatusContratoData().Alterar(statusContratoEntity);

                    statusContrato.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(statusContrato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.StatusContrato statusContrato = null;

            try
            {
                StatusContratoEntity statusContratoEntity = new StatusContratoEntity();

                statusContratoEntity.CD_STATUS_CONTRATO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new StatusContratoData().ObterLista(statusContratoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        statusContrato = new Models.StatusContrato
                        {
                            CD_STATUS_CONTRATO = dataTableReader["CD_STATUS_CONTRATO"].ToString(),
                            DS_STATUS_CONTRATO = dataTableReader["DS_STATUS_CONTRATO"].ToString()
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

            if (statusContrato == null)
                return HttpNotFound();
            else
                return View(statusContrato);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.StatusContrato status = new Models.StatusContrato();
            try
            {
                if (ModelState.IsValid)
                {
                    StatusContratoEntity statusContratoEntity = new StatusContratoEntity();

                    statusContratoEntity.CD_STATUS_CONTRATO = ControlesUtility.Criptografia.Descriptografar(idKey);
                    statusContratoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new StatusContratoData().Excluir(statusContratoEntity);

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

        public ActionResult VerificarCodigo(string CD_STATUS_CONTRATO, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {
                if (CancelarVerificarCodigo == false)
                {
                    StatusContratoEntity statusContratoEntity = new StatusContratoEntity();

                    statusContratoEntity.CD_STATUS_CONTRATO = CD_STATUS_CONTRATO;
                    DataTableReader dataTableReader = new StatusContratoData().ObterLista(statusContratoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                            Liberado = false;
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return Json(Liberado, JsonRequestBehavior.AllowGet);
        }
    }
}
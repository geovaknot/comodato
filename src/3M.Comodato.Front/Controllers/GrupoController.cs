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
    public class GrupoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Grupo> grupos = new List<Models.Grupo>();

            try
            {
                GrupoEntity grupoEntity = new GrupoEntity();
                DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Grupo Grupo = new Models.Grupo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_GRUPO"].ToString()),
                            CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                            DS_GRUPO = dataTableReader["DS_GRUPO"].ToString()
                        };
                        grupos.Add(Grupo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Grupo> iGrupos = grupos;
            return View(iGrupos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Grupo grupo = new Models.Grupo();
            grupo.CancelarVerificarCodigo = false;

            return View(grupo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Grupo grupo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool Gravar = true;
                    GrupoEntity grupoEntity = new GrupoEntity();

                    if (!string.IsNullOrEmpty(grupo.CD_GRUPO))
                    {
                        //verifica se já existe esse registro no banco
                        grupoEntity.CD_GRUPO = grupo.CD_GRUPO;
                        DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();
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
                    }

                    if (Gravar == true)
                    {
                        grupoEntity.CD_GRUPO = grupo.CD_GRUPO;
                        grupoEntity.DS_GRUPO = grupo.DS_GRUPO;
                        grupoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                        new GrupoData().Inserir(ref grupoEntity);

                        grupo.JavaScriptToRun = "MensagemSucesso()";
                        //return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(grupo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Grupo grupo = null;

            try
            {
                GrupoEntity grupoEntity = new GrupoEntity();

                grupoEntity.CD_GRUPO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        grupo = new Models.Grupo
                        {
                            CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                            DS_GRUPO = dataTableReader["DS_GRUPO"].ToString(),
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

            if (grupo == null)
                return HttpNotFound();
            else
                return View(grupo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Grupo grupo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    GrupoEntity grupoEntity = new GrupoEntity();

                    grupoEntity.CD_GRUPO = grupo.CD_GRUPO;
                    grupoEntity.DS_GRUPO = grupo.DS_GRUPO;
                    grupoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new GrupoData().Alterar(grupoEntity);

                    grupo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(grupo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Grupo grupo = null;

            try
            {
                GrupoEntity grupoEntity = new GrupoEntity();

                grupoEntity.CD_GRUPO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        grupo = new Models.Grupo
                        {
                            CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                            DS_GRUPO = dataTableReader["DS_GRUPO"].ToString()
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

            if (grupo == null)
                return HttpNotFound();
            else
                return View(grupo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Grupo grupo = new Models.Grupo();
            try
            {
                if (ModelState.IsValid)
                {
                    GrupoEntity grupoEntity = new GrupoEntity();

                    grupoEntity.CD_GRUPO = ControlesUtility.Criptografia.Descriptografar(idKey);
                    grupoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new GrupoData().Excluir(grupoEntity);

                    grupo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(grupo);
          //  return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult VerificarCodigo(string CD_GRUPO, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {
                if (CancelarVerificarCodigo == false)
                {
                    GrupoEntity grupoEntity = new GrupoEntity();

                    grupoEntity.CD_GRUPO = CD_GRUPO;
                    DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();

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

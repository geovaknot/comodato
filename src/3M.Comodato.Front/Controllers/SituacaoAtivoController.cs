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
    public class SituacaoAtivoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.SituacaoAtivo> situacaoAtivos = new List<Models.SituacaoAtivo>();

            try
            {
                SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();
                DataTableReader dataTableReader = new SituacaoAtivoData().ObterLista(situacaoAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.SituacaoAtivo SituacaoAtivo = new Models.SituacaoAtivo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_SITUACAO_ATIVO"].ToString()),
                            CD_SITUACAO_ATIVO = Convert.ToInt64(dataTableReader["CD_SITUACAO_ATIVO"].ToString()),
                            DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString(),
                        };
                        situacaoAtivos.Add(SituacaoAtivo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.SituacaoAtivo> iSituacaoAtivos = situacaoAtivos;
            return View(iSituacaoAtivos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.SituacaoAtivo situacao = new Models.SituacaoAtivo();
            return View(situacao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.SituacaoAtivo situacaoAtivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();

                    situacaoAtivoEntity.CD_SITUACAO_ATIVO = situacaoAtivo.CD_SITUACAO_ATIVO;
                    situacaoAtivoEntity.DS_SITUACAO_ATIVO = situacaoAtivo.DS_SITUACAO_ATIVO;
                    situacaoAtivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new SituacaoAtivoData().Inserir(ref situacaoAtivoEntity);

                    situacaoAtivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(situacaoAtivo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.SituacaoAtivo situacaoAtivo = null;

            try
            {
                SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();

                situacaoAtivoEntity.CD_SITUACAO_ATIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new SituacaoAtivoData().ObterLista(situacaoAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        situacaoAtivo = new Models.SituacaoAtivo
                        {
                            CD_SITUACAO_ATIVO = Convert.ToInt64(dataTableReader["CD_SITUACAO_ATIVO"].ToString()),
                            DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString()
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

            if (situacaoAtivo == null)
                return HttpNotFound();
            else
                return View(situacaoAtivo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.SituacaoAtivo situacaoAtivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();

                    situacaoAtivoEntity.CD_SITUACAO_ATIVO = situacaoAtivo.CD_SITUACAO_ATIVO;
                    situacaoAtivoEntity.DS_SITUACAO_ATIVO = situacaoAtivo.DS_SITUACAO_ATIVO;
                    situacaoAtivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new SituacaoAtivoData().Alterar(situacaoAtivoEntity);

                    situacaoAtivo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(situacaoAtivo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.SituacaoAtivo situacaoAtivo = null;

            try
            {
                SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();

                situacaoAtivoEntity.CD_SITUACAO_ATIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new SituacaoAtivoData().ObterLista(situacaoAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        situacaoAtivo = new Models.SituacaoAtivo
                        {
                            CD_SITUACAO_ATIVO = Convert.ToInt64(dataTableReader["CD_SITUACAO_ATIVO"].ToString()),
                            DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString()
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

            if (situacaoAtivo == null)
                return HttpNotFound();
            else
                return View(situacaoAtivo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.SituacaoAtivo situacao = new Models.SituacaoAtivo();
            try
            {
                if (ModelState.IsValid)
                {
                    SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();

                    situacaoAtivoEntity.CD_SITUACAO_ATIVO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    situacaoAtivoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new SituacaoAtivoData().Excluir(situacaoAtivoEntity);

                    situacao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(situacao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}
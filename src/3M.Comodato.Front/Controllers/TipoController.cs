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
    public class TipoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Tipo> tipos = new List<Models.Tipo>();

            try
            {
                TipoEntity tipoEntity = new TipoEntity();
                DataTableReader dataTableReader = new TipoData().ObterLista(tipoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Tipo Tipo = new Models.Tipo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_TIPO"].ToString()),
                            CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"].ToString()),
                            DS_TIPO = dataTableReader["DS_TIPO"].ToString(),

                        };
                        tipos.Add(Tipo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Tipo> iTipos = tipos;
            return View(iTipos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Tipo tipo = new Models.Tipo();
            return View(tipo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Tipo tipo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoEntity tipoEntity = new TipoEntity();

                    tipoEntity.CD_TIPO = tipo.CD_TIPO;
                    tipoEntity.DS_TIPO = tipo.DS_TIPO;
                    tipoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TipoData().Inserir(ref tipoEntity);

                    tipo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(tipo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Tipo tipo = null;

            try
            {
                TipoEntity tipoEntity = new TipoEntity();

                tipoEntity.CD_TIPO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new TipoData().ObterLista(tipoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tipo = new Models.Tipo
                        {
                            CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"].ToString()),
                            DS_TIPO = dataTableReader["DS_TIPO"].ToString()
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

            if (tipo == null)
                return HttpNotFound();
            else
                return View(tipo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Tipo tipo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoEntity tipoEntity = new TipoEntity();

                    tipoEntity.CD_TIPO = tipo.CD_TIPO;
                    tipoEntity.DS_TIPO = tipo.DS_TIPO;
                    tipoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TipoData().Alterar(tipoEntity);

                    tipo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(tipo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Tipo tipo = null;

            try
            {
                TipoEntity tipoEntity = new TipoEntity();

                tipoEntity.CD_TIPO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new TipoData().ObterLista(tipoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tipo = new Models.Tipo
                        {
                            CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"].ToString()),
                            DS_TIPO = dataTableReader["DS_TIPO"].ToString()
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

            if (tipo == null)
                return HttpNotFound();
            else
                return View(tipo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Tipo tipo = new Models.Tipo();
            try
            {
                if (ModelState.IsValid)
                {
                    TipoEntity tipoEntity = new TipoEntity();

                    tipoEntity.CD_TIPO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    tipoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TipoData().Excluir(tipoEntity);

                    tipo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(tipo);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}
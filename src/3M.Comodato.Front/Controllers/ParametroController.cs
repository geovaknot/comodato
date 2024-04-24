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
    public class ParametroController : BaseController
    {
        // GET: Parametro
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Parametro> parametro = new List<Models.Parametro>();

            try
            {
                ParametroEntity parametroEntity = new ParametroEntity();

                if (CurrentUser.perfil.nidPerfil != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M))
                    parametroEntity.flgTipoParametro = "N"; //Parâmetros de Negócio (Padrão)
                 
                DataTableReader dataTableReader = new ParametroData().ObterLista(parametroEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Parametro Parametro = new Models.Parametro
                        {
                            nidParametro = Convert.ToInt64(dataTableReader["nidParametro"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidParametro"].ToString()),
                            ccdParametro = dataTableReader["ccdParametro"].ToString(),
                            cdsParametro = dataTableReader["cdsParametro"].ToString(),
                            cvlParametro = dataTableReader["cvlParametro"].ToString(),
                            flgTipoParametro = (dataTableReader["flgTipoParametro"].ToString() == "N" ? "Negócio" : "Sistema"),
                        };
                        parametro.Add(Parametro);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Parametro> iPerfis = parametro;
            return View(iPerfis);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Parametro parametro = new Models.Parametro();
            parametro.tiposParametros = ControlesUtility.Dicionarios.TipoParametro();
            return View(parametro);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Parametro parametro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ParametroEntity parametroEntity = new ParametroEntity();

                    parametroEntity.ccdParametro = parametro.ccdParametro;
                    parametroEntity.cdsParametro = parametro.cdsParametro;
                    parametroEntity.cvlParametro = parametro.cvlParametro;
                    parametroEntity.flgTipoParametro = parametro.flgTipoParametro;
                    parametroEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ParametroData().Inserir(ref parametroEntity);

                    parametro.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            parametro.tiposParametros = ControlesUtility.Dicionarios.TipoParametro();
            return View(parametro); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Parametro parametro = null;

            try
            {
                ParametroEntity parametroEntity = new ParametroEntity();

                parametroEntity.nidParametro = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ParametroData().ObterLista(parametroEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        parametro = new Models.Parametro
                        {
                            nidParametro = Convert.ToInt64(dataTableReader["nidParametro"]),
                            ccdParametro = dataTableReader["ccdParametro"].ToString(),
                            cdsParametro = dataTableReader["cdsParametro"].ToString(),
                            cvlParametro = dataTableReader["cvlParametro"].ToString(),
                            flgTipoParametro = dataTableReader["flgTipoParametro"].ToString(),
                        };

                        parametro.tiposParametros = ControlesUtility.Dicionarios.TipoParametro();
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

            if (parametro == null)
                return HttpNotFound();
            else
                return View(parametro);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Parametro parametro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ParametroEntity parametroEntity = new ParametroEntity();

                    parametroEntity.nidParametro = parametro.nidParametro;
                    parametroEntity.ccdParametro = parametro.ccdParametro;
                    parametroEntity.cdsParametro = parametro.cdsParametro;
                    parametroEntity.cvlParametro = parametro.cvlParametro;
                    parametroEntity.flgTipoParametro = parametro.flgTipoParametro;
                    parametroEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ParametroData().Alterar(parametroEntity);

                    parametro.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            parametro.tiposParametros = ControlesUtility.Dicionarios.TipoParametro();
            return View(parametro); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Parametro parametro = null;

            try
            {
                ParametroEntity parametroEntity = new ParametroEntity();

                parametroEntity.nidParametro = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ParametroData().ObterLista(parametroEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        parametro = new Models.Parametro
                        {
                            nidParametro = Convert.ToInt64(dataTableReader["nidParametro"]),
                            ccdParametro = dataTableReader["ccdParametro"].ToString(),
                            cdsParametro = dataTableReader["cdsParametro"].ToString(),
                            cvlParametro = dataTableReader["cvlParametro"].ToString(),
                            flgTipoParametro = (dataTableReader["flgTipoParametro"].ToString() == "N" ? "Negócio" : "Sistema"),
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

            if (parametro == null)
                return HttpNotFound();
            else
                return View(parametro);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Parametro parametro = new Models.Parametro();
            try
            {
                if (ModelState.IsValid)
                {
                    ParametroEntity parametroEntity = new ParametroEntity();

                    parametroEntity.nidParametro = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    parametroEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ParametroData().Excluir(parametroEntity);

                    parametro.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(parametro);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

    }
}
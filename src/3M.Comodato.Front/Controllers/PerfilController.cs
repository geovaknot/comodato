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
    public class PerfilController : BaseController
    {
        // GET: Perfil
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Perfil> perfis = new List<Models.Perfil>();

            try
            {
                PerfilEntity perfilEntity = new PerfilEntity();
                DataTableReader dataTableReader = new PerfilData().ObterLista(perfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Perfil perfil = new Models.Perfil
                        {
                            nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidPerfil"].ToString()),
                            cdsPerfil = dataTableReader["cdsPerfil"].ToString(),
                            ccdPerfil = Convert.ToInt32(dataTableReader["ccdPerfil"]),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Sim" : "Não")
                        };
                        perfis.Add(perfil);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Perfil> iPerfis = perfis;
            return View(iPerfis);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Perfil perfil = new Models.Perfil();
            return View(perfil);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Perfil perfil)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PerfilEntity perfilEntity = new PerfilEntity();

                    perfilEntity.cdsPerfil = perfil.cdsPerfil;
                    perfilEntity.ccdPerfil = (int)perfil.ccdPerfil;
                    perfilEntity.bidAtivo = perfil.bidAtivo;
                    perfilEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new PerfilData().Inserir(ref perfilEntity);

                    perfil.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(perfil); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Perfil perfil = null;

            try
            {
                PerfilEntity perfilEntity = new PerfilEntity();

                perfilEntity.nidPerfil = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new PerfilData().ObterLista(perfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        perfil = new Models.Perfil
                        {
                            nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]),
                            cdsPerfil = dataTableReader["cdsPerfil"].ToString(),
                            ccdPerfil = Convert.ToInt32(dataTableReader["ccdPerfil"]),
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

            if (perfil == null)
                return HttpNotFound();
            else
                return View(perfil);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Perfil perfil)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PerfilEntity perfilEntity = new PerfilEntity();

                    perfilEntity.nidPerfil = perfil.nidPerfil;
                    perfilEntity.cdsPerfil = perfil.cdsPerfil;
                    perfilEntity.ccdPerfil = (int)perfil.ccdPerfil;
                    perfilEntity.bidAtivo = perfil.bidAtivo;
                    perfilEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new PerfilData().Alterar(perfilEntity);

                    perfil.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(perfil); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Perfil perfil = null;

            try
            {
                PerfilEntity perfilEntity = new PerfilEntity();

                perfilEntity.nidPerfil = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new PerfilData().ObterLista(perfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        perfil = new Models.Perfil
                        {
                            nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]),
                            cdsPerfil = dataTableReader["cdsPerfil"].ToString(),
                            ccdPerfil = Convert.ToInt32(dataTableReader["ccdPerfil"]),
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

            if (perfil == null)
                return HttpNotFound();
            else
                return View(perfil);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Perfil perfil = new Models.Perfil();
            try
            {
                if (ModelState.IsValid)
                {
                    PerfilEntity perfilEntity = new PerfilEntity();

                    perfilEntity.nidPerfil = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    perfilEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new PerfilData().Excluir(perfilEntity);

                    perfil.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(perfil);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}
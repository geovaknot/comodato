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
    public class GrupoModeloController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.GrupoModelo> grupoModelos = new List<Models.GrupoModelo>();

            try
            {
                GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();
                DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.GrupoModelo grupoModelo = new Models.GrupoModelo
                        {
                            idKey = Utility.ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_GRUPOMODELO"].ToString()),
                            CD_GRUPO_MODELO = dataTableReader["CD_GRUPOMODELO"].ToString(),
                            DS_GRUPO_MODELO = dataTableReader["DS_GRUPOMODELO"].ToString(),
                            ID_GRUPO_MODELO = Convert.ToInt32(dataTableReader["ID_GRUPOMODELO"].ToString())
                        };

                        grupoModelos.Add(grupoModelo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.GrupoModelo> iGrupoModelos = grupoModelos;
            return View(iGrupoModelos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.GrupoModelo grupoModelo = new Models.GrupoModelo();
            return View(grupoModelo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.GrupoModelo grupoModelo)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();

                    if (grupoModelo.ID_GRUPO_MODELO > 0)
                    {
                        //verifica se já existe esse registro no banco
                        grupoModeloEntity.ID_GRUPO_MODELO = grupoModelo.ID_GRUPO_MODELO;
                        DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }
                    }

                    grupoModeloEntity.ID_GRUPO_MODELO = grupoModelo.ID_GRUPO_MODELO;
                    grupoModeloEntity.CD_GRUPO_MODELO = grupoModelo.CD_GRUPO_MODELO;
                    grupoModeloEntity.DS_GRUPO_MODELO = grupoModelo.DS_GRUPO_MODELO;

                    new GrupoModeloData().Inserir(ref grupoModeloEntity);

                    grupoModelo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                    
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(grupoModelo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.GrupoModelo grupoModelo = null;

            try
            {
                GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();

                grupoModeloEntity.ID_GRUPO_MODELO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        grupoModelo = new Models.GrupoModelo
                        {
                            ID_GRUPO_MODELO = Convert.ToInt32(dataTableReader["ID_GRUPOMODELO"].ToString()),
                            CD_GRUPO_MODELO = dataTableReader["CD_GRUPOMODELO"].ToString(),
                            DS_GRUPO_MODELO = dataTableReader["DS_GRUPOMODELO"].ToString(),
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

            if (grupoModelo == null)
                return HttpNotFound();
            else
                return View(grupoModelo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.GrupoModelo grupoModelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();

                    grupoModeloEntity.ID_GRUPO_MODELO = grupoModelo.ID_GRUPO_MODELO;
                    grupoModeloEntity.CD_GRUPO_MODELO = grupoModelo.CD_GRUPO_MODELO;
                    grupoModeloEntity.DS_GRUPO_MODELO = grupoModelo.DS_GRUPO_MODELO;

                    new GrupoModeloData().Alterar(grupoModeloEntity);

                    grupoModelo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            
            return View(grupoModelo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.GrupoModelo grupoModelo = null;

            try
            {
                GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();

                grupoModeloEntity.ID_GRUPO_MODELO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        grupoModelo = new Models.GrupoModelo
                        {
                            ID_GRUPO_MODELO = Convert.ToInt32(dataTableReader["ID_GRUPOMODELO"].ToString()),
                            CD_GRUPO_MODELO = dataTableReader["CD_GRUPOMODELO"].ToString(),
                            DS_GRUPO_MODELO = dataTableReader["DS_GRUPOMODELO"].ToString(),
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

            if (grupoModelo == null)
                return HttpNotFound();
            else
                return View(grupoModelo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.GrupoModelo grupoModelo = new Models.GrupoModelo();
            try
            {
                if (ModelState.IsValid)
                {
                    GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();

                    grupoModeloEntity.ID_GRUPO_MODELO = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                    //DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                    //if (dataTableReader.HasRows)
                    //{
                    //    if (dataTableReader.Read())
                    //    {
                    //        grupoModelo = new Models.GrupoModelo
                    //        {
                    //            ID_GRUPO_MODELO = Convert.ToInt32(dataTableReader["ID_GRUPOMODELO"].ToString()),
                    //            CD_GRUPO_MODELO = dataTableReader["CD_GRUPOMODELO"].ToString(),
                    //            //DS_GRUPO_MODELO = dataTableReader["DS_GRUPOMODELO"].ToString()
                    //        };
                    //    }
                    //}

                    //grupoModeloEntity.CD_GRUPO_MODELO = grupoModelo.CD_GRUPO_MODELO;
                    //grupoModeloEntity.ID_GRUPO_MODELO = grupoModelo.ID_GRUPO_MODELO;
                    grupoModeloEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new GrupoModeloData().Excluir(grupoModeloEntity);

                    grupoModelo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(grupoModelo);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        //public ActionResult VerificarCodigo(string CD_MODELO, bool CancelarVerificarCodigo)
        //{
        //    bool Liberado = true;
        //    try
        //    {
        //        if (CancelarVerificarCodigo == false)
        //        {
        //            ModeloEntity modeloEntity = new ModeloEntity();

        //            modeloEntity.CD_MODELO = CD_MODELO;
        //            DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

        //            if (dataTableReader.HasRows)
        //            {
        //                if (dataTableReader.Read())
        //                    Liberado = false;
        //            }

        //            if (dataTableReader != null)
        //            {
        //                dataTableReader.Dispose();
        //                dataTableReader = null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    return Json(Liberado, JsonRequestBehavior.AllowGet);
        //}

    }
}
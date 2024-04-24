using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;
using System.Data;

namespace _3M.Comodato.Front.Controllers
{
    public class UsuarioXWFGrupoController : BaseController
    {

        // GET: UsuarioXWFGrupo
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.WfGrupoUsu usuarioGrupo = new Models.WfGrupoUsu
            {
                usuarios = new List<Models.Usuario>(),
                gruposwf = new List<Models.WfGrupo>(),
                Prioridade = ControlesUtility.Dicionarios.Prioridade()
            };

            //WFGrupoData wfg = new WFGrupoData();
            //DataTableReader dtr = wfg.ObterLista().CreateDataReader();
            //List<Models.WFGrupo> grupos = new List<Models.WFGrupo>();
            //if (dtr.HasRows)
            //{
            //    while (dtr.Read())
            //    {
            //        Models.WFGrupo grupo = new Models.WFGrupo
            //        {
            //            ID_GRUPOWF = Convert.ToInt32(dtr["ID_GRUPOWF"]),
            //            CD_GRUPOWF = dtr["CD_GRUPOWF"].ToString(),
            //            DS_GRUPOWF = dtr["DS_GRUPOWF"].ToString(),
            //            TP_GRUPOWF = dtr["TP_GRUPOWF"].ToString()
            //        };

            //        grupos.Add(grupo);
            //    }

            //    usuarioGrupo.gruposwf = grupos;
            //}

            return View(usuarioGrupo);
        }

        protected DataTable ObterListaUsuarioXWFGrupo(WfGrupoUsuEntity usuarioXWFGrupoEntity)
        {
            DataTable dataTable;

            try
            {
                
                dataTable = new WfGrupoUsuData().ObterLista(usuarioXWFGrupoEntity);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return dataTable;
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.WfGrupoUsu usuarioXWFGrupo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    WfGrupoUsuEntity usuarioXWFGrupoEntity = new WfGrupoUsuEntity();

                    usuarioXWFGrupoEntity.ID_GRUPOWF_USU = usuarioXWFGrupo.ID_GRUPOWF_USU;
                    usuarioXWFGrupoEntity.usuario.nidUsuario = usuarioXWFGrupo.usuario.nidUsuario;
                    usuarioXWFGrupoEntity.grupoWf.ID_GRUPOWF = usuarioXWFGrupo.wfGrupo.ID_GRUPOWF;
                    usuarioXWFGrupoEntity.NM_PRIORIDADE = usuarioXWFGrupo.NM_PRIORIDADE;
                    usuarioXWFGrupoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new WfGrupoUsuData().Inserir(ref usuarioXWFGrupoEntity);

                    usuarioXWFGrupo.JavaScriptToRun = "MensagemSucesso()";
                }
                usuarioXWFGrupo.usuarios = ObterListaUsuario();
                usuarioXWFGrupo.gruposwf = ObterListaWFGrupo();
                usuarioXWFGrupo.Prioridade = ControlesUtility.Dicionarios.Prioridade();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioXWFGrupo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public JsonResult ObterListaUsuarioXWFGrupoJson()
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaUsuarioXWFGrupo> ListaUsuarioXWFGrupo = ObterListaUsuarioXWFGrupo();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/UsuarioXWFGrupo/_gridMVC.cshtml", ListaUsuarioXWFGrupo));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected List<Models.ListaUsuarioXWFGrupo> ObterListaUsuarioXWFGrupo()
        {
            List<Models.ListaUsuarioXWFGrupo> ListaUsuarioXWFGrupo = new List<Models.ListaUsuarioXWFGrupo>();

            try
            {
                WfGrupoUsuEntity usuarioXWFGrupoEntity = new WfGrupoUsuEntity();
                DataTableReader dataTableReader = new WfGrupoUsuData().ObterLista(usuarioXWFGrupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaUsuarioXWFGrupo listaUsuarioXWFGrupo = new Models.ListaUsuarioXWFGrupo();

                        listaUsuarioXWFGrupo.ID_GRUPOWF_USU = Convert.ToInt64(dataTableReader["ID_GRUPOWF_USU"]);
                        listaUsuarioXWFGrupo.ID_USUARIO = Convert.ToInt64(dataTableReader["ID_USUARIO"]);
                        listaUsuarioXWFGrupo.cnmNome = dataTableReader["cnmNome"].ToString();
                        listaUsuarioXWFGrupo.CD_GRUPOWF = dataTableReader["CD_GRUPOWF"].ToString();
                        listaUsuarioXWFGrupo.NM_PRIORIDADE = Convert.ToInt32(dataTableReader["NM_PRIORIDADE"]);


                        ListaUsuarioXWFGrupo.Add(listaUsuarioXWFGrupo);
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

            return ListaUsuarioXWFGrupo;
        }
    }
}
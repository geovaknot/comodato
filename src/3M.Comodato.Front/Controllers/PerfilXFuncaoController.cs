using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.IO;

namespace _3M.Comodato.Front.Controllers
{
    public class PerfilXFuncaoController : BaseController
    {
        // GET: PerfilXFuncao
        [_3MAuthentication]
        public ActionResult Editar()
        {
            Models.PerfilXFuncao perfilXFuncao = new Models.PerfilXFuncao
            {
                perfis = ObterListaPerfil(),
                funcoes = ObterListaFuncao()
            };

            return View(perfilXFuncao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.PerfilXFuncao perfilXFuncao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PerfilFuncaoEntity perfilXfuncaoEntity = new PerfilFuncaoEntity();

                    perfilXfuncaoEntity.funcao.nidFuncao = perfilXFuncao.funcao.nidFuncao;
                    perfilXfuncaoEntity.perfil.nidPerfil = perfilXFuncao.perfil.nidPerfil;
                    perfilXfuncaoEntity.bidAtivo = true;
                    perfilXfuncaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new PerfilFuncaoData().Inserir(ref perfilXfuncaoEntity);

                    perfilXFuncao.JavaScriptToRun = "MensagemSucesso()";
                }
                perfilXFuncao.perfis = ObterListaPerfil();
                perfilXFuncao.funcoes = ObterListaFuncao();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(perfilXFuncao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public JsonResult ObterListaPerfilXFuncaoJson(Int64 nidPerfil)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaPerfilXFuncao> listaPerfilXFuncao = ObterListaPerfilXFuncao(nidPerfil);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/PerfilXFuncao/_gridMVC.cshtml", listaPerfilXFuncao));
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

        public JsonResult ExcluirPerfilXFuncaoJson(Int64 nidPerfilFuncao)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                PerfilFuncaoEntity perfilFuncaoEntity = new PerfilFuncaoEntity();

                perfilFuncaoEntity.nidPerfilFuncao = nidPerfilFuncao;
                perfilFuncaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                new PerfilFuncaoData().Excluir(perfilFuncaoEntity);

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

        protected List<Models.ListaPerfilXFuncao> ObterListaPerfilXFuncao(Int64 nidPerfil)
        {
            List<Models.ListaPerfilXFuncao> listaPerfilXFuncao = new List<Models.ListaPerfilXFuncao>();

            try
            {
                PerfilFuncaoEntity perfilFuncaoEntity = new PerfilFuncaoEntity();
                perfilFuncaoEntity.perfil.nidPerfil = nidPerfil;
                DataTableReader dataTableReader = new PerfilFuncaoData().ObterLista(perfilFuncaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaPerfilXFuncao perfilfuncao = new Models.ListaPerfilXFuncao
                        {
                            nidPerfilFuncao = Convert.ToInt64(dataTableReader["nidPerfilFuncao"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidPerfilFuncao"].ToString()),
                            nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]),
                            bidAtivoPerfilFuncao = Convert.ToBoolean(dataTableReader["bidAtivoPerfilFuncao"]),
                            nidFuncao = Convert.ToInt64(dataTableReader["nidFuncao"]),
                            ccdFuncao = dataTableReader["ccdFuncao"].ToString(),
                            cdsFuncao = dataTableReader["cdsFuncao"].ToString()
                        };
                        listaPerfilXFuncao.Add(perfilfuncao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return listaPerfilXFuncao;
        }
    }
}
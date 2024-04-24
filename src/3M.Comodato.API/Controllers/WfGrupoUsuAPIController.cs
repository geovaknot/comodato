using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/WfGrupoUsuAPI")]
    [Authorize]
    public class WfGrupoUsuAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(WfGrupoUsuEntity wfGrupoUsuEntity)
        {
            try
            {
                new WfGrupoUsuData().Inserir(ref wfGrupoUsuEntity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot insert duplicate key"))
                    return Request.CreateResponse(HttpStatusCode.OK);

                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(WfGrupoUsuEntity wfGrupoUsuEntity)
        {
            try
            {
                new WfGrupoUsuData().Excluir(wfGrupoUsuEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("ObterLista")]
        [HttpGet, HttpPost]
        public IHttpActionResult ObterLista(WfGrupoUsuEntity wfGrupoUsuEntity)
        {
            List<WfGrupoUsuEntity> listaGrupos = new List<WfGrupoUsuEntity>();

            try
            {
                WfGrupoUsuEntity grupoEntity = null;

                DataTableReader dataTableReader = new WfGrupoUsuData().ObterLista(wfGrupoUsuEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        grupoEntity = new WfGrupoUsuEntity();
                        grupoEntity.ID_GRUPOWF_USU = Convert.ToInt64(dataTableReader["ID_GRUPOWF_USU"]);
                        grupoEntity.usuario.nidUsuario = Convert.ToInt64(dataTableReader["ID_USUARIO"]);
                        grupoEntity.usuario.cnmNome = dataTableReader["cnmNome"].ToString();
                        grupoEntity.grupoWf.ID_GRUPOWF = Convert.ToInt32(dataTableReader["ID_GRUPOWF"]);
                        grupoEntity.grupoWf.CD_GRUPOWF = dataTableReader["CD_GRUPOWF"].ToString();
                        grupoEntity.grupoWf.DS_GRUPOWF = dataTableReader["DS_GRUPOWF"].ToString();
                        grupoEntity.grupoWf.TP_GRUPOWF = dataTableReader["TP_GRUPOWF"].ToString();
                        grupoEntity.NM_PRIORIDADE = Convert.ToInt32(dataTableReader["NM_PRIORIDADE"]);

                        listaGrupos.Add(grupoEntity);
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("grupos", JsonConvert.SerializeObject(listaGrupos, Formatting.None));

            return Ok(jObject);
        }

    }
}
using _3M.Comodato.Data;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/WfGrupoAPI")]
    [Authorize]
    public class WfGrupoAPIController : BaseAPIController
    {
        [Route("ObterLista")]
        [HttpGet, HttpPost]
        public IHttpActionResult ObterLista()
        {
            List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

            try
            {
                WfGrupoEntity grupoEntity = new WfGrupoEntity();

                DataTableReader dataTableReader = new WfGrupoData().ObterLista(grupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        grupoEntity = new WfGrupoEntity();
                        grupoEntity.ID_GRUPOWF = Convert.ToInt32(dataTableReader["ID_GRUPOWF"]);
                        grupoEntity.CD_GRUPOWF = dataTableReader["CD_GRUPOWF"].ToString();
                        grupoEntity.DS_GRUPOWF = dataTableReader["DS_GRUPOWF"].ToString();
                        grupoEntity.TP_GRUPOWF = dataTableReader["TP_GRUPOWF"].ToString();

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

        [HttpGet]
        [Route("ObterDados")]
        public HttpResponseMessage ObterDados(int idGrupoWF)
        {
            List<WfGrupoEntity> grupo = new List<WfGrupoEntity>();

            WfGrupoEntity WFGrupoEntity = new WfGrupoEntity();
            WFGrupoEntity.ID_GRUPOWF = idGrupoWF;

            try
            {
                DataTableReader dataTableReader = new WfGrupoData().ObterLista(WFGrupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        WFGrupoEntity = new WfGrupoEntity();

                        CarregarGrupoWFEntity(dataTableReader, WFGrupoEntity);

                        grupo.Add(WFGrupoEntity);
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { grupos = grupo });
        }

        [Route("ObterListaByStatusPedido")]
        [HttpGet, HttpPost]
        public IHttpActionResult ObterListaByStatusPedido(int ST_STATUS_PEDIDO)
        {
            List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

            try
            {
                WfGrupoEntity grupoEntity = new WfGrupoEntity();

                DataTableReader dataTableReader = new WfGrupoData().ObterListaByStatusPedido(ST_STATUS_PEDIDO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        grupoEntity = new WfGrupoEntity();
                        grupoEntity.ID_GRUPOWF = Convert.ToInt32(dataTableReader["ID_GRUPOWF"]);
                        grupoEntity.CD_GRUPOWF = dataTableReader["CD_GRUPOWF"].ToString();
                        grupoEntity.DS_GRUPOWF = dataTableReader["DS_GRUPOWF"].ToString();
                        grupoEntity.TP_GRUPOWF = dataTableReader["TP_GRUPOWF"].ToString();

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

        [Route("ObterListaByStatusPedidoEnvio")]
        [HttpGet, HttpPost]
        public IHttpActionResult ObterListaByStatusPedidoEnvio(int ST_STATUS_PEDIDO, int CATEGORIA, string TIPOLOCACAO, string LINHA, string MODELO)
        {
            if (TIPOLOCACAO == null)
                TIPOLOCACAO = "";
            if (LINHA == null)
                LINHA = "";
            if (MODELO == null)
                MODELO = "";

            List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

            try
            {
                Utility.WorkflowUtility wfUtil = new WorkflowUtility();
                listaGrupos = wfUtil.IdentificaGrupo(ST_STATUS_PEDIDO, CATEGORIA, TIPOLOCACAO, LINHA, MODELO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("grupos", JsonConvert.SerializeObject(listaGrupos, Formatting.None));

            return Ok(jObject);
        }

        protected void CarregarGrupoWFEntity(DataTableReader dataTableReader, WfGrupoEntity WFGrupoEntity)
        {
            WFGrupoEntity.ID_GRUPOWF = Convert.ToInt32(dataTableReader["ID_GRUPOWF"].ToString());
            WFGrupoEntity.CD_GRUPOWF = Convert.ToString(dataTableReader["CD_GRUPOWF"]);
            WFGrupoEntity.DS_GRUPOWF = Convert.ToString(dataTableReader["DS_GRUPOWF"]);
            WFGrupoEntity.TP_GRUPOWF = Convert.ToString(dataTableReader["TP_GRUPOWF"]);
        }
    }
}
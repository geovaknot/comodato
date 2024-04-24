using _3M.Comodato.Data;
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
    [RoutePrefix("api/RRStatusAPI")]
    [Authorize]
    public class RRStatusAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterListaStatusRR")]
        public IHttpActionResult ObterListaStatusRR()
        {

            RRStatusEntity entity = new RRStatusEntity();
            List<RRStatusEntity> listaRRStatus = new List<RRStatusEntity>();
            try
            {
                DataTableReader dataTableReader = new RRStatusData().ObterLista(entity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        RRStatusEntity rStatusEntity = new RRStatusEntity
                        {
                            ST_STATUS_RR = Convert.ToInt32(dataTableReader["ST_STATUS_RR"].ToString()),
                            DS_STATUS_NOME = dataTableReader["DS_STATUS_NOME"].ToString(),
                           
                        };

                        listaRRStatus.Add(rStatusEntity);
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
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);
            }
            JObject JO = new JObject();
            JO.Add("listaRRStatus", JsonConvert.SerializeObject(listaRRStatus, Formatting.None));
            return Ok(JO);
        }

        /// <summary>
        /// Consulta de Tipos de Status de Pedido conforme lista recebida em statusCarregar
        /// </summary>
        /// <param name="statusCarregar"></param>
        /// <param name="TP_PEDIDO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ObterListaStatus")]
        public HttpResponseMessage ObterListaStatus(string statusCarregar,string ID_GRUPOWF)
        {
            List<RRStatusEntity> tiposStatusRR = new List<RRStatusEntity>();
            
            try
            {
                RRStatusEntity tpStatusRR = new RRStatusEntity();

                DataTableReader dataTableReader = new RRStatusData().ObterListaStatus(statusCarregar,Convert.ToInt32(ID_GRUPOWF)).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tpStatusRR = new RRStatusEntity();
                        tpStatusRR.ID_RR_STATUS = Convert.ToInt64(dataTableReader["ID_RR_STATUS"]);
                        tpStatusRR.ST_STATUS_RR = Convert.ToInt32(dataTableReader["ST_STATUS_RR"]);
                        tpStatusRR.DS_STATUS_NOME_REDUZ = dataTableReader["DS_STATUS_NOME_REDUZ"].ToString();
                        tpStatusRR.DS_STATUS_NOME = dataTableReader["DS_STATUS_NOME"].ToString();
                        tpStatusRR.DS_STATUS_DESCRICAO = dataTableReader["DS_STATUS_DESCRICAO"].ToString();
                   
                        tpStatusRR.DS_TRANSICAO = dataTableReader["DS_TRANSICAO"].ToString();
                        tpStatusRR.DS_COMENTARIO = dataTableReader["DS_COMENTARIO"].ToString();
                        tpStatusRR.NR_ORDEM_FLUXO = Convert.ToInt32(dataTableReader["NR_ORDEM_FLUXO"]);
                        
                        tiposStatusRR.Add(tpStatusRR);
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
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { tiposStatusRR = tiposStatusRR });
        }

    }
}

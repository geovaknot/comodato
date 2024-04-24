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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/RegiaoAPI")]
    [Authorize]
    public class RegiaoAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de Regiões
        /// </summary>
        /// <param name="regiaoEntity"></param>
        /// <returns>List<RegiaoEntity> regioes</returns>
        //[HttpPost]
        [HttpGet]
        [Route("ObterLista")]
        //public IHttpActionResult ObterLista(RegiaoEntity regiaoEntity)
        public HttpResponseMessage ObterLista(RegiaoEntity regiaoEntity)        
        {
            List<RegiaoEntity> regioes = new List<RegiaoEntity>();
            try
            {
                if (regiaoEntity == null)
                    regiaoEntity = new RegiaoEntity();

                DataTableReader dataTableReader = new RegiaoData().ObterLista(regiaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        regiaoEntity = new RegiaoEntity();
                        regiaoEntity.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();
                        regiaoEntity.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();
                        regioes.Add(regiaoEntity);
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
                //return BadRequest(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            //JObject JO = new JObject();
            //JO.Add("REGIAO", JsonConvert.SerializeObject(regioes, Formatting.None));
            //return Ok(JO);
            return Request.CreateResponse(HttpStatusCode.OK, new { regioes = regioes });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para RegiaoEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="regiaoEntity"></param>
        protected void CarregarRegiaoEntity(DataTableReader dataTableReader, RegiaoEntity regiaoEntity)
        {
            regiaoEntity.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();
            regiaoEntity.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();
        }

        [HttpGet]
        [Route("ObterListaRegiaoSinc")]
        public IHttpActionResult ObterListaRegiaoSinc(RegiaoEntity regiaoEntity)
        {
            List<RegiaoEntity> regioes = new List<RegiaoEntity>();
            try
            {
                if (regiaoEntity == null)
                    regiaoEntity = new RegiaoEntity();

                DataTableReader dataTableReader = new RegiaoData().ObterLista(regiaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        regiaoEntity = new RegiaoEntity();
                        regiaoEntity.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();
                        regiaoEntity.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();
                        regioes.Add(regiaoEntity);
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

            JObject JO = new JObject();
            //JO.Add("REGIAO", JsonConvert.SerializeObject(regioes, Formatting.None));
            JO.Add("REGIAO", JArray.FromObject(regioes));
            return Ok(JO);
        }


    }
}
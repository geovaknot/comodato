using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/ExecutivoAPI")]
    [Authorize]
    public class ExecutivoAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de executivos
        /// </summary>
        /// <param name="executivoEntity"></param>
        /// <returns>List<ExecutivoEntity>executivos</returns>
        [HttpGet]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(ExecutivoEntity executivoEntity)
        {
            List<ExecutivoEntity> executivos = new List<ExecutivoEntity>();

            try
            {
                if (executivoEntity == null)
                    executivoEntity = new ExecutivoEntity();

                DataTableReader dataTableReader = new ExecutivoData().ObterLista(executivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        executivoEntity = new ExecutivoEntity();
                        CarregarExecutivoEntity(dataTableReader, executivoEntity);
                        executivos.Add(executivoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { executivos = executivos });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para ExecutivoEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="executivoEntity"></param>
        protected void CarregarExecutivoEntity(DataTableReader dataTableReader, ExecutivoEntity executivoEntity)
        {
            executivoEntity.CD_EXECUTIVO = Convert.ToInt64(dataTableReader["CD_EXECUTIVO"]);
            executivoEntity.NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString();
        }
    }

    
}
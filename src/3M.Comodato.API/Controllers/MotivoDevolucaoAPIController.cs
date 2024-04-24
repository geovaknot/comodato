using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
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
    [RoutePrefix("api/MotivoDevolucaoAPI")]
    [Authorize]
    public class MotivoDevolucaoAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de motivos
        /// </summary>
        /// <param name="motivoDevolucaoEntity"></param>
        /// <returns>List<GrupoEntity> grupos</returns>
        [HttpGet]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(MotivoDevolucaoEntity motivoEntity)
        {
            List<MotivoDevolucaoEntity> motivos = new List<MotivoDevolucaoEntity>();

            try
            {
                if (motivoEntity == null)
                    motivoEntity = new MotivoDevolucaoEntity();

                DataTableReader dataTableReader = new MotivoDevolucaoData().ObterLista(motivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        motivoEntity = new MotivoDevolucaoEntity();
                        CarregarMotivoEntity(dataTableReader, motivoEntity);
                        motivos.Add(motivoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { motivos = motivos });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para MotivoEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="motivoEntity"></param>
        protected void CarregarMotivoEntity(DataTableReader dataTableReader, MotivoDevolucaoEntity motivoEntity)
        {
            motivoEntity.CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString();
            motivoEntity.DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString();
        }
    }

    
}
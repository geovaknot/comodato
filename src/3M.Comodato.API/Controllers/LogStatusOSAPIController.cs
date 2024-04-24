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
    [RoutePrefix("api/LogStatusOSAPI")]
    [Authorize]
    public class LogStatusOSAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(Models.LogStatusOS logStatusOS)
        {
            try
            {
                LogStatusOSEntity logStatusOSEntity = new LogStatusOSEntity();

                logStatusOSEntity.ID_LOG_STATUS_OS = Convert.ToInt64(logStatusOS.ID_LOG_STATUS_OS);
                logStatusOSEntity.OS.ID_OS = Convert.ToInt64(logStatusOS.OS.ID_OS);
                logStatusOSEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
                logStatusOSEntity.nidUsuarioAtualizacao = Convert.ToInt64(logStatusOS.nidUsuarioAtualizacao);

                try
                {
                    logStatusOSEntity.DT_DATA_LOG_OS = Convert.ToDateTime(logStatusOS.DT_DATA_LOG_OS_Formatado + " " + logStatusOS.DT_DATA_LOG_OS_HORA_Formatado);
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MENSAGEM = "Data/Hora inválida ou não informada!" });
                }

                new LogStatusOSData().Alterar(logStatusOSEntity);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Consulta o LOG
        /// </summary>
        /// <param name="ID_LOG_STATUS_OS"></param>
        /// <returns>Models.LogStatusOS logStatusOS</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID_LOG_STATUS_OS)
        {
            Models.LogStatusOS logStatusOS = new Models.LogStatusOS();
            LogStatusOSEntity logStatusOSEntity = new LogStatusOSEntity();

            try
            {
                logStatusOSEntity.ID_LOG_STATUS_OS = ID_LOG_STATUS_OS;
                DataTableReader dataTableReader = new LogStatusOSData().ObterLista(logStatusOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        CarregarlogStatusOS(dataTableReader, logStatusOS);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { logStatusOS = logStatusOS });
        }

        protected void CarregarlogStatusOS(DataTableReader dataTableReader, Models.LogStatusOS logStatusOS)
        {
            logStatusOS.ID_LOG_STATUS_OS = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_OS"]);
            logStatusOS.OS.ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]);
            logStatusOS.DT_DATA_LOG_OS = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]);
            logStatusOS.DT_DATA_LOG_OS_Formatado = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]).ToString("dd/MM/yyyy");
            logStatusOS.DT_DATA_LOG_OS_HORA_Formatado = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]).ToString("HH:mm:ss");
            logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            logStatusOS.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
        }


        [HttpGet]
        [Route("ObterListaLogStatusOsSinc")]
        public IHttpActionResult ObterListaLogStatusOsSinc(Int64 idUsuario)
        {
            IList<LogStatusOSSinc> listaLogStatusOs = new List<LogStatusOSSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                LogStatusOSData logStatusOSData = new LogStatusOSData();
                listaLogStatusOs = logStatusOSData.ObterListaLogStatusOsSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("LOG_STATUS_OS", JsonConvert.SerializeObject(listaLogStatusOs, Formatting.None));
                JO.Add("LOG_STATUS_OS", JArray.FromObject(listaLogStatusOs));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

    }
}
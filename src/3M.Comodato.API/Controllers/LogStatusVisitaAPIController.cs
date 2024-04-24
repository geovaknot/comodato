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
    [RoutePrefix("api/LogStatusVisitaAPI")]
    [Authorize]
    public class LogStatusVisitaAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(Models.LogStatusVisita logStatusVisita)
        {
            try
            {
                LogStatusVisitaEntity logStatusVisitaEntity = new LogStatusVisitaEntity();

                logStatusVisitaEntity.ID_LOG_STATUS_VISITA = Convert.ToInt64(logStatusVisita.ID_LOG_STATUS_VISITA);
                logStatusVisitaEntity.visita.ID_VISITA = Convert.ToInt64(logStatusVisita.visita.ID_VISITA);
                logStatusVisitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
                logStatusVisitaEntity.nidUsuarioAtualizacao = Convert.ToInt64(logStatusVisita.nidUsuarioAtualizacao);

                try
                {
                    logStatusVisitaEntity.DT_DATA_LOG_VISITA = Convert.ToDateTime(logStatusVisita.DT_DATA_LOG_VISITA_Formatado + " " + logStatusVisita.DT_DATA_LOG_VISITA_HORA_Formatado);
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MENSAGEM = "Data/Hora inválida ou não informada!" });
                }

                new LogStatusVisitaData().Alterar(logStatusVisitaEntity);
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
        /// <param name="ID_LOG_STATUS_VISITA"></param>
        /// <returns>Models.LogStatusVisita logStatusVisita</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID_LOG_STATUS_VISITA)
        {
            Models.LogStatusVisita logStatusVisita = new Models.LogStatusVisita();
            LogStatusVisitaEntity logStatusVisitaEntity = new LogStatusVisitaEntity();

            try
            {
                logStatusVisitaEntity.ID_LOG_STATUS_VISITA = ID_LOG_STATUS_VISITA;
                DataTableReader dataTableReader = new LogStatusVisitaData().ObterLista(logStatusVisitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        CarregarLogStatusVisita(dataTableReader, logStatusVisita);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { logStatusVisita = logStatusVisita });
        }

        protected void CarregarLogStatusVisita(DataTableReader dataTableReader, Models.LogStatusVisita logStatusVisita)
        {
            logStatusVisita.ID_LOG_STATUS_VISITA = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_VISITA"]);
            logStatusVisita.visita.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
            logStatusVisita.DT_DATA_LOG_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]);
            logStatusVisita.DT_DATA_LOG_VISITA_Formatado = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]).ToString("dd/MM/yyyy");
            logStatusVisita.DT_DATA_LOG_VISITA_HORA_Formatado = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]).ToString("HH:mm:ss");
            logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS= Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            logStatusVisita.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
        }


        [HttpGet]
        [Route("ObterListaLogStatusVisitaSinc")]
        public IHttpActionResult ObterListaLogStatusVisitaSinc(Int64 idUsuario)
        {
            IList<LogStatusVisitaSinc> listaLogStatusVisita = new List<LogStatusVisitaSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                LogStatusVisitaData logStatusVisitaData = new LogStatusVisitaData();
                listaLogStatusVisita = logStatusVisitaData.ObterListaLogStatusVisitaSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("LOG_STATUS_VISITA", JsonConvert.SerializeObject(listaLogStatusVisita, Formatting.None));
                JO.Add("LOG_STATUS_VISITA", JArray.FromObject(listaLogStatusVisita));
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
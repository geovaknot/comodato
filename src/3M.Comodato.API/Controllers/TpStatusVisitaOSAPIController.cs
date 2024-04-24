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

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpStatusVisitaOSAPI")]
    [Authorize]
    public class TpStatusVisitaOSAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de Tipos de Status de Visita ou OS
        /// </summary>
        /// <param name="tpStatusVisitaOSEntity"></param>
        /// <returns>List<TpStatusVisitaOSEntity> tiposStatusVisitaOS</returns>
        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(TpStatusVisitaOSEntity tpStatusVisitaOSEntity)
        {
            List<TpStatusVisitaOSEntity> tiposStatusVisitaOS = new List<TpStatusVisitaOSEntity>();

            try
            {
                if (tpStatusVisitaOSEntity == null)
                    tpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();

                DataTableReader dataTableReader = new TpStatusVisitaOSData().ObterLista(tpStatusVisitaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();
                        tpStatusVisitaOSEntity.ID_TP_STATUS_VISITA_OS = Convert.ToInt64(dataTableReader["ID_TP_STATUS_VISITA_OS"]);
                        tpStatusVisitaOSEntity.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
                        tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
                        tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS_ACAO = dataTableReader["DS_TP_STATUS_VISITA_OS_ACAO"].ToString();
                        tpStatusVisitaOSEntity.FL_STATUS_OS = dataTableReader["FL_STATUS_OS"].ToString();
                        tiposStatusVisitaOS.Add(tpStatusVisitaOSEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tiposStatusVisitaOS = tiposStatusVisitaOS });
        }

        [HttpPost]
        [Route("ObterListaOsPadraoStatus")]
        public HttpResponseMessage ObterListaOsPadraoStatus(TpStatusOSPadraoEntity tpStatusVisitaOSEntity)
        {
            List<TpStatusOSPadraoEntity> tiposStatusVisitaOS = new List<TpStatusOSPadraoEntity>();

            try
            {
                if (tpStatusVisitaOSEntity == null)
                    tpStatusVisitaOSEntity = new TpStatusOSPadraoEntity();

                DataTableReader dataTableReader = new TpStatusOSPadraoData().ObterListaStatusOsPadrao(tpStatusVisitaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tpStatusVisitaOSEntity = new TpStatusOSPadraoEntity();
                        tpStatusVisitaOSEntity.ID_STATUS_OS = Convert.ToInt64(dataTableReader["ID_STATUS_OS"]);
                        tpStatusVisitaOSEntity.ST_STATUS_OS = Convert.ToInt32(dataTableReader["ST_STATUS_OS"]);
                        tpStatusVisitaOSEntity.DS_STATUS_OS = dataTableReader["DS_STATUS_OS"].ToString();
                        tiposStatusVisitaOS.Add(tpStatusVisitaOSEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tiposStatusVisitaOS = tiposStatusVisitaOS });
        }

        /// <summary>
        /// Consulta de Tipos de Status de Visita ou OS conforme lista recebida em statusCarregar
        /// </summary>
        /// <param name="statusCarregar"></param>
        /// <param name="FL_STATUS_OS"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ObterListaStatus")]
        public HttpResponseMessage ObterListaStatus(string statusCarregar, string FL_STATUS_OS)
        {
            List<TpStatusVisitaOSEntity> tiposStatusVisitaOS = new List<TpStatusVisitaOSEntity>();

            try
            {
                TpStatusVisitaOSEntity tpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();

                DataTableReader dataTableReader = new TpStatusVisitaOSData().ObterListaStatus(statusCarregar, FL_STATUS_OS).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tpStatusVisitaOSEntity = new TpStatusVisitaOSEntity();
                        tpStatusVisitaOSEntity.ID_TP_STATUS_VISITA_OS = Convert.ToInt64(dataTableReader["ID_TP_STATUS_VISITA_OS"]);
                        tpStatusVisitaOSEntity.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
                        tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
                        tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS_ACAO = dataTableReader["DS_TP_STATUS_VISITA_OS_ACAO"].ToString();
                        tpStatusVisitaOSEntity.FL_STATUS_OS = dataTableReader["FL_STATUS_OS"].ToString();
                        tiposStatusVisitaOS.Add(tpStatusVisitaOSEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tiposStatusVisitaOS = tiposStatusVisitaOS });
        }
    }
}
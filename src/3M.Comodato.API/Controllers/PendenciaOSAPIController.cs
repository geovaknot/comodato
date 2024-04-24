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
    [RoutePrefix("api/PendenciaOSAPI")]
    [Authorize]
    public class PendenciaOSAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(PendenciaOSEntity pendenciaOSEntity)
        {
            if (pendenciaOSEntity.TOKEN <= 0)
                throw new Exception($"Token para integração de registro de pendência da OS {pendenciaOSEntity.OS.ID_OS} não informado.");

            try
            {
                if (!string.IsNullOrEmpty(pendenciaOSEntity.DT_ABERTURA_Formatado) && pendenciaOSEntity.DT_ABERTURA == null)
                    pendenciaOSEntity.DT_ABERTURA = Convert.ToDateTime(pendenciaOSEntity.DT_ABERTURA_Formatado + " " + DateTime.Now.ToString("HH:mm:ss"));

                if (!string.IsNullOrEmpty(pendenciaOSEntity.QT_PECA_Formatado) && pendenciaOSEntity.QT_PECA == null)
                    pendenciaOSEntity.QT_PECA = Convert.ToDecimal(pendenciaOSEntity.QT_PECA_Formatado);

                new PendenciaOSData().Inserir(ref pendenciaOSEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pendenciaOSEntity.ID_PENDENCIA_OS, pendenciaOSEntity.TOKEN });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(PendenciaOSEntity pendenciaOSEntity)
        {
            try
            {
                if (!string.IsNullOrEmpty(pendenciaOSEntity.DT_ABERTURA_Formatado) && pendenciaOSEntity.DT_ABERTURA == null)
                    pendenciaOSEntity.DT_ABERTURA = Convert.ToDateTime(pendenciaOSEntity.DT_ABERTURA_Formatado + " " + DateTime.Now.ToString("HH:mm:ss"));

                if (!string.IsNullOrEmpty(pendenciaOSEntity.QT_PECA_Formatado) && pendenciaOSEntity.QT_PECA == null)
                    pendenciaOSEntity.QT_PECA = Convert.ToDecimal(pendenciaOSEntity.QT_PECA_Formatado);

                new PendenciaOSData().Alterar(pendenciaOSEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_PENDENCIA_OS = pendenciaOSEntity.ID_PENDENCIA_OS });
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(PendenciaOSEntity pendenciaOSEntity)
        {
            try
            {
                new PendenciaOSData().Excluir(pendenciaOSEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("Finalizar")]
        public HttpResponseMessage Finalizar(PendenciaOSEntity pendenciaOSEntity)
        {
            try
            {
                pendenciaOSEntity.ST_STATUS_PENDENCIA = ControlesUtility.Dicionarios.TipoStatusPendenciaOS().ToArray()[1].Value;

                new PendenciaOSData().AlterarStatus(pendenciaOSEntity);
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
        /// Consulta a pendência informado
        /// </summary>
        /// <param name="ID_PENDENCIA_OS"></param>
        /// <returns>PendenciaOSEntity pendenciaOSEntity</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID_PENDENCIA_OS)
        {
            PendenciaOSEntity pendenciaOSEntity = new PendenciaOSEntity();
            Models.PendenciaOS pendenciaOSModel = new Models.PendenciaOS();

            try
            {
                pendenciaOSEntity.ID_PENDENCIA_OS = ID_PENDENCIA_OS;
                DataTableReader dataTableReader = new PendenciaOSData().ObterLista(pendenciaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarPendenciaEntity(dataTableReader, pendenciaOSEntity);
                        CarregarPendenciaModel(dataTableReader, pendenciaOSModel);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { pendenciaOS = pendenciaOSEntity, pendenciaOSModel = pendenciaOSModel });
        }

        protected void CarregarPendenciaModel(DataTableReader dataTableReader, Models.PendenciaOS pendenciaOS)
        {
            pendenciaOS.ID_PENDENCIA_OS = Convert.ToInt64(dataTableReader["ID_PENDENCIA_OS"]);
            pendenciaOS.OS = new Models.OS()
            {
                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                ID_OS_Formatado = Convert.ToInt64(dataTableReader["ID_OS"]).ToString("000000"),
                DT_DATA_ABERTURA = Convert.ToDateTime(dataTableReader["DT_DATA_ABERTURA"]).ToString("dd/MM/yyyy"),
                tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                {
                    ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                    DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                }
            };
            pendenciaOS.DT_ABERTURA = Convert.ToDateTime(dataTableReader["DT_ABERTURA"]).ToString("dd/MM/yyyy");
            pendenciaOS.DS_DESCRICAO = dataTableReader["DS_DESCRICAO"].ToString();
            pendenciaOS.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
            pendenciaOS.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
            pendenciaOS.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();

            if (dataTableReader["QT_PECA"] != DBNull.Value)
            {
                if (pendenciaOS.peca.TX_UNIDADE == "MT")
                    pendenciaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3");
                else
                    pendenciaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N0");
            }

            pendenciaOS.ST_STATUS_PENDENCIA = dataTableReader["ST_STATUS_PENDENCIA"].ToString();
            pendenciaOS.ST_TP_PENDENCIA = dataTableReader["ST_TP_PENDENCIA"].ToString();
            pendenciaOS.CD_TP_ESTOQUE_CLI_TEC = dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString();
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para PendenciaOSEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="pendenciaOSEntity"></param>
        protected void CarregarPendenciaEntity(DataTableReader dataTableReader, PendenciaOSEntity pendenciaOSEntity)
        {
            pendenciaOSEntity.ID_PENDENCIA_OS = Convert.ToInt64(dataTableReader["ID_PENDENCIA_OS"]);
            pendenciaOSEntity.OS.ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]);
            pendenciaOSEntity.OS.DT_DATA_ABERTURA = Convert.ToDateTime(dataTableReader["DT_DATA_ABERTURA"]);
            pendenciaOSEntity.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            pendenciaOSEntity.OS.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
            pendenciaOSEntity.DT_ABERTURA = Convert.ToDateTime(dataTableReader["DT_ABERTURA"]);
            //pendenciaOSEntity.DT_ABERTURA_Formatado = Convert.ToDateTime(dataTableReader["DT_ABERTURA"]).ToString("dd/MM/yyyy");
            pendenciaOSEntity.DS_DESCRICAO = dataTableReader["DS_DESCRICAO"].ToString();
            pendenciaOSEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
            pendenciaOSEntity.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
            pendenciaOSEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            if (dataTableReader["QT_PECA"] != DBNull.Value)
                pendenciaOSEntity.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]);
            //pendenciaOSEntity.QT_PECA_Formatado = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3");
            pendenciaOSEntity.ST_STATUS_PENDENCIA = dataTableReader["ST_STATUS_PENDENCIA"].ToString();
            pendenciaOSEntity.ST_TP_PENDENCIA = dataTableReader["ST_TP_PENDENCIA"].ToString();
            pendenciaOSEntity.CD_TP_ESTOQUE_CLI_TEC = dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString();
        }


        [HttpGet]
        [Route("ObterListaPendenciaOsSinc")]
        public IHttpActionResult ObterListaPendenciaOsSinc(Int64 idUsuario, int bloquearDataRetroativa = 0)
        {
            IList<PendenciaOSSinc> listaPendenciaOs = new List<PendenciaOSSinc>();
            try
            {
                PendenciaOSData pendenciaOSData = new PendenciaOSData();
                listaPendenciaOs = pendenciaOSData.ObterListaPendenciaOsSinc(idUsuario);

                //DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-60);
                //listaPendenciaOs = listaPendenciaOs.Where(x => x.DT_DATA_OS >= dtInicio).ToList();

                //if (bloquearDataRetroativa == 1)
                //{
                //    DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //    if (DateTime.Now.Day < 6) dtInicio = dtInicio.AddMonths(-1); // Até o 5o dia do mês, retornar registros do mês anterior
                //    listaPendenciaOs = listaPendenciaOs.Where(x => x.DT_DATA_OS >= dtInicio).ToList();
                //}

                JObject JO = new JObject
                {
                    { "PENDENCIA", JArray.FromObject(listaPendenciaOs) }
                };

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
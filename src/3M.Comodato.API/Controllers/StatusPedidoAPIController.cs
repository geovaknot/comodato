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
    [RoutePrefix("api/StatusPedidoAPI")]
    [Authorize]
    public class StatusPedidoAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(StatusPedidoEntity statusPedidoEntity)
        {
            List<StatusPedidoEntity> statusPedidos = new List<StatusPedidoEntity>();

            try
            {
                if (statusPedidoEntity == null)
                    statusPedidoEntity = new StatusPedidoEntity();

                DataTableReader dataTableReader = new StatusPedidoData().ObterLista(statusPedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        statusPedidoEntity = new StatusPedidoEntity();

                        statusPedidoEntity.ID_STATUS_PEDIDO = Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]);
                        statusPedidoEntity.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_PEDIDO"].ToString();
                        statusPedidoEntity.DS_STATUS_PEDIDO_ACAO = dataTableReader["DS_STATUS_PEDIDO_ACAO"].ToString();
                        statusPedidos.Add(statusPedidoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { statusPedidos = statusPedidos });
        }

        /// <summary>
        /// Consulta de Tipos de Status de Pedidos conforme lista recebida em statusCarregar
        /// </summary>
        /// <param name="statusCarregar"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ObterListaStatus")]
        public HttpResponseMessage ObterListaStatus(string statusCarregar)
        {
            List<StatusPedidoEntity> tiposStatusPedidos = new List<StatusPedidoEntity>();

            try
            {
                StatusPedidoEntity statusPedidoEntity = new StatusPedidoEntity();

                DataTableReader dataTableReader = new StatusPedidoData().ObterListaStatus(statusCarregar).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        statusPedidoEntity = new StatusPedidoEntity();
                        statusPedidoEntity.ID_STATUS_PEDIDO = Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]);
                        statusPedidoEntity.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_PEDIDO"].ToString();
                        statusPedidoEntity.DS_STATUS_PEDIDO_ACAO = dataTableReader["DS_STATUS_PEDIDO_ACAO"].ToString();
                        tiposStatusPedidos.Add(statusPedidoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tiposStatusPedidos = tiposStatusPedidos });
        }

        /// <summary>
        /// Obter a listagem de Agenda de um tecnico (usuario)
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObterListaStatusPedidoSinc")]
        public IHttpActionResult ObterListaStatusPedidoSinc()
        {
            IList<StatusPedidoSincEntity> listaStatusPedido = new List<StatusPedidoSincEntity>();
            try
            {
                //Int32 idUsuario = 60237;
                StatusPedidoData statusPedidoData = new StatusPedidoData();
                listaStatusPedido = statusPedidoData.ObterListaStatusPedidoSinc();

                JObject JO = new JObject();
                //JO.Add("AGENDA", JsonConvert.SerializeObject(listaAgenda, Formatting.None));
                JO.Add("STATUS_PEDIDO", JArray.FromObject(listaStatusPedido));
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
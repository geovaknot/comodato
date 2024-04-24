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
    [RoutePrefix("api/WfStatusPedidoEquipAPI")]
    [Authorize]
    public class WfStatusPedidoEquipAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(string tpPedido)
        {
            try
            {
                WfStatusPedidoEquipEntity entity = new WfStatusPedidoEquipEntity();
                entity.TP_PEDIDO = tpPedido;

                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new WfStatusPedidoEquipData().ObterLista(entity), Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Consulta de Tipos de Status de Pedido conforme lista recebida em statusCarregar
        /// </summary>
        /// <param name="statusCarregar"></param>
        /// <param name="TP_PEDIDO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ObterListaStatus")]
        public HttpResponseMessage ObterListaStatus(string statusCarregar, string TP_PEDIDO)
        {
            List<WfStatusPedidoEquipEntity> tiposStatusPedido = new List<WfStatusPedidoEquipEntity>();

            try
            {
                WfStatusPedidoEquipEntity tpStatusPedido = new WfStatusPedidoEquipEntity();

                DataTableReader dataTableReader = new WfStatusPedidoEquipData().ObterListaStatus(statusCarregar, TP_PEDIDO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tpStatusPedido = new WfStatusPedidoEquipEntity();
                        tpStatusPedido.ID_TP_STATUS_PEDIDO_EQUIP = Convert.ToInt64(dataTableReader["ID_TP_STATUS_PEDIDO_EQUIP"]);
                        tpStatusPedido.ST_STATUS_PEDIDO = Convert.ToInt32(dataTableReader["ST_STATUS_PEDIDO"]);
                        tpStatusPedido.DS_STATUS_NOME_REDUZ = dataTableReader["DS_STATUS_NOME_REDUZ"].ToString();
                        tpStatusPedido.DS_STATUS_NOME = dataTableReader["DS_STATUS_NOME"].ToString();
                        tpStatusPedido.DS_STATUS_DESCRICAO = dataTableReader["DS_STATUS_DESCRICAO"].ToString();
                        tpStatusPedido.TP_PEDIDO = dataTableReader["TP_PEDIDO"].ToString();
                        tpStatusPedido.DS_TRANSICAO = dataTableReader["DS_TRANSICAO"].ToString();
                        tpStatusPedido.DS_COMENTARIO = dataTableReader["DS_COMENTARIO"].ToString();
                        tiposStatusPedido.Add(tpStatusPedido);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tiposStatusPedido = tiposStatusPedido });
        }

    }
}

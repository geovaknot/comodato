using _3M.Comodato.Data;
//using System.Web.Mvc;
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
    [RoutePrefix("api/WfPedidoComentAPI")]
    [Authorize]
    public class WfPedidoComentAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(WfPedidoComentEntity pedidoComentEntity)
        {
            try
            {
                pedidoComentEntity.DS_COMENT = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy hh:mm") + " - " + pedidoComentEntity.usuario.cnmNome + ": " + pedidoComentEntity.DS_COMENT;
                new WfPedidoComentData().Inserir(ref pedidoComentEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_WF_PEDIDO_COMENT = pedidoComentEntity.ID_WF_PEDIDO_COMENT });
        }
    }
}
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
    [RoutePrefix("api/RRComentAPI")]
    [Authorize]
    public class RRComentAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(RRComentEntity rrComentEntity)
        {
            try
            {
                rrComentEntity.DS_COMENT = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy hh:mm") + " - " + rrComentEntity.usuario.cnmNome + ": " + rrComentEntity.DS_COMENT;
                new RRComentData().Inserir(ref rrComentEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_WF_PEDIDO_COMENT = rrComentEntity.ID_RR_COMMENT });
        }


        [HttpGet]
        [Route("ObterListaRRComentSinc")]
        public IHttpActionResult ObterListaRRComentSinc(Int64? idUsuario)
        {
            IList<RRComentSincEntity> listaRRComent = new List<RRComentSincEntity>();
            try
            {
                RRComentData rrComentData = new RRComentData();
                listaRRComent = rrComentData.ObterListaRRComentSinc(idUsuario);

                JObject JO = new JObject();
                JO.Add("RRCOMENT", JArray.FromObject(listaRRComent));
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
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/StatusVisitaOSAPI")]
    [Authorize]
    public class StatusVisitaOSAPIController : BaseAPIController
    {

        [HttpGet]
        [Route("ObterListaStatusVisitaOSSinc")]
        public IHttpActionResult ObterListaStatusVisitaOSSinc()
        {
            IList<Entity.StatusVisitaOSEntity> listaStatusVisitaOS = new List<Entity.StatusVisitaOSEntity>();
            try
            {
                StatusVisitaOSData statusVisitaOSData = new StatusVisitaOSData();
                listaStatusVisitaOS = statusVisitaOSData.ObterListaStatusVisitaOS();

                JObject JO = new JObject();
                //JO.Add("TP_STATUS_VISITA", JsonConvert.SerializeObject(listaStatusVisitaOS, Formatting.None));
                JO.Add("TP_STATUS_VISITA", JArray.FromObject(listaStatusVisitaOS));
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

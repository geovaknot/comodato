using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpStatusVisitaPadraoAPI")]
    [Authorize]
    public class TpStatusVisitaPadraoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_STATUS_VISITA)
        {
            try
            {
                TpStatusVisitaPadraoEntity statusVisitaPadraoEntity = new TpStatusVisitaPadraoEntity();
                statusVisitaPadraoEntity.ID_STATUS_VISITA = ID_STATUS_VISITA;
                var listaVisitaPadrao = new TpStatusVisitaPadraoData().ObterLista(statusVisitaPadraoEntity);

                JObject JO = new JObject();
                JO.Add("STATUS_VISITA", JArray.FromObject(listaVisitaPadrao));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }
 
        [HttpGet]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista([FromUri] TpStatusVisitaPadraoEntity statusVisitaPadraoEntity)
        {
            try
            {
                IList<TpStatusVisitaPadraoEntity> listaStatusVisita = new List<TpStatusVisitaPadraoEntity>();
                listaStatusVisita = new TpStatusVisitaPadraoData().ObterLista(statusVisitaPadraoEntity);

                JObject JO = new JObject();
                JO.Add("STATUS_VISITA", JArray.FromObject(listaStatusVisita));
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
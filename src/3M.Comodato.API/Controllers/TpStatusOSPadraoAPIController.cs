using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpStatusOSPadraoAPI")]
    [Authorize]
    public class TpStatusOSPadraoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_STATUS_OS)
        {
            try
            {
                TpStatusOSPadraoEntity statusOSPadraoEntity = new TpStatusOSPadraoEntity();
                statusOSPadraoEntity.ID_STATUS_OS = ID_STATUS_OS;
                var listaOSPadrao = new TpStatusOSPadraoData().ObterLista(statusOSPadraoEntity);

                JObject JO = new JObject();
                JO.Add("STATUS_OS", JArray.FromObject(listaOSPadrao));
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
        public IHttpActionResult ObterLista([FromUri] TpStatusOSPadraoEntity statusOSPadraoEntity)
        {
            try
            {
                IList<TpStatusOSPadraoEntity> listaStatusOS = new List<TpStatusOSPadraoEntity>();
                listaStatusOS = new TpStatusOSPadraoData().ObterLista(statusOSPadraoEntity);

                JObject JO = new JObject();
                JO.Add("STATUS_OS", JArray.FromObject(listaStatusOS));
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
using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpMotivoVisitaPadraoAPI")]
    [Authorize]
    public class TpMotivoVisitaPadraoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_MOTIVO_VISITA)
        {
            try
            {
                TpMotivoVisitaPadraoEntity motivoVisitaPadraoEntity = new TpMotivoVisitaPadraoEntity();
                motivoVisitaPadraoEntity.ID_MOTIVO_VISITA = ID_MOTIVO_VISITA;
                var listaVisitaPadrao = new TpMotivoVisitaPadraoData().ObterLista(motivoVisitaPadraoEntity);

                JObject JO = new JObject();
                JO.Add("MOTIVO_VISITA", JArray.FromObject(listaVisitaPadrao));
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
        public IHttpActionResult ObterLista([FromUri] TpMotivoVisitaPadraoEntity motivoVisitaPadraoEntity)
        {
            try
            {
                IList<TpMotivoVisitaPadraoEntity> listaMotivoVisita = new List<TpMotivoVisitaPadraoEntity>();
                listaMotivoVisita = new TpMotivoVisitaPadraoData().ObterLista(motivoVisitaPadraoEntity);

                JObject JO = new JObject();
                JO.Add("MOTIVO_VISITA", JArray.FromObject(listaMotivoVisita));
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
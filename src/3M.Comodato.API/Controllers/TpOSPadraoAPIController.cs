using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpOSPadraoAPI")]
    [Authorize]
    public class TpOSPadraoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_TIPO_OS)
        {
            try
            {
                TpOSPadraoEntity tipoOSPadraoEntity = new TpOSPadraoEntity();
                tipoOSPadraoEntity.ID_TIPO_OS = ID_TIPO_OS;
                var listaTipoOSPadrao = new TpOSPadraoData().ObterLista(tipoOSPadraoEntity);

                JObject JO = new JObject();
                JO.Add("TIPO_OS", JArray.FromObject(listaTipoOSPadrao));
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
        public IHttpActionResult ObterLista([FromUri] TpOSPadraoEntity tipoOSPadraoEntity)
        {
            try
            {
                IList<TpOSPadraoEntity> listaOSPadrao = new List<TpOSPadraoEntity>();
                listaOSPadrao = new TpOSPadraoData().ObterLista(tipoOSPadraoEntity);

                JObject JO = new JObject();
                JO.Add("TIPO_OS", JArray.FromObject(listaOSPadrao));
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
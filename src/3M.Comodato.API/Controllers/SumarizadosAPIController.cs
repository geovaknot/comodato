using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/SumarizadosAPI")]
    [Authorize]
    public class SumarizadosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de pecas aprovadas
        /// </summary>
        ///<param name="DT_INICIAL"></param>
        ///<param name="DT_FINAL"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(SumarizadosEntity sumarizadosEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new SumarizadosData().ObterLista(sumarizadosEntity), Formatting.None));
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
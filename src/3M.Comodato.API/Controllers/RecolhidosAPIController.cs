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
    [RoutePrefix("api/RecolhidosAPI")]
    [Authorize]
    public class RecolhidosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de equipamentos recolhidos
        /// </summary>
        ///<param name="CD_CLIENTE"></param>
        ///<param name="CD_ATIVO_FIXO"></param>
        ///<param name="DT_DEV_INICIAL"></param>
        ///<param name="DT_DEV_FINAL"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(RecolhidosEntity recolhidosEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new RecolhidosData().ObterLista(recolhidosEntity), Formatting.None));
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
using System;
using System.Collections.Generic;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/AcompanhamentosAPI")]
    [Authorize]
    public class AcompanhamentosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de acompanhamento de preços
        /// </summary>
        ///<param name="CD_CLIENTE"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(List<string> cdCliente)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new AcompanhamentosData().ObterLista(cdCliente), Formatting.None));
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
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
    [RoutePrefix("api/CompletosAPI")]
    [Authorize]
    public class CompletosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de equipamentos em clientes
        /// </summary>
        ///<param name="CD_CLIENTE"></param>
        ///<param name="CD_EXECUTIVO"></param>
        ///<param name="CD_VENDEDOR"></param>
        ///<param name="CD_LINHA_PRODUTO"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(CompletosEntity completosEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new CompletosData().ObterLista(completosEntity), Formatting.None));
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
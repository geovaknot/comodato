using System;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/ResumosAPI")]
    [Authorize]
    public class ResumosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de consumo - resumido
        /// </summary>
        ///<param name="CD_LINHA_PRODUTO"></param>
        ///<param name="CD_CLIENTE"></param>
        ///<param name="CD_VENDEDOR"></param>
        ///<param name="CD_GRUPO"></param>
        ///<param name="CD_REGIAO"></param>
        ///<param name="CD_EXECUTIVO"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(ResumosEntity resumosEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new ResumosData().ObterLista(resumosEntity), Formatting.None));
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
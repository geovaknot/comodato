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
    [RoutePrefix("api/DevolvidosAPI")]
    [Authorize]
    public class DevolvidosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de equipamentos recolhidos
        /// </summary>
        ///<param name="CD_CLIENTE"></param>
        ///<param name="CD_VENDEDOR"></param>
        ///<param name="CD_GRUPO"></param>
        ///<param name="CD_ATIVO_FIXO"></param>
        ///<param name="CD_MOTIVO_DEVOLUCAO"></param>
        ///<param name="DT_DEV_INICIAL"></param>
        ///<param name="DT_DEV_FINAL"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(DevolvidosEntity devolvidosEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new DevolvidosData().ObterLista(devolvidosEntity), Formatting.None));
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
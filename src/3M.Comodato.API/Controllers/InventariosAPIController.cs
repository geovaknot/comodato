using System;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/InventariosAPI")]
    [Authorize]
    public class InventariosAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de equipamentos em clientes
        /// </summary>
        ///<param name="CD_CLIENTE"></param>
        ///<param name="CD_VENDEDOR"></param>
        ///<param name="CD_GRUPO"></param>
        ///<param name="CD_ATIVO_FIXO"></param>
        ///<param name="DT_INICIAL"></param>
        ///<param name="DT_FINAL"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(InventariosEntity inventariosEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new InventariosData().ObterLista(inventariosEntity), Formatting.None));
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
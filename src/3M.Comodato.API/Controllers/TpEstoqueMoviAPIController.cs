using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpEstoqueMoviAPI")]
    [Authorize]
    public class TpEstoqueMoviAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterListaTpEstoqueMoviSinc")]
        public IHttpActionResult ObterListaTpEstoqueMoviSinc()
        {
            IList<TpEstoqueMoviSinc> listaTpEstoqueMovi = new List<TpEstoqueMoviSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                TpEstoqueMoviData tpEstoqueMoviData = new TpEstoqueMoviData();
                listaTpEstoqueMovi = tpEstoqueMoviData.ObterListaTpEstoqueMoviSinc();

                JObject JO = new JObject();
                //JO.Add("ESTOQUE_MOVI", JsonConvert.SerializeObject(listaTpEstoqueMovi, Formatting.None));
                JO.Add("ESTOQUE_MOVI", JArray.FromObject(listaTpEstoqueMovi));
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

using _3M.Comodato.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/EstoqueMoviAPI")]
    public class EstoqueMoviAPIController : BaseAPIController
    {

        /// <summary>
        /// Obtem Lista de Movimentações de estoque 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObterListaEstoqueMoviSinc")]
        [Authorize]
        public IHttpActionResult ObterListaEstoqueMoviSinc(Int64 idUsuario)
        {
            IList<EstoqueMoviSinc> listaEstoqueMovi = new List<EstoqueMoviSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                EstoqueMoviData estoqueMoviData = new EstoqueMoviData();
                listaEstoqueMovi = estoqueMoviData.ObterListaEstoqueMoviSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("ESTOQUE_MOVI", JsonConvert.SerializeObject(listaEstoqueMovi, Formatting.None));
                JO.Add("ESTOQUE_MOVI", JArray.FromObject(listaEstoqueMovi));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recebe e grava Lista de Movimentações de Estoque
        /// </summary>
        /// <param name="JO">JObject contendo lista de Movimentações de Estoque ainda não sincronizadas JObject= </param>
        /// <returns></returns>
        [HttpPost] //---------------------------------- Em testes
        [Route("GravarListaEstoqueMoviSinc")]
        public IHttpActionResult GravarListaEstoqueMoviSinc(JObject JO) 
        {
            List<EstoqueMoviSinc> estoqueMovi = new List<EstoqueMoviSinc>();
            estoqueMovi = JsonConvert.DeserializeObject<List<EstoqueMoviSinc>>(JO["ESTOQUE_MOVI"].ToString());
            
            try
            {
                JO.Add("PEDIDO", JsonConvert.SerializeObject(estoqueMovi, Formatting.None));
                for (int i = 0; i < JO.Count; i++)
                {
                    Console.WriteLine(JO[i].Values());
                }



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

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
    [RoutePrefix("api/LoteAPI")]
    [Authorize]
    public class LoteAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de pedido de peças por lote
        /// </summary>
        ///<param name="ID_PEDIDO"></param>
        ///<param name="ID_LOTE_PEDIDO"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(Int64 ID_PEDIDO, Int64 ID_LOTE_PEDIDO)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("LOTE", JsonConvert.SerializeObject(new LotesData().ObterLista(ID_PEDIDO, ID_LOTE_PEDIDO), Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AtualizaRefNF")]
        public IHttpActionResult AtualizaRefNF(long ID_LOTE, string GUID)
        {
            try
            {                
                LotesData data = new LotesData();
                data.AtualizaRefNF(ID_LOTE, GUID);

                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));
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
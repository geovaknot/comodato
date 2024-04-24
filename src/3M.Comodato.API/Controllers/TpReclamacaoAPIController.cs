using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpReclamacaoAPI")]
    [Authorize]
    public class TpReclamacaoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_TIPO_RECLAMACAO)
        {
            try
            {
                TpReclamacaoEntity tipoReclamacaoEntity = new TpReclamacaoEntity();
                tipoReclamacaoEntity.ID_TIPO_RECLAMACAO = ID_TIPO_RECLAMACAO;
                var listaTipoReclamacao = new TpReclamacaoData().ObterLista(tipoReclamacaoEntity);

                JObject JO = new JObject();
                JO.Add("TIPO_RECLAMACAO", JArray.FromObject(listaTipoReclamacao));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }
 
        [HttpGet]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista([FromUri] TpReclamacaoEntity tipoReclamacaoEntity)
        {
            try
            {
                IList<TpReclamacaoEntity> listaTipoReclamacao = new TpReclamacaoData().ObterLista(tipoReclamacaoEntity);

                JObject JO = new JObject();
                JO.Add("TIPO_RECLAMACAO", JArray.FromObject(listaTipoReclamacao));
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
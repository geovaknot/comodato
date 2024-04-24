using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TpAtendimentoReclamacaoAPI")]
    [Authorize]
    public class TpAtendimentoReclamacaoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_TIPO_ATEND_RECLAMACAO)
        {
            try
            {
                TpAtendimentoReclamacaoEntity tipoAtendimentoReclamacaoEntity = new TpAtendimentoReclamacaoEntity();
                tipoAtendimentoReclamacaoEntity.ID_TIPO_ATEND_RECLAMACAO = ID_TIPO_ATEND_RECLAMACAO;
                var listaTipoAtendimentoReclamacao = new TpAtendimentoReclamacaoData().ObterLista(tipoAtendimentoReclamacaoEntity);

                JObject JO = new JObject();
                JO.Add("TIPO_RECLAMACAO", JArray.FromObject(listaTipoAtendimentoReclamacao));
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
        public IHttpActionResult ObterLista([FromUri] TpAtendimentoReclamacaoEntity tipoReclamacaoEntity)
        {
            try
            {
                IList<TpAtendimentoReclamacaoEntity> listaTipoAtendimentoReclamacao = new TpAtendimentoReclamacaoData().ObterLista(tipoReclamacaoEntity);

                JObject JO = new JObject();
                JO.Add("TIPO_RECLAMACAO", JArray.FromObject(listaTipoAtendimentoReclamacao));
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
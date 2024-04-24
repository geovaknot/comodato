using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/NotificacaoAPI")]
    [Authorize]
    public class NotificacaoAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public IHttpActionResult Incluir(NotificacaoEntity notificacaoEntity)
        {
            try
            {
                new NotificacaoData().Inserir(ref notificacaoEntity);

                JObject resultadoInclusao = new JObject
                {
                    { "SUCESSO", true },
                    { "ID_NOTIFICACAO", notificacaoEntity.ID_NOTIFICACAO }
                };
                return Ok(resultadoInclusao);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                JObject resultadoInclusao = new JObject
                {
                    { "SUCESSO", false },
                    { "MENSAGEM", ex.Message }
                };
                return Ok(resultadoInclusao);
            }
        }

        [HttpPost]
        [Route("Alterar")]
        public IHttpActionResult Alterar(NotificacaoEntity notificacaoEntity)
        {
            try
            {
                new NotificacaoData().Alterar(notificacaoEntity);

                JObject resultadoAlteracao = new JObject
                {
                    { "SUCESSO", true },
                    { "ID_NOTIFICACAO", notificacaoEntity.ID_NOTIFICACAO }
                };
                return Ok(resultadoAlteracao);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                JObject resultadoAlteracao = new JObject
                {
                    { "SUCESSO", false },
                    { "MENSAGEM", ex.Message }
                };
                return Ok(resultadoAlteracao);
            }
        }

        [HttpPost]
        [Route("Excluir")]
        public IHttpActionResult Excluir(NotificacaoEntity notificacaoEntity)
        {
            try
            {
                new NotificacaoData().Excluir(notificacaoEntity);

                JObject resultadoExclusao = new JObject
                {
                    { "SUCESSO", true },
                    { "ID_NOTIFICACAO", notificacaoEntity.ID_NOTIFICACAO }
                };
                return Ok(resultadoExclusao);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                JObject resultadoExclusao = new JObject
                {
                    { "SUCESSO", false },
                    { "MENSAGEM", ex.Message }
                };
                return Ok(resultadoExclusao);
            }
        }

        [HttpPost]
        [Route("ObterNotificacoes")]
        public IHttpActionResult ObterNotificacoes(NotificacaoEntity notificacaoEntity)
        {
            try
            {
                if (notificacaoEntity == null)
                    return BadRequest("Objeto para filtrar notificações não foi enviado.");

                var notificacoes = new NotificacaoData().ObterLista(notificacaoEntity);

                JObject resultadoObter = new JObject
                {
                    { "SUCESSO", true },
                    { "NOTIFICACOES", JArray.FromObject(notificacoes) }
                };
                return Ok(resultadoObter);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                JObject resultadoObter = new JObject
                {
                    { "SUCESSO", false },
                    { "MENSAGEM", ex.Message }
                };

                return Ok(resultadoObter);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/MensagemAPI")]
    [Authorize]
    public class MensagemAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(MensagemEntity mensagemEntity)
        {
            try
            {
                if (mensagemEntity.DT_OCORRENCIA == null)
                    mensagemEntity.DT_OCORRENCIA = DateTime.Now;

                new MensagemData().Inserir(ref mensagemEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_MENSAGEM = mensagemEntity.ID_MENSAGEM });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(MensagemEntity mensagemEntity)
        {
            try
            {
                new MensagemData().Alterar(mensagemEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_MENSAGEM = mensagemEntity.ID_MENSAGEM });
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(MensagemEntity mensagemEntity)
        {
            try
            {
                new MensagemData().Excluir(mensagemEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(MensagemEntity mensagemEntity)
        {
            List<MensagemEntity> mensagens = new List<MensagemEntity>();

            try
            {
                if (mensagemEntity == null)
                    mensagemEntity = new MensagemEntity();

                DataTableReader dataTableReader = new MensagemData().ObterLista(mensagemEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        mensagemEntity = new MensagemEntity();

                        mensagemEntity.ID_MENSAGEM = Convert.ToInt64("0" + dataTableReader["ID_MENSAGEM"]);

                        mensagens.Add(mensagemEntity);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidos = mensagens });
        }

    }
}
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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/UsuarioClienteAPI")]
    [Authorize]
    public class UsuarioClienteAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(UsuarioClienteEntity usuarioClienteEntity)
        {
            try
            {
                new UsuarioClienteData().Inserir(ref usuarioClienteEntity);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Cannot insert duplicate key"))
                    return Request.CreateResponse(HttpStatusCode.OK);

                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(UsuarioClienteEntity usuarioClienteEntity)
        {
            try
            {
                new UsuarioClienteData().Excluir(usuarioClienteEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
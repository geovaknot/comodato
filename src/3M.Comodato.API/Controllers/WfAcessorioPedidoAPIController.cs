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
    [RoutePrefix("api/WfAcessorioPedidoAPI")]
    [Authorize]
    public class WfAcessorioPedidoAPIController : BaseAPIController
    {

        [HttpPost]
        [Route("Adicionar")]
        public IHttpActionResult Adicionar(WfAcessorioPedidoEntity acessorioPedidoWorkflowEntity)
        {
            try
            {
                WfAcessorioPedidoData data = new WfAcessorioPedidoData();
                if (data.Inserir(ref acessorioPedidoWorkflowEntity))
                    return Ok(acessorioPedidoWorkflowEntity);
                else
                    return InternalServerError();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Remover")]
        public IHttpActionResult Remover(long ID_WF_ACESSORIO_EQUIP, long nidUsuario)
        {
            try
            {
                WfAcessorioPedidoEntity entity = new WfAcessorioPedidoEntity();
                entity.ID_WF_ACESSORIO_EQUIP = ID_WF_ACESSORIO_EQUIP;
                entity.nidUsuarioAtualizacao = nidUsuario;

                WfAcessorioPedidoData data = new WfAcessorioPedidoData();
                data.Excluir(entity);

                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemExclusaoSucesso, Formatting.None));
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

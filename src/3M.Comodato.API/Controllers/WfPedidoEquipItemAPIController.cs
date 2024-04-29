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
    [RoutePrefix("api/WfPedidoEquipItemAPI")]
    [Authorize]
    public class WfPedidoEquipItemAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Adicionar")]
        public IHttpActionResult Adicionar(WfPedidoEquipItemEntity equipItemEntity)
        {
            try
            {
                WfPedidoEquipItemData data = new WfPedidoEquipItemData();
                if (data.Inserir(ref equipItemEntity))
                    return Ok(equipItemEntity);
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
        public IHttpActionResult Remover(long id_wf_pedido_equip_item, long nidUsuario)
        {
            try
            {
                WfPedidoEquipItemEntity entity = new WfPedidoEquipItemEntity();
                entity.ID_WF_PEDIDO_EQUIP_ITEM = id_wf_pedido_equip_item;
                entity.nidUsuarioAtualizacao = nidUsuario;

                WfPedidoEquipItemData data = new WfPedidoEquipItemData();
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

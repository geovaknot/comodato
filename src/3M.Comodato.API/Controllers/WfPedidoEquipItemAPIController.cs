using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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
        [Route("VerificarGridEquipamentosDevolucao")]
        public IHttpActionResult VerificarGridEquipamentosDevolucao(int codigoWorkflow)
        {
            WfPedidoEquipItemEntity filter = new WfPedidoEquipItemEntity();
            filter.ID_WF_PEDIDO_EQUIP = codigoWorkflow;

            WfPedidoEquipItemData data = new WfPedidoEquipItemData();
            var listaData = data.ObterLista(filter);

            
            DataTable dataTable = listaData;

            int QuantidadeAnexos = 0;

            if (dataTable.Rows.Count < 1)
            {
                JObject JOo = new JObject();
                JOo.Add("Qtd", QuantidadeAnexos);
                return Ok(JOo);
            }

            try
            {

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    QuantidadeAnexos += 1;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject JO = new JObject();
            JO.Add("Qtd", QuantidadeAnexos);
            return Ok(JO);
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

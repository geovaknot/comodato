
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//------------------------------

namespace _3M.Comodato.API.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        public string PedidoCriadoEmail(WfPedidoEquipEntity pedido)
        {
            var view = AuxController.RenderViewToString("Email", "PedidoCriadoEmail", pedido);
            return view;
        }

        public string PedidoAlteradoEmail(WfPedidoEquipEntity pedido)
        {
            var view = AuxController.RenderViewToString("Email" ,"PedidoAlteradoEmail", pedido);
            return view;
        }

        public string PedidoSolicitadoEmail(WfPedidoEquipEntity pedido)
        {
            string gradeItens = string.Empty;

            var cliente = new ClienteData().ObterPorId(pedido.CD_CLIENTE);

            var pedidoEquipItens = new WfPedidoEquipItemData().ObterListaEntity(new WfPedidoEquipItemEntity() { ID_WF_PEDIDO_EQUIP = pedido.ID_WF_PEDIDO_EQUIP });

            var obj = new ExpandoObject();
            var model = obj as IDictionary<string, object>;
            model.Add("pedido", pedido);
            model.Add("cliente", cliente);
            model.Add("itens", pedidoEquipItens);


            var view = AuxController.RenderViewToString( "Email", "PedidoSolicitadoEmail", model);
            return view;
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
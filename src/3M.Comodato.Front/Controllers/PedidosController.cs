using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class PedidosController : BaseController
    {
        // GET: Devolvidos
        [_3MAuthentication]
        public ActionResult Index()
        {
            string data = DateTime.Now.ToString("dd/MM/yyyy");
            string dataMenos1Mes = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");           

            Models.PedidosDetalhe pedidosDetalhe = new Models.PedidosDetalhe
            {
                DT_FINAL = data,
                DT_INICIAL = dataMenos1Mes,
                clientes = new List<Models.Cliente>(),
                //vendedores = new List<Models.Vendedor>(),
                tecnicos = new List<Models.Tecnico>(),
                grupos = new List<Models.Grupo>()
            };
            var ArrayModeloRelatorio = new[]
{
                new SelectListItem { Value = "1", Text = "Modelo de Tabela Simplificado (Excel)" },
                new SelectListItem { Value = "2", Text = "Modelo de Tabela Completo" },
            };


            ViewBag.SelectModeloRelatorio = new SelectList(ArrayModeloRelatorio.ToList(), "Value", "Text");
            
            return View(pedidosDetalhe);
        }

        //[_3MAuthentication]
        //public ActionResult Imprimir(string idKey)
        //{
        //    return Redirect($"~/RelatorioPedidos.aspx?IdKey={ControlesUtility.Criptografia.Criptografar(idKey)}");
        //}
    }
}
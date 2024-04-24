using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class InventariosController : BaseController
    {
        // GET: Devolvidos
        [_3MAuthentication]
        public ActionResult Index()
        {
            //string data = DateTime.Now.ToString("dd/MM/yyyy");
            //string dataMenos1Mes = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");

            Models.InventariosDetalhe inventariosDetalhe = new Models.InventariosDetalhe
            {
                DT_FINAL = string.Empty, //data,
                DT_INICIAL = string.Empty, //dataMenos1Mes,
                clientes = new List<Models.Cliente>(),
                vendedores = new List<Models.Vendedor>(),
                grupos = new List<Models.Grupo>()
            };

            var ArraySitUltManutencao = new[]
 {
                new SelectListItem { Value = "1", Text = "Manutenção Vencida" },
                new SelectListItem { Value = "2", Text = "Manutenção Em Dia" },
            };


            ViewBag.SitUltManutencao = new SelectList(ArraySitUltManutencao.ToList(), "Value", "Text");


            return View(inventariosDetalhe);
        }
    }
}
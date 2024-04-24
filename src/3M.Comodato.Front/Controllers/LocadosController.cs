using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class LocadosController : BaseController
    {
        // GET: Locados
        [_3MAuthentication]
        public ActionResult Index()
        {
            string data = DateTime.Now.ToString("dd/MM/yyyy");
            string dataMenos1Mes = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");

            Models.LocadosDetalhe locadosDetalhe = new Models.LocadosDetalhe
            {
                DT_FINAL = data,
                DT_INICIAL = dataMenos1Mes,
                clientes = new List<Models.Cliente>(),
                vendedores = new List<Models.Vendedor>(),
                grupos = new List<Models.Grupo>()
            };
            return View(locadosDetalhe);
        }
    }
}
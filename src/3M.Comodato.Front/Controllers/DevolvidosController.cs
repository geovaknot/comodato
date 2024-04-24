using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class DevolvidosController : BaseController
    {
        // GET: Devolvidos
        [_3MAuthentication]
        public ActionResult Index()
        {
            string data = DateTime.Now.ToString("dd/MM/yyyy");
            string dataMenos1Mes = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");

            Models.DevolvidosDetalhe devolvidosDetalhe = new Models.DevolvidosDetalhe
            {
                DT_DEV_FINAL = data,
                DT_DEV_INICIAL = dataMenos1Mes,
                clientes = new List<Models.Cliente>(),
                vendedores = new List<Models.Vendedor>(),
                grupos = new List<Models.Grupo>(),
                motivos = new List<Models.MotivoDevolucao>()
            };
            return View(devolvidosDetalhe);
        }
    }
}
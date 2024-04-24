using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class SumarizadosController : BaseController
    {
        // GET: Sumarizados
        [_3MAuthentication]
        public ActionResult Index()
        {
            string data = DateTime.Now.ToString("dd/MM/yyyy");
            string dataMenos1Mes = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");

            Models.SumarizadosDetalhe sumarizadosDetalhe = new Models.SumarizadosDetalhe
            {
                DT_FINAL = data,
                DT_INICIAL = dataMenos1Mes,
            };
            return View(sumarizadosDetalhe);
        }
    }
}
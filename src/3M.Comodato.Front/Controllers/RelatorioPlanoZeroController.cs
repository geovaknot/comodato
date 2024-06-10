using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class RelatorioPlanoZeroController : BaseController
    {
        public ActionResult Index()
        {

            var ArrayModeloRelatorio = new[]
            {
                new SelectListItem { Value = "1", Text = "Modelo de Tabela Simplificado (Excel)" }
            };

            Models.RelatorioPlanoZero relatorioPlanoZero = new Models.RelatorioPlanoZero()
            {
                Periodos = new List<Models.Periodo>()
            };

            ViewBag.SelectModeloRelatorio = new SelectList(ArrayModeloRelatorio.ToList(), "Value", "Text");

            return View(relatorioPlanoZero);
        }
    }
}
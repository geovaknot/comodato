using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class RelatorioPecasController : BaseController
    {
        // GET: RelatorioPecas
        
        public ActionResult Index()
        {
            var ArrayModeloRelatorio = new[]
{
                new SelectListItem { Value = "1", Text = "Modelo de Tabela Simplificado (Excel)" }
            };

            var ArrayStatusRelatorio = new[]
{
                new SelectListItem { Value = "0", Text = "Ativo"},
                new SelectListItem { Value = "1", Text = "Inativo"}
            };

            ViewBag.SelectStatusRelatorio = new SelectList(ArrayStatusRelatorio.ToList(), "Value", "Text"); 
            ViewBag.SelectModeloRelatorio = new SelectList(ArrayModeloRelatorio.ToList(), "Value", "Text");


            return View();
        }
    }
}
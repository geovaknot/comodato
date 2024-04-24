using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class AcompanhamentosController : BaseController
    {
        // GET: Devolvidos
        [_3MAuthentication]
        public ActionResult Index()
        {

            Models.Acompanhamentos acompanhamentos = new Models.Acompanhamentos
            {
                clientes = new List<Models.Cliente>(),
            };
            return View(acompanhamentos);
        }
    }
}
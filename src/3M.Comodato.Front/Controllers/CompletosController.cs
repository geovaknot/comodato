using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class CompletosController : BaseController
    {
        // GET: Devolvidos
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.CompletosDetalhe completo = new Models.CompletosDetalhe
            {
                produtos = new List<Models.LinhaProduto>(),
                clientes = new List<Models.Cliente>(),
                executivos = new List<Models.Executivo>(),
                vendedores = new List<Models.Vendedor>()                
            };
            return View(completo);
        }
    }
}
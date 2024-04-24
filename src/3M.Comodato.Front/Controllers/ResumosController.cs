using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class ResumosController : BaseController
    {
        // GET: Devolvidos
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.resumosDetalhe resumo = new Models.resumosDetalhe
            {
                produtos = new List<Models.LinhaProduto>(),
                clientes = new List<Models.Cliente>(),
                vendedores = new List<Models.Vendedor>(),
                regioes = new List<Models.Regiao>(),
                grupos = new List<Models.Grupo>(),
                executivos = new List<Models.Executivo>()                
            };
            return View(resumo);
        }
    }
}
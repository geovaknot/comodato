using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    [_3MAuthentication]
    public class KatTecnicoVisitaController : BaseController
    {
        // GET: KatTecnico
        //[_3MAuthentication]
        public ActionResult Index()
        {
            return View();
        }
    }
}
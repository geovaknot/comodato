using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class DistribuidorController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index(string idKey)
        {
            return View();
        }
    }
}
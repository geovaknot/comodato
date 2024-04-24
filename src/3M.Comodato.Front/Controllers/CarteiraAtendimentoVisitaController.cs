using _3M.Comodato.Utility;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class CarteiraAtendimentoVisitaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        //[_3MAuthentication]
        //public ActionResult Imprimir(string idKey)
        //{
        //    idKey = ControlesUtility.Criptografia.Criptografar(idKey);
        //    return Redirect($"~/RelatorioCarteiraAtendimento.aspx?IdKey={idKey}");
        //}
    }
}
using _3M.Comodato.Utility;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class ManutencaoController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            var ArrayModeloRelatorio = new[]
{
                new SelectListItem { Value = "1", Text = "Modelo de Tabela Simplificado (Excel)" },
                new SelectListItem { Value = "2", Text = "Modelo de Tabela Completo" },
            };


            ViewBag.SelectModeloRelatorio = new SelectList(ArrayModeloRelatorio.ToList(), "Value", "Text");


            return View();
        }

        //[_3MAuthentication]
        //public ActionResult Imprimir(string idKey)
        //{
        //    idKey = ControlesUtility.Criptografia.Criptografar(idKey);
        //    return Redirect($"~/RelatorioManutencao.aspx?IdKey={idKey}");
        //}
    }
}
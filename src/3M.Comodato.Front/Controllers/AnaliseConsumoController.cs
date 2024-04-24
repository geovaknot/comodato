using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class AnaliseConsumoController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            ViewBag.ListaExecutivos = ObterListaExecutivos();
            ViewBag.ListaVendedores = ObterListaVendedores();
            ViewBag.ListaAgrupamento = ObterListaAgrupamento();

            return View();
        }

        private List<SelectListItem> ObterListaExecutivos()
        {
            ExecutivoEntity executivoEntity = new ExecutivoEntity();
            executivoEntity.bidAtivo = true;

            ExecutivoData data = new ExecutivoData();
            DataTable dt = data.ObterLista(executivoEntity);

            List<SelectListItem> lista = (from r in dt.Rows.Cast<DataRow>()
                                          select new SelectListItem() { Text = r["NM_EXECUTIVO"].ToString(), Value = r["CD_EXECUTIVO"].ToString() }).ToList();
            lista.Insert(0, new SelectListItem());
            return lista;
        }

        private List<SelectListItem> ObterListaVendedores()
        {
            VendedorEntity vendedorEntity = new VendedorEntity();
            vendedorEntity.FL_ATIVO = "S";
            VendedorData data = new VendedorData();
            DataTable dt = data.ObterLista(vendedorEntity);

            List<SelectListItem> lista = (from r in dt.Rows.Cast<DataRow>()
                                          select new SelectListItem() { Text = r["NM_VENDEDOR"].ToString(), Value = r["CD_VENDEDOR"].ToString() }).ToList();

            lista.Insert(0, new SelectListItem());
            return lista;
        }

        private List<SelectListItem> ObterListaAgrupamento()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            lista.Add(new SelectListItem() { Value="1",Text= "Cliente" } );
            lista.Add(new SelectListItem() { Value = "2", Text = "Grupo" });
            lista.Add(new SelectListItem() { Value = "3", Text = "Região" });
            lista.Add(new SelectListItem() { Value = "4", Text = "Executivo" });
            lista.Add(new SelectListItem() { Value = "5", Text = "Vendedor" });
            return lista;
        }

        //[_3MAuthentication]
        //public ActionResult Imprimir(string idKey)
        //{
        //    idKey = ControlesUtility.Criptografia.Criptografar(idKey);
        //    return Redirect($"~/RelatorioConsumo.aspx?IdKey={idKey}");
        //}

    }
}
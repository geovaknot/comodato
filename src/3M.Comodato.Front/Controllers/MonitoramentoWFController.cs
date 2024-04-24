using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class MonitoramentoWFController : BaseController
    {
        // GET: Monitoramento
        [_3MAuthentication]
        public ActionResult Index()
        {
            string data = DateTime.Now.ToString("dd/MM/yyyy");
            string dataMenos1Mes = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");

            Models.MonitoramentoWF mon = new Models.MonitoramentoWF
            {
                DT_FINAL = data,
                DT_INICIAL = dataMenos1Mes,
                ListaStatus = new SelectList(ControlesUtility.Dicionarios.TipoPedidoWorkflow(), "value", "key")
            };
            return View(mon);
        }

        [HttpPost]
        public string CarregarMonitoramentoView(MonitoramentoWF pesquisa)
        {
            DateTime inicio = Convert.ToDateTime(pesquisa.DT_INICIAL + " 00:00:00.000");
            DateTime final = Convert.ToDateTime(pesquisa.DT_FINAL + " 23:59:59.999");

            MonitoramentoWFEntity monitoramento = new MonitoramentoWFEntity();
            monitoramento.DT_INICIAL = inicio;
            monitoramento.DT_FINAL = final;
            monitoramento.TIPO = pesquisa.TIPO;
            List<MonitoramentoWFEntity> lista = new MonitoramentoWFData().ObterListaEntity(monitoramento);

            var view = RenderRazorViewToString("gridMonitoramentoWF", lista);
            return view;
        }
    }
}